using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using TabNoc.PiWeb.DataTypes.WateringWeb.Settings;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SettingsController : ControllerBase
	{
		private readonly NpgsqlConnection _connection;

		public SettingsController(NpgsqlConnection connection)
		{
			_connection = connection;
		}

		[HttpGet]
		public ActionResult<SettingsData> Get()
		{
			SettingsData data = new SettingsData
			{
				Valid = true,
				HumiditySensors = new Dictionary<string, string>()
			};
			lock (_connection)
			{
				using (NpgsqlCommand command = _connection.CreateCommand())
				{
					command.CommandText = "select enabled, location, location_friendly_name, override_value, weather_enabled from t_settings;";
					using (NpgsqlDataReader dataReader = command.ExecuteReader())
					{
						if (dataReader.Read() == false)
						{
							return NoContent();
						}
						data.Enabled = dataReader.GetBoolean(0);
						data.Location = dataReader.GetString(1);
						data.LocationFriendlyName = dataReader.GetString(2);
						data.OverrideValue = dataReader.GetInt32(3);
						data.WeatherEnabled = dataReader.GetBoolean(4);
					}
					command.CommandText = "select friendly_name, real_name from t_humidity_sensor where settings_id = 1;";
					using (NpgsqlDataReader dataReader = command.ExecuteReader())
					{
						while (dataReader.Read())
						{
							data.HumiditySensors.Add(dataReader.GetString(1), dataReader.GetString(0));
						}
					}
				}
			}
			return Ok(data);
		}

		[HttpGet("enabled")]
		public ActionResult<bool> GetEnabled()
		{
			return Ok(true);
		}

		[HttpPut]
		public ActionResult Put([FromBody] SettingsData settingsData)
		{
			lock (_connection)
			{
				using (NpgsqlCommand command = _connection.CreateCommand())
				{
					command.Transaction = _connection.BeginTransaction();
					command.CommandText = "select 1 from t_settings;";
					NpgsqlDataReader reader = command.ExecuteReader();
					if (reader.Read() == false)
					{
						reader.Dispose();
						command.CommandText = $@"
INSERT INTO t_settings (id, enabled, location, location_friendly_name, override_value, weather_enabled)
VALUES (DEFAULT, false, '', '', 100, false);";
						command.ExecuteNonQuery();
					}
					else
					{
						reader.Dispose();
					}

					// select where id == 1 if none, then insert

					command.CommandText = $@"
UPDATE t_settings
SET enabled					= {settingsData.Enabled},
	location				= '{settingsData.Location}',
	location_friendly_name	= '{settingsData.LocationFriendlyName}',
	override_value			= {settingsData.OverrideValue},
	weather_enabled			= {settingsData.WeatherEnabled}
WHERE id = 1;";
					command.ExecuteNonQuery();

					command.CommandText = @"
TRUNCATE TABLE ONLY t_humidity_sensor;";
					command.ExecuteNonQuery();

					command.CommandText = "INSERT INTO t_humidity_sensor (settings_id, friendly_name, real_name) VALUES (1, @friendlyName, @realName);";
					foreach ((string realSensorName, string friendlySensorName) in settingsData.HumiditySensors)
					{
						command.Parameters.AddWithValue("@friendlyName", NpgsqlDbType.Text, friendlySensorName);
						command.Parameters.AddWithValue("@realName", NpgsqlDbType.Text, realSensorName);
						command.ExecuteNonQuery();
					}
					command.Transaction.Commit();
				}
			}
			return NoContent();
		}
	}
}

using Npgsql;
using System;
using TabNoc.PiWeb.WateringWebServer.other;

namespace TabNoc.PiWeb.WateringWebServer.TempManager
{
	public class TemperaturReader
	{
		public static (DateTime, int) ReadInternTemperature()
		{
			const string sensorName = "88";
			using (ConnectionPool.ConnectionUsable usable = new ConnectionPool.ConnectionUsable())
			{
				NpgsqlCommand command = usable.Connection.CreateCommand();
				command.CommandText = @"
SELECT timestamp, value
FROM t_temperature
WHERE source=@name
ORDER BY timestamp DESC
LIMIT 1";
				command.Parameters.AddWithValue("@name", sensorName);
				using (NpgsqlDataReader dataReader = command.ExecuteReader())
				{
					if (dataReader.Read())
					{
						return (dataReader.GetDateTime(0), dataReader.GetInt32(1));
					}
					else
					{
						throw new Exception("Es konnten keine Daten aus der Datnbank gelesen werden!");
					}
				}
			}
		}
	}
}

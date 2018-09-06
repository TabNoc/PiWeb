using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TabNoc.PiWeb.DataTypes.WateringWeb.Settings;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HumidityController : ControllerBase
	{
		public HumidityController()
		{
		}

		[HttpGet]
		public ActionResult<HumiditySensorData> Get()
		{
			HumiditySensorData data = new HumiditySensorData
			{
				Valid = true,
				HumiditySensors = new List<string>() { "testSensorEintrag" }
			};
			//lock (_connection)
			//{
			//	using (NpgsqlCommand npgsqlCommand = _connection.CreateCommand())
			//	{
			//		npgsqlCommand.CommandText = "select * from t_history order by msgtimestamp desc;";
			//		using (NpgsqlDataReader dataReader = npgsqlCommand.ExecuteReader())
			//		{
			//			while (dataReader.Read())
			//			{
			//				data.HistoryElements.Add(new HistoryElement((DateTime)dataReader[0], (string)dataReader[1], (string)dataReader[2], (string)dataReader[3]));
			//			}
			//		}
			//	}
			//}
			return Ok(data);
		}

		[HttpGet("enabled")]
		public ActionResult<bool> GetEnabled()
		{
			return Ok(true);
		}
	}
}

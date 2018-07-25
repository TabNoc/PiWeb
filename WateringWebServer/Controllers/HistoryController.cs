using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TabNoc.PiWeb.DataTypes.WateringWeb.History;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HistoryController : ControllerBase
	{
		private readonly NpgsqlConnection _connection;

		public HistoryController(NpgsqlConnection connection)
		{
			_connection = connection;
		}

		[HttpGet]
		public ActionResult<HistoryData> Get()
		{
			HistoryData data = new HistoryData
			{
				Valid = true
			};
			lock (_connection)
			{
				using (NpgsqlCommand npgsqlCommand = _connection.CreateCommand())
				{
					npgsqlCommand.CommandText = "select * from t_history order by msgtimestamp desc;";
					using (NpgsqlDataReader dataReader = npgsqlCommand.ExecuteReader())
					{
						while (dataReader.Read())
						{
							data.HistoryElements.Add(new HistoryElement((DateTime)dataReader[0], (string)dataReader[1], (string)dataReader[2], (string)dataReader[3]));
						}
					}
				}
			}
			return Ok(data);
		}

		[HttpGet("range")]
		public ActionResult<IEnumerable<HistoryElement>> GetRange([FromQuery(Name = "from")] DateTime from, [FromQuery(Name = "amount")] int amount)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"GetRange({from},{amount})");
			Console.ResetColor();
			List<HistoryElement> returnval = new List<HistoryElement>();
			lock (_connection)
			{
				using (NpgsqlCommand command = _connection.CreateCommand())
				{
					command.CommandText = "select * from t_history where msgtimestamp >= @fromTime order by msgtimestamp desc limit @amount;";
					command.Parameters.AddWithValue("@fromTime", from);
					command.Parameters.AddWithValue("@amount", amount);
					using (NpgsqlDataReader dataReader = command.ExecuteReader())
					{
						while (dataReader.Read())
						{
							returnval.Add(new HistoryElement((DateTime)dataReader[0], (string)dataReader[1], (string)dataReader[2], (string)dataReader[3]));
						}
					}
				}
			}

			return Ok(returnval);
		}

		[HttpGet("{id}")]
		public ActionResult<HistoryElement> Get(int id)
		{
			throw new NotImplementedException();
		}

		[HttpPost]
		public ActionResult CreateNewEntry(HistoryElement element)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			lock (_connection)
			{
				using (NpgsqlCommand command = _connection.CreateCommand())
				{
					command.CommandText = $"INSERT INTO t_history(msgtimestamp, source, status, message) Values (@time, '{element.Source}', '{element.Status}', '{element.Message}');";
					command.Parameters.AddWithValue("@time", element.TimeStamp);
					command.ExecuteNonQuery();
				}
			}

			return Ok();
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System;
using System.Collections.Generic;
using NpgsqlTypes;
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
					command.Parameters.AddWithValue("@time", NpgsqlDbType.Timestamp, element.TimeStamp);
					command.ExecuteNonQuery();
				}
			}

			return Ok();
		}

		/// <summary>
		/// Fragt alle Verlaufsdaten des Servers ohne beschränkung der Menge oder Filterung ab.
		/// </summary>
		/// <returns>Gibt ein <see cref="HistoryData"/> Objekt zurück welches für die Serialisierung verwedet wird.</returns>
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

		/// <summary>
		/// Ruft ein bestimmten Eintrag mittels des PrimaryKeys ab
		/// </summary>
		/// <param name="primaryKey"></param>
		/// <returns></returns>
		[HttpGet("{primaryKey}")]
		public ActionResult<HistoryElement> Get(DateTime primaryKey)
		{
			lock (_connection)
			{
				using (NpgsqlCommand command = _connection.CreateCommand())
				{
					command.CommandText = "select * from t_history where msgtimestamp == @primaryKey;";
					command.Parameters.AddWithValue("@primaryKey", NpgsqlDbType.Timestamp, primaryKey);
					using (NpgsqlDataReader dataReader = command.ExecuteReader())
					{
						while (dataReader.Read())
						{
							return Ok(new HistoryElement((DateTime)dataReader[0], (string)dataReader[1], (string)dataReader[2], (string)dataReader[3]));
						}
					}
				}
			}
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Fragt die Menge an Verlaufseinträgen ohne Filterung ab .
		/// </summary>
		/// <returns>Anzahl an Elementen</returns>
		[HttpGet("amount")]
		public ActionResult<int> GetAmount()
		{
			lock (_connection)
			{
				using (NpgsqlCommand npgsqlCommand = _connection.CreateCommand())
				{
					npgsqlCommand.CommandText = "select count(*) from t_history;";
					using (NpgsqlDataReader dataReader = npgsqlCommand.ExecuteReader())
					{
						while (dataReader.Read())
						{
							return Ok(dataReader[0]);
						}
					}
				}
			}
			throw new InvalidOperationException();
		}

		[HttpGet("range")]
		public ActionResult<IEnumerable<HistoryElement>> GetRange([FromQuery(Name = "primaryKey")] DateTime primaryKey, [FromQuery(Name = "takeAmount")] int takeAmount)
		{
			List<HistoryElement> returnval = new List<HistoryElement>();
			lock (_connection)
			{
				using (NpgsqlCommand command = _connection.CreateCommand())
				{
					if (primaryKey == default(DateTime))
					{
						command.CommandText = "select * from t_history order by msgtimestamp desc limit @amount;";
					}
					else
					{
						command.CommandText = "select * from t_history where msgtimestamp <= @fromTime order by msgtimestamp desc limit @amount;";
						command.Parameters.AddWithValue("@fromTime", NpgsqlDbType.Timestamp, primaryKey);
					}

					command.Parameters.AddWithValue("@amount", NpgsqlDbType.Integer, takeAmount);
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

		[HttpGet("search")]
		public ActionResult<IEnumerable<HistoryElement>> GetSearched([FromQuery(Name = "searchString")] string searchString, [FromQuery(Name = "collumn")]int collumn, [FromQuery(Name = "amount")]int amount)
		{
			List<HistoryElement> returnval = new List<HistoryElement>();
			lock (_connection)
			{
				using (NpgsqlCommand command = _connection.CreateCommand())
				{
					command.CommandText = "select * from t_history where " + GetCollumnName(collumn) + "::text like '%" + searchString + "%' order by msgtimestamp desc limit @amount;";
					//command.Parameters.AddWithValue("@tableName", GetCollumnName(collumn));
					//command.Parameters.AddWithValue("@searchstring", searchString);
					command.Parameters.AddWithValue("@amount", NpgsqlDbType.Integer, amount);
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

		private string GetCollumnName(int collumn)
		{
			switch (collumn)
			{
				case 0:
					return "msgtimestamp";

				case 1:
					return "source";

				case 2:
					return "status";

				case 3:
					return "message";

				default:
					throw new IndexOutOfRangeException();
			}
		}
	}
}

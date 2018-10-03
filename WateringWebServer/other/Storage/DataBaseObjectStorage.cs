using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Linq;

namespace TabNoc.PiWeb.WateringWebServer.other.Storage
{
	public static class DataBaseObjectStorage
	{
		public static T LoadFromDataBase<T>(Func<T> computeDefaultValue = null)
		{
			using (ConnectionPool.ConnectionUsable usable = new ConnectionPool.ConnectionUsable())
			using (NpgsqlCommand command = usable.Connection.CreateCommand())
			{
				InitTableRow<T>(command);

				command.CommandText = "SELECT serialized_object FROM t_object_storage WHERE data_type_name = @type_name;";
				command.Parameters.AddWithValue("@type_name", NpgsqlDbType.Text, GetTypeName<T>());
				using (NpgsqlDataReader reader = command.ExecuteReader())
				{
					return reader.Read() == false
						? (computeDefaultValue == null ? default(T) : computeDefaultValue())
						: JsonConvert.DeserializeObject<T>(reader.GetString(0));
				}
			}
		}

		public static (T, (NpgsqlTransaction, NpgsqlCommand, ConnectionPool.ConnectionPoolItem)) LoadFromDataBaseLocked<T>(Func<T> computeDefaultValue = null)
		{
			ConnectionPool.ConnectionPoolItem connectionPoolItem = ConnectionPool.GetConnection();
			NpgsqlCommand command = connectionPoolItem.Connection.CreateCommand();
			{
				int sleepCounter = 0;
				NpgsqlTransaction transaction = connectionPoolItem.Connection.BeginTransaction();
				command.Transaction = transaction;
				InitTableRow<T>(command);

				command.Parameters.AddWithValue("@type_name", NpgsqlDbType.Text, GetTypeName<T>());
				while (true)
				{
					command.CommandText = "SELECT locked FROM t_object_storage WHERE data_type_name = @type_name FOR UPDATE;";
					using (NpgsqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							// Row does not exists -> Problem. For now I just break, table will be inserted when save is called.
							//throw new Exception("Reader did not Read!");
							break;
						}
						else
						{
							if (reader.GetBoolean(0))
							{
								// code is obsolete, database lock works
								reader.Dispose();
								System.Threading.Thread.Sleep(10);
								sleepCounter += 10;
								Console.WriteLine("Database is Locked!");
							}
							else
							{
								reader.Dispose();
								command.CommandText = "UPDATE t_object_storage SET locked = TRUE WHERE data_type_name = @type_name;";
								command.ExecuteNonQuery();
								break;
							}
						}
					}

					if (sleepCounter > 10000)
					{
						throw new Exception($"The lock request for {nameof(T)} was not fulfilled in 10s");
					}
				}

				command.CommandText = "SELECT serialized_object FROM t_object_storage WHERE data_type_name = @type_name;";
				using (NpgsqlDataReader reader = command.ExecuteReader())
				{
					return (reader.Read() == false
						? (computeDefaultValue == null ? default(T) : computeDefaultValue())
						: JsonConvert.DeserializeObject<T>(reader.GetString(0)), (transaction, command, connectionPoolItem));
				}
			}
		}

		public static void SaveToDataBase<T>(T data)
		{
			string serializedObject = JsonConvert.SerializeObject(data, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				PreserveReferencesHandling = PreserveReferencesHandling.Objects
			});

			using (ConnectionPool.ConnectionUsable usable = new ConnectionPool.ConnectionUsable())
			using (NpgsqlCommand command = usable.Connection.CreateCommand())
			{
				command.CommandText = "UPDATE t_object_storage SET serialized_object = @serialized_object WHERE data_type_name = @type_name;";
				command.Parameters.AddWithValue("@type_name", NpgsqlDbType.Text, GetTypeName<T>());
				command.Parameters.AddWithValue("@serialized_object", NpgsqlDbType.Text, serializedObject);
				command.ExecuteNonQuery();
			}
		}

		public static void SaveToLockedDataBase<T>(T data, (NpgsqlTransaction, NpgsqlCommand, ConnectionPool.ConnectionPoolItem) connectionItems)
		{
			string serializedObject = JsonConvert.SerializeObject(data, new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
				PreserveReferencesHandling = PreserveReferencesHandling.Objects
			});

			{
				connectionItems.Item2.CommandText = "UPDATE t_object_storage SET serialized_object = @serialized_object WHERE data_type_name = @type_name;";
				connectionItems.Item2.Parameters.AddWithValue("@serialized_object", NpgsqlDbType.Text, serializedObject);
				connectionItems.Item2.ExecuteNonQuery();

				connectionItems.Item2.CommandText = "UPDATE t_object_storage SET locked = FALSE WHERE data_type_name = @type_name;";
				connectionItems.Item2.ExecuteNonQuery();
			}
			connectionItems.Item1.Commit();
			connectionItems.Item1.Dispose();
			connectionItems.Item2.Dispose();
			ConnectionPool.ReleaseConnection(connectionItems.Item3);
		}

		private static string GetTypeName<T>()
		{
			return typeof(T).IsGenericType ? typeof(T).Name + "<" + typeof(T).GetGenericArguments().Select(type => type.Name).Join() + ">" : typeof(T).Name;
		}

		private static void InitTableRow<T>(NpgsqlCommand command)
		{
			command.CommandText = "SELECT 1 FROM t_object_storage WHERE data_type_name = @type_name;";
			command.Parameters.AddWithValue("@type_name", NpgsqlDbType.Text, GetTypeName<T>());
			NpgsqlDataReader firstItemReader = command.ExecuteReader();
			if (firstItemReader.Read() == false)
			{
				firstItemReader.Dispose();
				command.CommandText = "INSERT INTO t_object_storage (data_type_name) VALUES (@type_name);";
				command.ExecuteNonQuery();
			}
			else
			{
				firstItemReader.Dispose();
			}
		}
	}
}

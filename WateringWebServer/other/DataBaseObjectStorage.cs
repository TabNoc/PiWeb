using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Linq;

namespace TabNoc.PiWeb.WateringWebServer.other
{
	public static class DataBaseObjectStorage
	{
		public static T LoadFromDataBase<T>(Func<T> computeDefaultValue = null)
		{
			using (ConnectionPool.ConnectionUsable usable = new ConnectionPool.ConnectionUsable())
			using (NpgsqlCommand command = usable.Connection.CreateCommand())
			{
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
				command.CommandText = "SELECT 1 FROM t_object_storage WHERE data_type_name = @type_name;";
				command.Parameters.AddWithValue("@type_name", NpgsqlDbType.Text, GetTypeName<T>());
				NpgsqlDataReader reader = command.ExecuteReader();
				if (reader.Read() == false)
				{
					reader.Dispose();
					command.CommandText = "INSERT INTO t_object_storage (data_type_name) VALUES (@type_name);";
					command.Parameters.AddWithValue("@type_name", NpgsqlDbType.Text, GetTypeName<T>());
					command.ExecuteNonQuery();
				}
				else
				{
					reader.Dispose();
				}

				command.CommandText = "UPDATE t_object_storage SET serialized_object = @serialized_object WHERE data_type_name = @type_name;";
				command.Parameters.AddWithValue("@type_name", NpgsqlDbType.Text, GetTypeName<T>());
				command.Parameters.AddWithValue("@serialized_object", NpgsqlDbType.Text, serializedObject);
				command.ExecuteNonQuery();
			}
		}

		private static string GetTypeName<T>()
		{
			return typeof(T).IsGenericType ? typeof(T).Name + "<" + typeof(T).GetGenericArguments().Select(type => type.Name).Join() + ">" : typeof(T).Name;
		}
	}

	public class DatabaseObjectStorageEntryUsable<T> : IDisposable
	{
		public readonly T Data;

		public DatabaseObjectStorageEntryUsable(T data)
		{
			//TODO: create dblock or compare Data before saving
			Data = data;
		}

		public static DatabaseObjectStorageEntryUsable<T> GetData(Func<T> computeDefaultValue = null)
		{
			return new DatabaseObjectStorageEntryUsable<T>(DataBaseObjectStorage.LoadFromDataBase(computeDefaultValue));
		}

		public void Dispose()
		{
			DataBaseObjectStorage.SaveToDataBase(Data);
		}
	}
}

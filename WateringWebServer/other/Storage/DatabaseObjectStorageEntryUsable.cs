using System;
using Npgsql;

namespace TabNoc.PiWeb.WateringWebServer.other.Storage
{
	public class DatabaseObjectStorageEntryUsable<T> : IDisposable
	{
		public readonly T Data;
		private readonly (NpgsqlTransaction, NpgsqlCommand, ConnectionPool.ConnectionPoolItem) _connectionItems;
		private readonly bool _useLocked;

		private DatabaseObjectStorageEntryUsable(T data)
		{
			Data = data;
			_useLocked = false;
		}

		private DatabaseObjectStorageEntryUsable((T, (NpgsqlTransaction, NpgsqlCommand, ConnectionPool.ConnectionPoolItem)) data)
		{
			Data = data.Item1;
			_connectionItems = data.Item2;
			_useLocked = true;
		}

		public static DatabaseObjectStorageEntryUsable<T> GetData(Func<T> computeDefaultValue = null)
		{
			return new DatabaseObjectStorageEntryUsable<T>(DataBaseObjectStorage.LoadFromDataBase(computeDefaultValue));
		}

		public static DatabaseObjectStorageEntryUsable<T> GetDataLocked(Func<T> computeDefaultValue = null)
		{
			return new DatabaseObjectStorageEntryUsable<T>(DataBaseObjectStorage.LoadFromDataBaseLocked(computeDefaultValue));
		}

		public void Dispose()
		{
			if (_useLocked == true)
			{
				DataBaseObjectStorage.SaveToLockedDataBase(Data, _connectionItems);
			}
			else
			{
				DataBaseObjectStorage.SaveToDataBase(Data);
			}
		}
	}
}
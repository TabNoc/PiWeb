using Npgsql;
using System;

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

		#region Dispose Pattern

		private bool _disposed;

		~DatabaseObjectStorageEntryUsable()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		private void Dispose(bool disposing)
		{
			try
			{
				if (!_disposed)
				{
					if (_useLocked == true)
					{
						DataBaseObjectStorage.SaveToLockedDataBase(Data, _connectionItems);
					}
					else
					{
						DataBaseObjectStorage.SaveToDataBase(Data);
					}

					_disposed = true;
				}
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		#endregion Dispose Pattern
	}
}

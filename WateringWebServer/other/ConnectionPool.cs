using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TabNoc.PiWeb.WateringWebServer.other
{
	public static class ConnectionPool
	{
		private static readonly List<ConnectionPoolItem> Pool = new List<ConnectionPoolItem>();

		static ConnectionPool()
		{
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				NpgsqlConnection connection = new NpgsqlConnection(PrivateData.ConnectionStringBuilder.ToString());
				connection.Open();
				Pool.Add(new ConnectionPoolItem(connection, i));
			}
		}

		public static ConnectionPoolItem GetConnection()
		{
			while (true)
			{
				lock (Pool)
				{
					if (Pool.Any(item => item.ConnectionIsUsed == false))
					{
						ConnectionPoolItem connectionPoolItem = Pool.First(item => item.ConnectionIsUsed == false);
						connectionPoolItem.ConnectionIsUsed = true;
						return connectionPoolItem;
					}
				}
				System.Threading.Thread.Sleep(10);
			}
		}

		public static void ReleaseConnection(ConnectionPoolItem connectionItem)
		{
			lock (Pool)
			{
				try
				{
					ConnectionPoolItem connectionPoolItem = Pool.First(item => item == connectionItem);
					connectionPoolItem.ConnectionIsUsed = false;
				}
				catch (Exception e)
				{
					Console.WriteLine("Das Item:" + connectionItem.Number + " wurde nicht gefunden");
					throw;
				}
			}
		}

		public class ConnectionUsable : IDisposable
		{
			private readonly ConnectionPoolItem _connectionPoolItem;

			public NpgsqlConnection Connection
			{
				get
				{
					lock (_connectionPoolItem)
					{
						if (_connectionPoolItem.Connection.State != ConnectionState.Open)
						{
							_connectionPoolItem.Connection.Open();
						}
						return _connectionPoolItem.Connection;
					}
				}
			}

			public ConnectionUsable()
			{
				_connectionPoolItem = ConnectionPool.GetConnection();
			}

			public void Dispose()
			{
				ConnectionPool.ReleaseConnection(_connectionPoolItem);
			}
		}

		public class ConnectionPoolItem
		{
			public bool ConnectionIsUsed;
			private readonly NpgsqlConnection _connection;
			public readonly int Number;

			public NpgsqlConnection Connection
			{
				get
				{
					lock (_connection)
					{
						if (_connection.State != ConnectionState.Open)
						{
							_connection.Open();
						}
						return _connection;
					}
				}
			}

			public ConnectionPoolItem(NpgsqlConnection connection, int number)
			{
				_connection = connection;
				Number = number;
				ConnectionIsUsed = false;
			}
		}
	}
}

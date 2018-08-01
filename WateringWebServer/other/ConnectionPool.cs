using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TabNoc.PiWeb.WateringWebServer.other
{
	public class ConnectionPool
	{
		private static ConnectionPool _instance;

		private readonly List<ConnectionPoolItem> _pool = new List<ConnectionPoolItem>();

		public static ConnectionPool Instance => _instance ?? (_instance = new ConnectionPool());

		private ConnectionPool()
		{
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				NpgsqlConnection connection = new NpgsqlConnection(PrivateData.ConnectionStringBuilder.ToString());
				connection.Open();
				_pool.Add(new ConnectionPoolItem(connection, i));
			}
		}

		public ConnectionPoolItem GetConnection()
		{
			while (true)
			{
				lock (_pool)
				{
					if (_pool.Any(item => item.ConnectionIsUsed == false))
					{
						ConnectionPoolItem connectionPoolItem = _pool.First(item => item.ConnectionIsUsed == false);
						connectionPoolItem.ConnectionIsUsed = true;
						return connectionPoolItem;
					}
				}
				System.Threading.Thread.Sleep(10);
			}
		}

		public void ReleaseConnection(ConnectionPoolItem connectionItem)
		{
			lock (_pool)
			{
				try
				{
					ConnectionPoolItem connectionPoolItem = _pool.First(item => item == connectionItem);
					connectionPoolItem.ConnectionIsUsed = false;
				}
				catch (Exception e)
				{
					Console.WriteLine("Das Item:" + connectionItem.Number + " wurde nicht gefunden");
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
				_connectionPoolItem = ConnectionPool.Instance.GetConnection();
			}

			public void Dispose()
			{
				ConnectionPool.Instance.ReleaseConnection(_connectionPoolItem);
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

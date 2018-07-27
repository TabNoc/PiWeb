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
				_pool.Add(new ConnectionPoolItem(connection));
			}
		}

		public NpgsqlConnection GetConnection()
		{
			while (true)
			{
				lock (_pool)
				{
					if (_pool.Any(item => item.ConnectionIsUsed == false))
					{
						ConnectionPoolItem connectionPoolItem = _pool.First(item => item.ConnectionIsUsed == false);
						connectionPoolItem.ConnectionIsUsed = true;
						return connectionPoolItem.Connection;
					}
				}
				System.Threading.Thread.Sleep(10);
			}
		}

		public void ReleaseConnection(NpgsqlConnection connection)
		{
			lock (_pool)
			{
				ConnectionPoolItem connectionPoolItem = _pool.First(item => item.Connection == connection);
				connectionPoolItem.ConnectionIsUsed = false;
			}
		}

		public class ConnectionUsable : IDisposable
		{
			private readonly NpgsqlConnection _connection;

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

			public ConnectionUsable()
			{
				_connection = ConnectionPool.Instance.GetConnection();
			}

			public void Dispose()
			{
				ConnectionPool.Instance.ReleaseConnection(_connection);
			}
		}

		private class ConnectionPoolItem
		{
			public bool ConnectionIsUsed;
			private readonly NpgsqlConnection _connection;

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

			public ConnectionPoolItem(NpgsqlConnection connection)
			{
				_connection = connection;
				ConnectionIsUsed = false;
			}
		}
	}
}

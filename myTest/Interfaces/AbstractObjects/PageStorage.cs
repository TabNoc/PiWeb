using Newtonsoft.Json;
using System;
using TabNoc.Ooui.Storage.Channels;

namespace TabNoc.Ooui.Interfaces.AbstractObjects
{
	internal class PageStorage<T> : IDisposable where T : PageData
	{
		private static PageStorage<T> _instance;
		public static PageStorage<T> Instance => _instance ?? (_instance = new PageStorage<T>());

		private T _storageData;
		private bool _isDisposed = false;
		private string _loadedData;
		private bool _initialized = false;

		private Action<string> _saveDataCallback;
		private Func<string> _loadDataCallback;

		public T StorageData
		{
			get
			{
				if (_storageData == null)
				{
					Load();
					return _storageData;
				}

				return _storageData;
			}
		}


		public void Initialize(Func<string> loadDataCallback, Action<string> saveDataCallback)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(typeof(T).Name);
			}
			if (_initialized && ((loadDataCallback != null && loadDataCallback != _loadDataCallback) || (saveDataCallback != null && saveDataCallback != _saveDataCallback)))
			{
				throw new InvalidOperationException($"Bei einem widerholten aufrufen von {nameof(Initialize)} darf sich {nameof(loadDataCallback)} und {nameof(saveDataCallback)} nicht ändern!");
			}

			if (_initialized == false)
			{
				_loadDataCallback = loadDataCallback ?? throw new ArgumentNullException(nameof(loadDataCallback));

				_saveDataCallback = saveDataCallback ?? throw new ArgumentNullException(nameof(saveDataCallback));
			}

			_initialized = true;
		}

		private void Load()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(typeof(T).Name);
			}
			if (_loadDataCallback == null)
			{
				throw new NullReferenceException(nameof(Initialize) + " has to be called before Loading the the " + typeof(T).Name);
			}

			_loadedData = _loadDataCallback();
			_storageData = ReadData(_loadedData) ?? (T)typeof(T).GetMethod("CreateNew").Invoke(null, null);

			if (_storageData.Valid == false)
			{
				_storageData = (T)typeof(T).GetMethod("CreateNew").Invoke(null, null);
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("Die Eingelesenen Daten von " + typeof(T).Name + " waren ungültig.\r\nDie Standardwerte wurden geladen!");
				Console.ResetColor();
			}
		}

		public void Save()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(typeof(T).Name);
			}
			if (_saveDataCallback == null)
			{
				throw new NullReferenceException(nameof(Initialize) + " has to be called before Saving the the " + typeof(T).Name);
			}

			string writeData = WriteData(_storageData);
			if (writeData != _loadedData)
			{
				_saveDataCallback(writeData);
			}
		}

		private T ReadData(string loadData)
		{
			return loadData == "" ? null : JsonConvert.DeserializeObject<T>(loadData);
		}

		private string WriteData(T channelsData)
		{
			return JsonConvert.SerializeObject(channelsData);
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(typeof(T).Name);
			}
			Save();
			_instance = null;
			_storageData = null;
			_isDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}

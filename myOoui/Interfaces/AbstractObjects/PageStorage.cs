using Newtonsoft.Json;
using System;

namespace TabNoc.MyOoui.Interfaces.AbstractObjects
{
	public class PageStorage<T> : IDisposable where T : PageData
	{
		public bool ReadOnly = false;
		public bool WriteOnly = false;
		private static PageStorage<T> _instance;
		private bool _initialized = false;
		private bool _isDisposed = false;
		private Func<string> _loadDataCallback;
		private string _loadedData;
		private Action<string> _saveDataCallback;
		private T _storageData;
		public static PageStorage<T> Instance => _instance ?? (_instance = new PageStorage<T>());

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

		public void Dispose()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(typeof(T).Name);
			}
			if (!ReadOnly)
			{
				Save();
			}
			_instance = null;
			_storageData = null;
			_isDisposed = true;
			GC.SuppressFinalize(this);
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
				if (WriteOnly == true)
				{
					if (loadDataCallback != null)
					{
						throw new ArgumentException("You Cannot apply a " + nameof(loadDataCallback) + " to a writeonly PageStorage (" + typeof(T).Name + ")", nameof(loadDataCallback));
					}
				}
				else
				{
					_loadDataCallback = loadDataCallback ?? throw new ArgumentNullException(nameof(loadDataCallback));
				}

				if (ReadOnly == true)
				{
					if (saveDataCallback != null)
					{
						throw new ArgumentException("You Cannot apply a " + nameof(saveDataCallback) + " to a readonly PageStorage (" + typeof(T).Name + ")", nameof(saveDataCallback));
					}
				}
				else
				{
					_saveDataCallback = saveDataCallback ?? throw new ArgumentNullException(nameof(saveDataCallback));
				}
			}

			_initialized = true;
		}

		public void Save()
		{
			if (ReadOnly == true)
			{
				throw new InvalidOperationException(" this PageStorage is ReadOnly -> You can't save it.");
			}
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

		private void Load()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(typeof(T).Name);
			}
			if (_loadDataCallback == null && !WriteOnly)
			{
				throw new NullReferenceException(nameof(Initialize) + " has to be called before Loading the the " + typeof(T).Name);
			}

			_loadedData = WriteOnly ? "" : _loadDataCallback();
			_storageData = ReadData(_loadedData) ?? (T)typeof(T).GetMethod("CreateNew").Invoke(null, null);

			if (_storageData.Valid == false)
			{
				_storageData = (T)typeof(T).GetMethod("CreateNew").Invoke(null, null);
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("Die Eingelesenen Daten von " + typeof(T).Name + " waren ungültig.\r\nDie Standardwerte wurden geladen!");
				Console.ResetColor();
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
	}
}

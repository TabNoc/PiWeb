using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using TabNoc.MyOoui.Storage;
using TabNoc.MyOoui.UiComponents;
using TabNoc.PiWeb.DataTypes;
using TabNoc.PiWeb.Pages.WateringWeb.History;

namespace TabNoc.MyOoui.Interfaces.AbstractObjects
{
	public class PageStorage<T> : IDisposable where T : PageData
	{
		public bool ReadOnly = false;
		public bool WriteOnly = false;
		private static PageStorage<T> _instance;
		private TimeSpan _cacheTimeSpan;
		private bool _changed = false;
		private bool _initialized = false;
		private bool _isDisposed = false;
		private DateTime _lastLoadDateTime;
		private Func<string> _loadDataCallback;
		private string _loadedData;
		private Action<string> _saveDataCallback;
		private T _storageData;
		public static PageStorage<T> Instance => _instance ?? (_instance = new PageStorage<T>());

		public T StorageData
		{
			get
			{
				try
				{
					bool cacheTimeout = DateTime.Now - _lastLoadDateTime > _cacheTimeSpan;

					if (!ReadOnly && (_changed == false && cacheTimeout && _storageData != null))
					{
						// überprüfen ob der gecachet wert sich verändert hat. Wenn ja, dann darf dieser nicht verworfen werden
						if (GetWriteData(_storageData) != _loadedData)
						{
							_changed = true;
						}
					}

					if (_storageData == null || (cacheTimeout && _changed == false))
					{
						_storageData = null;
						Load();
						return _storageData;
					}

					return _storageData;
				}
				catch (Exception e)
				{
					Logging.Error("Beim Abrufen der Daten von " + this.GetType().Name + "<" + typeof(T).Name + "> ist ein Fehler aufgetreten!", e);
					throw;
				}
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

		public void Initialize(Func<string> loadDataCallback, Action<string> saveDataCallback, TimeSpan cacheTimeSpan)
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
			_cacheTimeSpan = cacheTimeSpan;
		}

		public void Initialize(string key, TimeSpan cacheTimeSpan)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(typeof(T).Name);
			}
			if (_initialized)
			{
				throw new InvalidOperationException($"Ein wieferholtes aufrufen von Initialize ist nicht zulässig!");
			}

			if (_initialized == false)
			{
				if (WriteOnly != true)
				{
					_loadDataCallback = () => LoadDataCaller(key);
				}

				if (ReadOnly != true)
				{
					_saveDataCallback = s => SaveDataCaller(s, key);
				}
			}

			_initialized = true;
			_cacheTimeSpan = cacheTimeSpan;
		}

		public void Save()
		{
			try
			{
				if (ReadOnly == true)
				{
					throw new InvalidOperationException($"This PageStorage<{typeof(T).Name}> is ReadOnly -> You can't save it.");
				}
				if (_isDisposed)
				{
					throw new ObjectDisposedException(typeof(T).Name);
				}
				if (_saveDataCallback == null)
				{
					throw new NullReferenceException($"{nameof(Initialize)} has to be called before Saving the PageStorage<{typeof(T).Name}>");
				}

				string writeData = GetWriteData(_storageData);
				if (writeData != _loadedData)
				{
					//TODO: Maybe Merge Server data?
					_saveDataCallback(writeData);
					_changed = false;
				}
			}
			catch (Exception e)
			{
				Logging.Error("Beim Speichern der Daten von " + this.GetType().Name + "<" + typeof(T).Name + "> ist ein kritischer Fehler aufgetreten!", e);
			}
		}

		private T FromReadData(string loadData)
		{
			return loadData == "" ? null : JsonConvert.DeserializeObject<T>(loadData);
		}

		private string GetWriteData(T channelsData)
		{
			return JsonConvert.SerializeObject(channelsData);
		}

		private void Load()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(typeof(T).Name);
			}
			if (_loadDataCallback == null && !WriteOnly)
			{
				throw new NullReferenceException($"{nameof(Initialize)} has to be called before Loading the PageStorage<{typeof(T).Name}>");
			}

			_loadedData = WriteOnly ? "" : _loadDataCallback();
			_storageData = FromReadData(_loadedData) ?? (T)typeof(T).GetMethod("CreateNew").Invoke(null, null);

			// Get the Data from this Machine to compare it later
			_loadedData = GetWriteData(_storageData);

			_lastLoadDateTime = DateTime.Now;

			if (_storageData.Valid == false)
			{
				_storageData = (T)typeof(T).GetMethod("CreateNew").Invoke(null, null);
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("Die Eingelesenen Daten von " + typeof(T).Name + " waren ungültig.\r\nDie Standardwerte wurden geladen!");
				Console.ResetColor();
			}
		}

		private string LoadDataCaller(string key)
		{
			if (typeof(T) != typeof(BackendData))
			{
				Dictionary<string, BackedProperties> backedPropertieses = PageStorage<BackendData>.Instance.StorageData.BackedPropertieses;
				if (!backedPropertieses.ContainsKey(key))
				{
					throw new ArgumentNullException(nameof(key), "Der Parameter existiert im Dictionary nicht");
				}
				if (backedPropertieses[key].RequestDataFromBackend == true)
				{
					return new HttpClient().GetAsync(backedPropertieses[key].DataSourcePath).EnsureResultSuccessStatusCode().Result.Content.ReadAsStringAsync().Result;
				}
			}

			if (File.Exists($"PageStorage({key}).json"))
			{
				FileInfo fileInfo = new FileInfo($"PageStorage({key}).json");
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					return streamReader.ReadToEnd();
				}
			}
			else
			{
				return "";
			}
		}

		private void SaveDataCaller(string data, string key)
		{
			if (typeof(T) != typeof(BackendData))
			{
				Dictionary<string, BackedProperties> backedPropertieses = PageStorage<BackendData>.Instance.StorageData.BackedPropertieses;
				if (backedPropertieses.ContainsKey(key) && backedPropertieses[key].RequestDataFromBackend == true)
				{
					HttpResponseMessage message = new HttpClient().PutAsync(backedPropertieses[key].DataSourcePath,
						new StringContent(data, Encoding.UTF8, "application/json")).EnsureResultSuccessStatusCode().Result;
					if (HttpStatusCode.NoContent != message.StatusCode)
					{
						throw new ApplicationException("Wrong Server Response Statuscode");
					}
					return;
				}
			}
			FileInfo fileInfo = new FileInfo($"PageStorage({key}).json");
			using (StreamWriter streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}
	}
}

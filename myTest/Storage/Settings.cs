using Newtonsoft.Json;
using System;

namespace TabNoc.Ooui.Storage
{
	internal class Settings : IDisposable
	{
		private static Settings _instance;
		public static Settings Instance => _instance ?? (_instance = new Settings());
		private SettingsData _settingsData;
		private bool _isDisposed = false;

		public SettingsData SettingsData
		{
			get
			{
				if (_settingsData == null)
				{
					Load();
					return _settingsData;
				}

				return _settingsData;
			}
		}

		public void Initialize(Func<string> loadDataCallback, Action<string> saveDataCallback)
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(nameof(Settings));
			}
			LoadDataCallback = loadDataCallback ?? throw new ArgumentNullException(nameof(loadDataCallback));

			SaveDataCallback = saveDataCallback ?? throw new ArgumentNullException(nameof(saveDataCallback));
		}

		public Action<string> SaveDataCallback;

		public Func<string> LoadDataCallback;

		public void Load()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(nameof(Settings));
			}
			if (LoadDataCallback == null)
			{
				throw new NullReferenceException(nameof(Initialize) + " has to be called before Loading the Settings");
			}

			string loadData = LoadDataCallback();
			_settingsData = ReadData(loadData) ?? SettingsData.CreateNew();
		}

		public void Save()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(nameof(Settings));
			}
			if (SaveDataCallback == null)
			{
				throw new NullReferenceException(nameof(Initialize) + " has to be called before Saving the Settings");
			}

			SaveDataCallback(WriteData(_settingsData));
		}

		private SettingsData ReadData(string loadData)
		{
			return loadData == "" ? null : JsonConvert.DeserializeObject<SettingsData>(loadData);
		}

		private string WriteData(SettingsData settingsData)
		{
			return JsonConvert.SerializeObject(settingsData);
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(nameof(Settings));
			}
			Save();
			_instance = null;
			_settingsData = null;
			_isDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}

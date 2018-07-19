using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.Storage.WateringWeb.Settings
{
	internal class SettingsData : PageData
	{
		public bool Enabled;
		public Dictionary<string, string> HumiditySensors;
		public string Location;
		public string LocationName;
		public int OverrideValue;
		public bool WeatherEnabled;

		#region BackendSettings

		// ReSharper disable InconsistentNaming

		public string Backend_HistoryPath;
		public bool Backend_HistoryEnabled;
		// ReSharper restore InconsistentNaming

		#endregion BackendSettings

		public new static SettingsData CreateNew() => new SettingsData
		{
			Enabled = true,
			OverrideValue = 100,
			WeatherEnabled = false,
			Valid = true,
			HumiditySensors = new Dictionary<string, string>() { },
			Backend_HistoryEnabled = false,
			Backend_HistoryPath = ""
		};
	}
}

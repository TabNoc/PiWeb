using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.Storage.Settings
{
	internal class SettingsData : PageData
	{
		public bool Enabled;
		public bool WeatherEnabled;
		public string Location;
		public string LocationName;
		public string OverrideValue;
		public Dictionary<string, string> HumiditySensors;

		public new static SettingsData CreateNew() => new SettingsData
		{
			Enabled = true,
			LocationName = "Biesdorf",
			OverrideValue = "100%",
			WeatherEnabled = false,
			Valid = true,
			HumiditySensors = new Dictionary<string, string>() { }
		};
	}
}

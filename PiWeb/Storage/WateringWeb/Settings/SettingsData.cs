using System.Collections.Generic;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.PiWeb.Storage.WateringWeb.Settings
{
	internal class SettingsData : PageData
	{
		public bool Enabled;
		public Dictionary<string, string> HumiditySensors;
		public string Location;
		public string LocationName;
		public int OverrideValue;
		public bool WeatherEnabled;

		public new static SettingsData CreateNew() => new SettingsData
		{
			Enabled = true,
			OverrideValue = 100,
			WeatherEnabled = false,
			Valid = true,
			HumiditySensors = new Dictionary<string, string>() { }
		};
	}
}

using System.Collections.Generic;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Settings
{
	public class SettingsData : PageData
	{
		public bool Enabled;
		public Dictionary<string, string> HumiditySensors;
		public string Location;
		public string LocationFriendlyName;
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

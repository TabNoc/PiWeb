using System.Collections.Generic;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Settings
{
	public class HumiditySensorData : PageData
	{
		public List<string> HumiditySensors;

		public new static HumiditySensorData CreateNew() => new HumiditySensorData()
		{
			Valid = true,
			HumiditySensors = new List<string>()
		};
	}
}

using System.Collections.Generic;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.PiWeb.Storage.WateringWeb.Settings
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

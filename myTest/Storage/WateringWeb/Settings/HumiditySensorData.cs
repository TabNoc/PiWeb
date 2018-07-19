using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.Storage.WateringWeb.Settings
{
	internal class HumiditySensorData : PageData
	{
		public List<string> HumiditySensors;

		public new static HumiditySensorData CreateNew() => new HumiditySensorData()
		{
			Valid = true,
			HumiditySensors = new List<string>()
		};
	}
}

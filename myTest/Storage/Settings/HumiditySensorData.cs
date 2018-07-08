using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.Storage.Settings
{
	internal class HumiditySensorData : PageData
	{
		public List<string> HumiditySensors;

		public new static HumiditySensorData CreateNew() => new HumiditySensorData()
		{
			Valid = true,
			//HumiditySensors = new List<string>() { "test", "1", "Baum",  "reeller Name"}
			HumiditySensors = new List<string>()
		};
	}
}

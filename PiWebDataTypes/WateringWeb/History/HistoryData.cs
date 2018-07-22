using System;
using System.Collections.Generic;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.PiWeb.Storage.WateringWeb.History
{
	public class HistoryData : PageData
	{
		public List<ManualHistoryElement> ManualHistoryElements;
		public List<AutomaticHistoryElement> AutomaticHistoryElements;
		public List<SystemHistoryElement> SystemHistoryElements;

		public new static HistoryData CreateNew() => new HistoryData
		{
			Valid = true,
			AutomaticHistoryElements = new List<AutomaticHistoryElement>()
			{
				new AutomaticHistoryElement(){TimeStamp = DateTime.Now, Message = "Auto Test", Status = "OK"},
				new AutomaticHistoryElement(){TimeStamp = DateTime.Now, Message = "Auto Test", Status = "Info"},
				new AutomaticHistoryElement(){TimeStamp = DateTime.Now, Message = "Auto Test", Status = "Warnung"},
				new AutomaticHistoryElement(){TimeStamp = DateTime.Now, Message = "Auto Test", Status = "Fehler"}
			},
			SystemHistoryElements = new List<SystemHistoryElement>()
			{
				new SystemHistoryElement(){TimeStamp = DateTime.Now, Message = "System Test", Status = "OK"},
				new SystemHistoryElement(){TimeStamp = DateTime.Now, Message = "System Test", Status = "Info"},
				new SystemHistoryElement(){TimeStamp = DateTime.Now, Message = "System Test", Status = "Warnung"},
				new SystemHistoryElement(){TimeStamp = DateTime.Now, Message = "System Test", Status = "Fehler"}
			},
			ManualHistoryElements = new List<ManualHistoryElement>()
			{
				new ManualHistoryElement(){TimeStamp = DateTime.Now, Message = "Manual Test", Status = "OK"},
				new ManualHistoryElement(){TimeStamp = DateTime.Now, Message = "Manual Test", Status = "Info"},
				new ManualHistoryElement(){TimeStamp = DateTime.Now, Message = "Manual Test", Status = "Warnung"},
				new ManualHistoryElement(){TimeStamp = DateTime.Now, Message = "Manual Test", Status = "Fehler"}
			}
		};
	}
}

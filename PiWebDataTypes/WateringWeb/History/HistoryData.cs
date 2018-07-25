using System;
using System.Collections.Generic;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.PiWeb.Storage.WateringWeb.History
{
	public class HistoryData : PageData
	{
		public List<HistoryElement> HistoryElements;

		public new static HistoryData CreateNew() => new HistoryData
		{
			Valid = true,
			HistoryElements = new List<HistoryElement>()
			{
				new HistoryElement(DateTime.Now, "Automatic", "OK", "Auto Test"),
				new HistoryElement(DateTime.Now, "Automatic", "Info", "Auto Test"),
				new HistoryElement(DateTime.Now, "Automatic", "Warnung", "Auto Test"),
				new HistoryElement(DateTime.Now, "Automatic", "Fehler", "Auto Test"),

				new HistoryElement(DateTime.Now, "System", "OK", "System Test"),
				new HistoryElement(DateTime.Now, "System", "Info", "System Test"),
				new HistoryElement(DateTime.Now, "System", "Warnung", "System Test"),
				new HistoryElement(DateTime.Now, "System", "Fehler", "System Test"),

				new HistoryElement(DateTime.Now, "Manual", "OK", "Manual Test"),
				new HistoryElement(DateTime.Now, "Manual", "Info", "Manual Test"),
				new HistoryElement(DateTime.Now, "Manual", "Warnung", "Manual Test"),
				new HistoryElement(DateTime.Now, "Manual", "Fehler", "Manual Test")
			}
		};

		public HistoryData()
		{
			HistoryElements = new List<HistoryElement>();
		}
	}
}

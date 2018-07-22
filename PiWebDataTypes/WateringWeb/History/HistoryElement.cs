using System;

namespace TabNoc.PiWeb.Storage.WateringWeb.History
{
	public abstract class HistoryElement
	{
		public DateTime TimeStamp;
		public string Status;
		public string Message;
	}
}

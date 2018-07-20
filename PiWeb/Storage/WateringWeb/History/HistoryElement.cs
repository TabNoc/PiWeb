using System;

namespace TabNoc.PiWeb.Storage.WateringWeb.History
{
	internal abstract class HistoryElement
	{
		public DateTime TimeStamp;
		public string Status;
		public string Message;
	}
}
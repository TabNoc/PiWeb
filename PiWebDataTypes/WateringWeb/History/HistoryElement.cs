using System;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.History
{
	public class HistoryElement
	{
		// msgtimestamp timestamp without time zone NOT NULL,
		public DateTime TimeStamp;

		// source text
		public string Source;

		// status text, -- Status: INFO, OK, WARN, ERROR, CRIT
		public string Status;

		// message text
		public string Message;

		public HistoryElement(DateTime timeStamp, string source, string status, string message)
		{
			TimeStamp = timeStamp;
			Source = source ?? throw new ArgumentNullException(nameof(source));
			Status = status ?? throw new ArgumentNullException(nameof(status));
			Message = message ?? throw new ArgumentNullException(nameof(message));
		}
	}
}

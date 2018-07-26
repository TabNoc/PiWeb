using System;

namespace TabNoc.MyOoui.UiComponents
{
	public class Logging
	{
		public static Action<string, Exception> ErrorAction;

		public static void Error(string message, Exception exception)
		{
			if (ErrorAction != null)
			{
				ErrorAction(message, exception);
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("{0}: {1}", message, exception);
				Console.ResetColor();
			}
		}

		public static Action<DateTime, string, string, string> LogAction;

		public static void WriteLog(string source, string status, string message, DateTime timestamp = default(DateTime))
		{
			if (timestamp == default(DateTime))
			{
				timestamp = DateTime.Now;
			}

			if (LogAction != null)
			{
				LogAction(timestamp, source, status, message);
			}
			else
			{
				Console.WriteLine("{0}-{1}>{2}:{3}", timestamp, source, status, message);
			}
		}
	}
}

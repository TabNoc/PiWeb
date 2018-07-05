using System;

namespace TabNoc.Ooui.Storage
{
	public class ChannelProgramData
	{
		public string Name;
		public string Description;
		public int Id;
		public bool Enabled;
		public bool EnableMasterChannel;
		public bool ActivateWeatherInfo;
		public TimeSpan StartTime;
		public TimeSpan Duration;

		public Weekdays ChoosenWeekdays;

		[Flags]
		public enum Weekdays
		{
			None = 0,
			Montag = 1,
			Dienstag = 2,
			Mittwoch = 4,
			Donnerstag = 8,
			Freitag = 16,
			Samstag = 32,
			Sonntag = 64
		}

		public static ChannelProgramData CreateNew(int id) => new ChannelProgramData()
		{
			Enabled = true,
			ChoosenWeekdays = Weekdays.Montag | Weekdays.Mittwoch | Weekdays.Freitag,
			Duration = TimeSpan.Zero,
			EnableMasterChannel = true,
			ActivateWeatherInfo = false,
			Id = id,
			StartTime = DateTime.Now.TimeOfDay,
			Name = id.ToString(),
			Description = ""
		};
	}
}

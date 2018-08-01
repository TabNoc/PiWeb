using System;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Channels
{
	public class ChannelProgramData
	{
		public bool ActivateWeatherInfo;
		public Weekdays ChoosenWeekdays;
		public string Description;
		public TimeSpan Duration;
		public bool Enabled;
		public bool EnableMasterChannel;
		public string Guid = System.Guid.NewGuid().ToString();
		public int Id;
		public string Name;
		public TimeSpan StartTime;

		public static ChannelProgramData CreateNew(int id) => new ChannelProgramData()
		{
			Enabled = true,
			ChoosenWeekdays = Weekdays.Montag | Weekdays.Mittwoch | Weekdays.Freitag,
			Duration = TimeSpan.Zero,
			EnableMasterChannel = true,
			ActivateWeatherInfo = false,
			Id = id,
			StartTime = TimeSpan.Zero,
			Name = id.ToString(),
			Description = ""
		};

		[Flags]
		public enum Weekdays
		{
			None = 1,
			Montag = 2,
			Dienstag = 4,
			Mittwoch = 8,
			Donnerstag = 16,
			Freitag = 32,
			Samstag = 64,
			Sonntag = 128
		}
	}
}

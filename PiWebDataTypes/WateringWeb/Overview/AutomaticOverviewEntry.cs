using System;
using TabNoc.PiWeb.DataTypes;
using TabNoc.PiWeb.Storage.WateringWeb.Channels;

namespace TabNoc.PiWeb.Storage.WateringWeb.Overview
{
	public class AutomaticOverviewEntry
	{
		[TableHeadingDefinition(0, "Kanal")]
		public string ChannelName;

		[TableHeadingDefinition(1, "Master")]
		public bool MasterEnabled;

		[TableHeadingDefinition(2, "Start")]
		public TimeSpan StartTime;

		[TableHeadingDefinition(3, "Ende")]
		public TimeSpan EndTime;

		[TableHeadingDefinition(4, "Wetter")]
		public bool WeatherEnabled;

		[TableHeadingDefinition(5, "Tage")]
		public ChannelProgramData.Weekdays ActiveWeekdays;

		[TableHeadingDefinition(6, "Aktiv")]
		public bool ChannelEnabled;

		public static AutomaticOverviewEntry CreateNew() => new AutomaticOverviewEntry
		{
			ChannelName = "N/A" + new Random().Next(),
			EndTime = new TimeSpan(0, 0, 0, new Random().Next()),
			MasterEnabled = false,
			StartTime = new TimeSpan(0, 0, 0, new Random().Next()),
			WeatherEnabled = false,
			ActiveWeekdays = ChannelProgramData.Weekdays.None,
			ChannelEnabled = false
		};
	}
}

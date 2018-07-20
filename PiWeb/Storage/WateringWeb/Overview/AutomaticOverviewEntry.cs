using System;
using TabNoc.MyOoui.UiComponents;
using TabNoc.PiWeb.Storage.WateringWeb.Channels;

namespace TabNoc.PiWeb.Storage.WateringWeb.Overview
{
	internal class AutomaticOverviewEntry
	{
		[Table.TableHeadingDefinition(0, "Kanal")]
		public string ChannelName;

		[Table.TableHeadingDefinition(1, "Master")]
		public bool MasterEnabled;

		[Table.TableHeadingDefinition(2, "Start")]
		public TimeSpan StartTime;

		[Table.TableHeadingDefinition(3, "Ende")]
		public TimeSpan EndTime;

		[Table.TableHeadingDefinition(4, "Wetter")]
		public bool WeatherEnabled;

		[Table.TableHeadingDefinition(5, "Tage")]
		public ChannelProgramData.Weekdays ActiveWeekdays;

		[Table.TableHeadingDefinition(6, "Aktiv")]
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

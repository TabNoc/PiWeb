using System;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Overview
{
	public class ManualOverviewEntry
	{
		[TableHeadingDefinition(0, "Reihenfolge")]
		public int ActivationPriority;

		[TableHeadingDefinition(1, "Name")]
		public string ActionName;

		[TableHeadingDefinition(2, "Kanal")]
		public string ChannelName;

		[TableHeadingDefinition(3, "Start")]
		public TimeSpan StartTime;

		[TableHeadingDefinition(4, "Ende")]
		public TimeSpan EndTime;

		[TableHeadingDefinition(5, "Master")]
		public bool MasterEnabled;

		public static ManualOverviewEntry CreateNew() => new ManualOverviewEntry
		{
			ActionName = "N/A",
			ActivationPriority = -1,
			ChannelName = "N/A",
			EndTime = TimeSpan.Zero,
			MasterEnabled = false,
			StartTime = TimeSpan.Zero
		};
	}
}

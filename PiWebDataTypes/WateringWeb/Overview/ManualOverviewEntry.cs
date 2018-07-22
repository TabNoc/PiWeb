using System;
using TabNoc.MyOoui.UiComponents;

namespace TabNoc.PiWeb.Storage.WateringWeb.Overview
{
	public class ManualOverviewEntry
	{
		[Table.TableHeadingDefinition(0, "Reihenfolge")]
		public int ActivationPriority;

		[Table.TableHeadingDefinition(1, "Name")]
		public string ActionName;

		[Table.TableHeadingDefinition(2, "Kanal")]
		public string ChannelName;

		[Table.TableHeadingDefinition(3, "Start")]
		public TimeSpan StartTime;

		[Table.TableHeadingDefinition(4, "Ende")]
		public TimeSpan EndTime;

		[Table.TableHeadingDefinition(5, "Master")]
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

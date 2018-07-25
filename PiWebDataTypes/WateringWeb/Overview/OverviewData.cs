using System.Collections.Generic;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Overview
{
	public class OverviewData : PageData
	{
		public List<AutomaticOverviewEntry> AutomaticOverviewEntries;
		public List<ManualOverviewEntry> ManualOverviewEntries;

		public new static OverviewData CreateNew() => new OverviewData
		{
			Valid = true,
			ManualOverviewEntries = new List<ManualOverviewEntry>() { ManualOverviewEntry.CreateNew() },
			AutomaticOverviewEntries = new List<AutomaticOverviewEntry>() { AutomaticOverviewEntry.CreateNew(), AutomaticOverviewEntry.CreateNew(), AutomaticOverviewEntry.CreateNew(), AutomaticOverviewEntry.CreateNew(), AutomaticOverviewEntry.CreateNew() }
		};
	}
}

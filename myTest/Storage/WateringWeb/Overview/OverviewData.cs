using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.Storage.WateringWeb.Overview
{
	internal class OverviewData : PageData
	{
		public List<AutomaticOverviewEntry> AutomaticOverviewEntries;
		public List<ManualOverviewEntry> ManualOverviewEntries;

		public new static OverviewData CreateNew() => new OverviewData
		{
			Valid = true,
			ManualOverviewEntries = new List<ManualOverviewEntry>(){ManualOverviewEntry.CreateNew()},
			AutomaticOverviewEntries = new List<AutomaticOverviewEntry>() { AutomaticOverviewEntry.CreateNew(), AutomaticOverviewEntry.CreateNew() }
		};
	}
}

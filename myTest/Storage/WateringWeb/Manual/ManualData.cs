using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Pages.WateringWeb.Overview;

namespace TabNoc.Ooui.Storage.WateringWeb.Manual
{
	internal class ManualData : PageData
	{
		public List<BatchEntry> BatchEntries;
		public List<JobEntry> JobEntries;
		public int BatchCounter;

		public new static ManualData CreateNew() => new ManualData
		{
			Valid = true,
			BatchEntries = new List<BatchEntry>(),
			JobEntries = new List<JobEntry>(),
			BatchCounter = 0
		};

		public int GetUniqueID()
		{
			return BatchCounter++;
		}
	}
}

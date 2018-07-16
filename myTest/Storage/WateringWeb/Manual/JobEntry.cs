using System.Collections.Generic;
using TabNoc.Ooui.Storage.WateringWeb.Manual;

namespace TabNoc.Ooui.Pages.WateringWeb.Overview
{
	internal class JobEntry
	{
		public List<BatchEntry> BatchEntries;
		public string Name;

		public JobEntry(string name, BatchEntry batch)
		{
			Name = name;
			BatchEntries = new List<BatchEntry>
			{
				batch
			};
		}
	}
}

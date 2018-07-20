using System.Collections.Generic;

namespace TabNoc.PiWeb.Storage.WateringWeb.Manual
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

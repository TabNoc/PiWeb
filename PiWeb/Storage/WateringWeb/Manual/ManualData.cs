using System.Collections.Generic;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.PiWeb.Storage.WateringWeb.Manual
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

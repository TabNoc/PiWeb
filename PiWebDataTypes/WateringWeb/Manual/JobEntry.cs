using System.Collections.Generic;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Manual
{
	public class JobEntry
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

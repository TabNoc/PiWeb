using System.Collections.Generic;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Manual
{
	public class ManualData : PageData
	{
		public List<BatchEntry> BatchEntries;
		public List<JobEntry> JobEntries;

		public new static ManualData CreateNew() => new ManualData
		{
			Valid = true,
			BatchEntries = new List<BatchEntry>(),
			JobEntries = new List<JobEntry>()
		};
	}
}

using System;

namespace TabNoc.Ooui.Storage.WateringWeb.Manual
{
	public class BatchEntry
	{
		public readonly bool ActivateMasterChannel;
		public readonly int ChannelId;
		public readonly TimeSpan Duration;
		public readonly int DurationOverride;
		public readonly int UniqueID;
		public string Name;

		public BatchEntry(string name, int channelId, TimeSpan duration, bool activateMasterChannel, int durationOverride, int uniqueID)
		{
			Name = name;
			ChannelId = channelId;
			Duration = duration;
			ActivateMasterChannel = activateMasterChannel;
			DurationOverride = durationOverride;
			UniqueID = uniqueID;
		}
	}
}

using System;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Manual
{
	public class BatchEntry
	{
		public readonly bool ActivateMasterChannel;
		public readonly int ChannelId;
		public readonly TimeSpan Duration;
		public readonly int DurationOverride;
		public string Name;

		public BatchEntry(string name, int channelId, TimeSpan duration, bool activateMasterChannel, int durationOverride)
		{
			Name = name;
			ChannelId = channelId;
			Duration = duration;
			ActivateMasterChannel = activateMasterChannel;
			DurationOverride = durationOverride;
		}

		public override string ToString()
		{
			return $"(ChNr.:{ChannelId}) -> {Duration} [Master:{ActivateMasterChannel}]";
		}
	}
}

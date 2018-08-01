using System.Collections.Generic;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Channels
{
	public class ChannelsData : PageData
	{
		public List<ChannelData> Channels;
		public ChannelData MasterChannel;

		public new static ChannelsData CreateNew() => new ChannelsData
		{
			Channels = new List<ChannelData>(),
			MasterChannel = ChannelData.CreateNew(0, true),
			Valid = true
		};
	}
}

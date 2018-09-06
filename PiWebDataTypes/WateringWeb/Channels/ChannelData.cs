using System;
using System.Collections.Generic;

namespace TabNoc.PiWeb.DataTypes.WateringWeb.Channels
{
	public class ChannelData
	{
		public int ChannelId;
		public string HumiditySensor;
		public bool HumiditySensorEnabled;
		public string Name;
		public List<ChannelProgramData> ProgramList;

		public static ChannelData CreateNew(int channelId, bool isMasterChannel = false) => new ChannelData
		{
			ProgramList = new List<ChannelProgramData>() { ChannelProgramData.CreateNew(1) },
			Name = isMasterChannel ? "Master" : "Kanal-" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
			ChannelId = isMasterChannel ? 0 : channelId < 1 ? throw new ArgumentOutOfRangeException(nameof(channelId)) : channelId,
			HumiditySensorEnabled = false,
			HumiditySensor = ""
		};
	}
}

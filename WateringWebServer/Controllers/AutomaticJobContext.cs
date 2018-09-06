using TabNoc.PiWeb.DataTypes.WateringWeb.Channels;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	public class AutomaticJobContext
	{
		public ChannelProgramData ChannelProgramData;
		public int ChannelId;
		public string HumiditySensor;
		public bool HumiditySensorEnabled;
		public string Name;

		public AutomaticJobContext()
		{
		}

		public AutomaticJobContext(ChannelProgramData channelProgramData, ChannelData channelData)
		{
			ChannelProgramData = channelProgramData;
			Name = channelData.Name;
			ChannelId = channelData.ChannelId;
			HumiditySensorEnabled = channelData.HumiditySensorEnabled;
			HumiditySensor = channelData.HumiditySensor;
		}
	}
}

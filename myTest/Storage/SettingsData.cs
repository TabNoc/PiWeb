using System.Collections.Generic;

namespace TabNoc.Ooui.Storage
{
	public class SettingsData
	{
		public bool Enabled;
		public List<ChannelData> Channels;
		public ChannelData MasterChannel;

		public static SettingsData CreateNew() => new SettingsData
		{
			Channels = new List<ChannelData>(),
			MasterChannel = ChannelData.CreateNew(true)
		};
	}
}

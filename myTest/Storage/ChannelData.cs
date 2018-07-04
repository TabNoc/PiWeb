using System;
using System.Collections.Generic;

namespace TabNoc.Ooui.Storage
{
	public class ChannelData
	{
		public List<ChannelProgramData> ProgramList;
		public string Name;

		public static ChannelData CreateNew(bool isMasterChannel = false) => new ChannelData
		{
			ProgramList = new List<ChannelProgramData>() { ChannelProgramData.CreateNew(1) },
			Name = isMasterChannel ? "Master": "Kanal-" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
		};
	}
}

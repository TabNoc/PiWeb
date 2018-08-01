using System;

namespace TabNoc.PiWeb.WateringWebServer.other
{
	internal interface IRelaisControl
	{
		bool Activate(int channelId, bool activateWithMasterChannel, string operatingMode, TimeSpan duration);

		bool Deactivate(int channelId);
	}
}

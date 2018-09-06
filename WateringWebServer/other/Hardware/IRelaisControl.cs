using System;

namespace TabNoc.PiWeb.WateringWebServer.other.Hardware
{
	internal interface IRelaisControl
	{
		bool Activate(int channelId, bool activateWithMasterChannel, string operatingMode, TimeSpan duration, out bool activateMasterChannel);

		bool Deactivate(int channelId, string operatingMode, out bool deactivateMasterChannel);
	}
}

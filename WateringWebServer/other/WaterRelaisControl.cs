using System;

namespace TabNoc.PiWeb.WateringWebServer.other
{
	internal class WaterRelaisControl : IRelaisControl
	{
		private static WaterRelaisControl _instance;
		public static WaterRelaisControl Instance => _instance ?? (_instance = new WaterRelaisControl());

		private WaterRelaisControl()
		{
			// TODO: implement lock, manual is not allowed to disable automatic entries
		}

		public bool Activate(int channelId, bool activateWithMasterChannel, string operatingMode, TimeSpan duration)
		{
			return true;
			//TODO: Implement
			throw new NotImplementedException();
		}

		public bool Deactivate(int channelId)
		{
			return true;
			//TODO: Implement
			throw new NotImplementedException();
		}
	}
}

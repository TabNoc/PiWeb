using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.PiWeb.WateringWebServer.other.Storage;

namespace TabNoc.PiWeb.WateringWebServer.other.Hardware
{
	internal class WaterRelaisControl : IRelaisControl
	{
		private static WaterRelaisControl _instance;
		public static WaterRelaisControl Instance => _instance ?? (_instance = new WaterRelaisControl());

		private WaterRelaisControl()
		{
			// TODO: implement lock, manual is not allowed to disable automatic entries
		}

		public bool Activate(int channelId, bool activateWithMasterChannel, string operatingMode, TimeSpan duration, out bool activateMasterChannel)
		{
			activateMasterChannel = activateWithMasterChannel;
			using (DatabaseObjectStorageEntryUsable<WaterRelaisControlData> dataWrapper = DatabaseObjectStorageEntryUsable<WaterRelaisControlData>.GetData(() => new WaterRelaisControlData()))
			{
				Dictionary<int, List<WaterRelaisControlData.WaterRelaisControlDataItem>> dataDictionary = dataWrapper.Data.Dictionary;
				if (dataDictionary.ContainsKey(channelId) == false)
				{
					dataDictionary.Add(channelId, new List<WaterRelaisControlData.WaterRelaisControlDataItem>());
				}

				dataDictionary[channelId].Add(new WaterRelaisControlData.WaterRelaisControlDataItem(activateWithMasterChannel, operatingMode, duration, DateTime.Now.TimeOfDay));

				List<WaterRelaisControlData.WaterRelaisControlDataItem> currentlyActiveActions = GetCurrentlyActiveActions(dataDictionary[channelId]);
				if (currentlyActiveActions.Count > 0)
				{
					activateMasterChannel = currentlyActiveActions.Any(item => item.ActivateWithMasterChannel);
				}
			}
			return true;
		}

		public bool Deactivate(int channelId, string operatingMode, out bool deactivateMasterChannel)
		{
			deactivateMasterChannel = false;
			using (DatabaseObjectStorageEntryUsable<WaterRelaisControlData> dataWrapper = DatabaseObjectStorageEntryUsable<WaterRelaisControlData>.GetData(() => new WaterRelaisControlData()))
			{
				List<WaterRelaisControlData.WaterRelaisControlDataItem> waterRelaisControlDataItems = dataWrapper.Data.Dictionary[channelId];

				#region Cleanup

				// remove entries wich are more than 5 minutes over their endTime
				waterRelaisControlDataItems.RemoveAll(item => item.EndTime.Add(new TimeSpan(0, 5, 0)) > DateTime.Now.TimeOfDay);

				#endregion Cleanup

				List<WaterRelaisControlData.WaterRelaisControlDataItem> currentlyActiveActions = GetCurrentlyActiveActions(waterRelaisControlDataItems);
				if (currentlyActiveActions.Count > 0)
				{
					if (currentlyActiveActions.All(item => item.ActivateWithMasterChannel == false))
					{
						deactivateMasterChannel = true;
					}
					return false;
				}
			}
			return true;
			//TODO: Implement
			throw new NotImplementedException();
		}

		// Ignore Items where the Endtime is only 10s in the future
		private static List<WaterRelaisControlData.WaterRelaisControlDataItem> GetCurrentlyActiveActions(
			List<WaterRelaisControlData.WaterRelaisControlDataItem> waterRelaisControlDataItems) =>
			waterRelaisControlDataItems
				.Where(item =>
					item.EndTime.Subtract(TimeSpan.FromSeconds(10)) > DateTime.Now.TimeOfDay
					).ToList();

		// ReSharper disable FieldCanBeMadeReadOnly.Local
		private class WaterRelaisControlData
		{
			public Dictionary<int, List<WaterRelaisControlDataItem>> Dictionary = new Dictionary<int, List<WaterRelaisControlDataItem>>();

			internal class WaterRelaisControlDataItem
			{
				public bool ActivateWithMasterChannel;
				public TimeSpan Duration;
				public string OperatingMode;
				public TimeSpan StartTime;
				public TimeSpan EndTime => Duration + StartTime;

				public WaterRelaisControlDataItem(bool activateWithMasterChannel, string operatingMode, TimeSpan duration, TimeSpan startTime)
				{
					ActivateWithMasterChannel = activateWithMasterChannel;
					OperatingMode = operatingMode;
					Duration = duration;
					StartTime = startTime;
				}
			}
		}

		// ReSharper restore FieldCanBeMadeReadOnly.Local
	}
}

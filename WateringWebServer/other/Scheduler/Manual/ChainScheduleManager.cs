using Hangfire;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TabNoc.PiWeb.DataTypes.WateringWeb.History;
using TabNoc.PiWeb.WateringWebServer.Controllers;
using TabNoc.PiWeb.WateringWebServer.other.Storage;

namespace TabNoc.PiWeb.WateringWebServer.other.Scheduler.Manual
{
	public partial class ChainScheduleManager<T> where T : ChainScheduleManager<T>.ChainedActionExecution
	{
		public readonly List<ChainedExecutionData> Jobs = new List<ChainedExecutionData>();

		public string CurrentJob = "";

		public static void AddEntry(T manualActionExecution, string elementEventSource)
		{
			using (DatabaseObjectStorageEntryUsable<ChainScheduleManager<T>> dataInstanceWrapper = GetInstance())
			{
				ChainScheduleManager<T> dataInstance = dataInstanceWrapper.Data;
				if (dataInstance.CurrentJob == "" && dataInstance.Jobs.Count > 0)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, "System", "Warn", $"ChainScheduleManager: Es ist kein aktuell ausgeführter Job spezifiziert, jedoch sind noch {dataInstance.Jobs.Count} Jobs in der Queue. Diese werden nun vom ältesten beginnend abgearbeitet. {JsonConvert.SerializeObject(dataInstance.Jobs.First())}"));
					Task.Run(() => ActivateJob(dataInstance.Jobs.First().Guid));
				}
				if (dataInstance.Jobs.Count == 0 || dataInstance.Jobs.All(data => data.NextGuid != ""))
				{
					ChainedExecutionData newChainedExecutionData = new ChainedExecutionData(manualActionExecution, elementEventSource, "");
					dataInstance.Jobs.Add(newChainedExecutionData);
					Task.Run(() => ActivateJob(newChainedExecutionData.Guid));
				}
				else
				{
					ChainedExecutionData previousChainedExecutionData = dataInstance.Jobs.Last(data => data.NextGuid == "");
					previousChainedExecutionData.NextGuid = manualActionExecution.Guid;
					dataInstance.Jobs.Add(new ChainedExecutionData(manualActionExecution, elementEventSource, previousChainedExecutionData.Guid));
				}
			}
		}

		public static void DeactivateJob(string guid)
		{
			ChainedExecutionData currentChainedExecutionData;

			using (DatabaseObjectStorageEntryUsable<ChainScheduleManager<T>> dataInstanceWrapper = GetInstance())
			{
				ChainScheduleManager<T> dataInstance = dataInstanceWrapper.Data;
				currentChainedExecutionData = dataInstance.Jobs.FirstOrDefault(data => data.Guid == guid);
				if (currentChainedExecutionData == null)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, "Manual", "Error", $"Es wurde kein Job mit der GUID: {guid} zum deaktivieren gefunden!\r\nDieser Eintrag wird nun übersprungen\r\nEs sind noch {dataInstance.Jobs.Count} Aufträge in der Queue.\r\nAktueller Auftrag ist: {dataInstance.CurrentJob}"));
					return;
				}

				dataInstance.CurrentJob = "";

				CallNextJob(currentChainedExecutionData, dataInstanceWrapper);

				currentChainedExecutionData.ChainedActionExecutionData.DeactivateAction();
				dataInstance.Jobs.RemoveAll(data => data.Guid == currentChainedExecutionData.ChainedActionExecutionData.Guid);
			}
		}

		public static void DeleteEntry(ChainScheduleManager<ManualChainedActionExecution>.ChainedExecutionData loadedDataJob)
		{
			//TODO: Implement Delete Manual Chain Schedul Manager Entry
			throw new NotImplementedException();
		}

		private static void ActivateJob(string guid)
		{
			ChainedExecutionData currentChainedExecutionData;
			using (DatabaseObjectStorageEntryUsable<ChainScheduleManager<T>> dataInstanceWrapper = GetInstance())
			{
				ChainScheduleManager<T> dataInstance = dataInstanceWrapper.Data;
				currentChainedExecutionData = dataInstance.Jobs.FirstOrDefault(data => data.Guid == guid);
				if (currentChainedExecutionData == null)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, "Manual", "Error", $"Es wurde kein Job mit der GUID: {guid} zum aktivieren gefunden!\r\nDieser Eintrag wird nun übersprungen\r\nEs sind noch {dataInstance.Jobs.Count} Aufträge in der Queue.\r\nAktueller Auftrag ist: {dataInstance.CurrentJob}"));
					return;
				}

				dataInstance.CurrentJob = guid;

				TimeSpan duration = (currentChainedExecutionData.ChainedActionExecutionData.Duration * currentChainedExecutionData.ChainedActionExecutionData.DurationOverride / 100 * GetGlobalOverride());

				currentChainedExecutionData.StartTime = DateTime.Now.TimeOfDay;
				currentChainedExecutionData.ChainedActionExecutionData.ActivateAction(duration);

				BackgroundJob.Schedule(() => DeactivateJob(currentChainedExecutionData.Guid), duration);
				currentChainedExecutionData.Duration = duration;
			}
		}

		private static void CallNextJob(ChainedExecutionData currentChainedExecutionData, DatabaseObjectStorageEntryUsable<ChainScheduleManager<T>> dataInstanceWrapper)
		{
			ChainScheduleManager<T> dataInstance = dataInstanceWrapper.Data;
			ChainedExecutionData nextChainedExecutionData = dataInstance.Jobs.FirstOrDefault(data => data.Guid == currentChainedExecutionData.NextGuid);
			if (nextChainedExecutionData != null)
			{
				Task.Run(() => ActivateJob(nextChainedExecutionData.Guid));
			}
		}

		private static double GetGlobalOverride()
		{
			using (ConnectionPool.ConnectionUsable usable = new ConnectionPool.ConnectionUsable())
			{
				using (NpgsqlCommand command = usable.Connection.CreateCommand())
				{
					command.CommandText = "select override_value from t_settings;";
					using (NpgsqlDataReader dataReader = command.ExecuteReader())
					{
						if (dataReader.Read() == false)
						{
							throw new Exception("Es existieren keine SettingsDaten!");
						}

						return (double)dataReader.GetInt32(0) / 100;
					}
				}
			}
		}

		private static DatabaseObjectStorageEntryUsable<ChainScheduleManager<T>> GetInstance() =>
			DatabaseObjectStorageEntryUsable<ChainScheduleManager<T>>.GetDataLocked(() =>
				new ChainScheduleManager<T>());
	}
}

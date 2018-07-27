using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Npgsql;
using TabNoc.PiWeb.DataTypes.WateringWeb.History;
using TabNoc.PiWeb.DataTypes.WateringWeb.Manual;
using TabNoc.PiWeb.WateringWebServer.Controllers;

namespace TabNoc.PiWeb.WateringWebServer.other
{
	public class ManualScheduleManager
	{
		public class ExecutionData
		{
			public readonly string Guid;
			public readonly ManualActionExecutionData.ManualActionExecution ManualActionExecution;
			public readonly string ElementEventSource;
			public string NextGuid = "";
			public string PreviousGuid;

			public ExecutionData(ManualActionExecutionData.ManualActionExecution manualActionExecution, string elementEventSource, string previousGuid)
			{
				Guid = manualActionExecution.Guid;
				ManualActionExecution = manualActionExecution;
				ElementEventSource = elementEventSource;
				PreviousGuid = previousGuid;
			}
		}

		private static readonly List<ExecutionData> Jobs = new List<ExecutionData>();
		private static string _currentJob = "";

		public static void AddEntry(ManualActionExecutionData.ManualActionExecution manualActionExecution, string elementEventSource)
		{
			lock (Jobs)
			{
				if (Jobs.Count == 0 || Jobs.Any(data => data.NextGuid == "") == false)
				{
					ExecutionData newExecutionData = new ExecutionData(manualActionExecution, elementEventSource, "");
					Jobs.Add(newExecutionData);
					Task.Run(() => ActivateJob(newExecutionData.Guid));
				}
				else
				{
					ExecutionData previousExecutionData = Jobs.Last(data => data.NextGuid == "");
					previousExecutionData.NextGuid = manualActionExecution.Guid;
					Jobs.Add(new ExecutionData(manualActionExecution, elementEventSource, previousExecutionData.Guid));
				}

				SaveToDataBase();
			}
		}

		private static void SaveToDataBase()
		{
			return;
			throw new NotImplementedException();
		}

		public static void ActivateJob(string guid)
		{
			ExecutionData currentExecutionData;
			lock (Jobs)
			{
				currentExecutionData = Jobs.FirstOrDefault(data => data.Guid == guid);
				if (currentExecutionData == null)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, "Manual", "Error", $"Es wurde kein Job mit der GUID: {guid} zum aktivieren gefunden!\r\nDieser Eintrag wird nun übersprungen\r\nEs sind noch {Jobs.Count} Aufträge in der Queue.\r\nAKtueller Auftrag ist: {_currentJob}"));
					return;
				}
			}
			lock (_currentJob)
			{
				_currentJob = guid;
			}

			RelaisControl.Activate(currentExecutionData.ManualActionExecution.ChannelId, currentExecutionData.ManualActionExecution.ActivateMasterChannel);
			BackgroundJob.Schedule(() => DeactivateJob(currentExecutionData.Guid), currentExecutionData.ManualActionExecution.Duration * currentExecutionData.ManualActionExecution.DurationOverride / 100 * GetGlobalOverride());
		}

		public static double GetGlobalOverride()
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

		public static void DeactivateJob(string guid)
		{
			ExecutionData currentExecutionData;
			lock (Jobs)
			{
				currentExecutionData = Jobs.FirstOrDefault(data => data.Guid == guid);
				if (currentExecutionData == null)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, "Manual", "Error", $"Es wurde kein Job mit der GUID: {guid} zum deaktivieren gefunden!\r\nDieser Eintrag wird nun übersprungen\r\nEs sind noch {Jobs.Count} Aufträge in der Queue.\r\nAKtueller Auftrag ist: {_currentJob}"));
					return;
				}
			}
			lock (_currentJob)
			{
				_currentJob = "";
			}

			CallNextJob(currentExecutionData);

			RelaisControl.Deactivate(currentExecutionData.ManualActionExecution.ChannelId);

			lock (Jobs)
			{
				Jobs.RemoveAll(data => data.Guid == currentExecutionData.ManualActionExecution.Guid);
			}
		}

		private static void CallNextJob(ExecutionData currentExecutionData)
		{
			lock (Jobs)
			{
				ExecutionData nextExecutionData = Jobs.FirstOrDefault(data => data.Guid == currentExecutionData.NextGuid);
				if (nextExecutionData != null)
				{
					ActivateJob(nextExecutionData.Guid);
				}
			}
		}
	}
}
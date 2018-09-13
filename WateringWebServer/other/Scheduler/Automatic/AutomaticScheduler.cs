using Hangfire;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.PiWeb.DataTypes.WateringWeb.Channels;
using TabNoc.PiWeb.WateringWebServer.other.Hardware;
using TabNoc.PiWeb.WateringWebServer.other.Storage;

namespace TabNoc.PiWeb.WateringWebServer.other.Scheduler.Automatic
{
	public class AutomaticScheduler
	{
		public static void ExecuteScheduledJob(AutomaticJobContext automaticJobContext)
		{
			if (GetGlobalScheduleEnabled())
			{
				TimeSpan duration = automaticJobContext.ChannelProgramData.Duration
									* GetGlobalOverride()
									* GetWeatherMultiplicator(automaticJobContext.ChannelProgramData.ActivateWeatherInfo)
									* GetHumidityMultiplicator(automaticJobContext.HumiditySensorEnabled, automaticJobContext.HumiditySensor);
				RelaisControl.Activate(automaticJobContext.ChannelId, automaticJobContext.ChannelProgramData.EnableMasterChannel, "Automatic", duration);
				BackgroundJob.Schedule(() => RelaisControl.Deactivate(automaticJobContext.ChannelId, "Automatic"), duration);
			}
		}

		public static void Setup(ChannelsData channelsData)
		{
			string enqueue = BackgroundJob.Enqueue(() => SetupChannel(channelsData.MasterChannel));
			foreach (ChannelData channelData in channelsData.Channels)
			{
				enqueue = BackgroundJob.ContinueWith(enqueue, () => SetupChannel(channelData));
			}
		}

		public static void SetupChannel(ChannelData channelData)
		{
			List<string> removedGuids = new List<string>();
			using (DatabaseObjectStorageEntryUsable<AutomaticJobStore> automaticJobStoreWrapper = DatabaseObjectStorageEntryUsable<AutomaticJobStore>.GetData(() => new AutomaticJobStore()))
			{
				AutomaticJobStore automaticJobStore = automaticJobStoreWrapper.Data;
				if (automaticJobStore.ChannelProgramDictionary.ContainsKey(channelData.ChannelId) == false)
				{
					automaticJobStore.ChannelProgramDictionary.Add(channelData.ChannelId, new List<string>());
				}

				// alle einträge von automaticJobStore.ChannelProgramDictionary[channelData.ChannelId] wo kein !gültiger! Eintrag in channelData.ProgramList enthalten ist
				foreach (string removedGuid in automaticJobStore.ChannelProgramDictionary[channelData.ChannelId].Where(s => channelData.ProgramList.Any(data => data.Guid == s && IsInvalid(data) == false) == false))
				{
					RecurringJob.RemoveIfExists(removedGuid);
					removedGuids.Add(removedGuid);
				}
				foreach (string removedGuid in removedGuids)
				{
					automaticJobStore.ChannelProgramDictionary[channelData.ChannelId].Remove(removedGuid);
				}

				foreach (ChannelProgramData channelProgramData in channelData.ProgramList.Where(data => IsInvalid(data) == false))
				{
					AutomaticJobContext automaticJobContext = new AutomaticJobContext(channelProgramData, channelData);
					RecurringJob.AddOrUpdate(automaticJobContext.ChannelProgramData.Guid, () => ExecuteScheduledJob(automaticJobContext), GetExecutionCron(automaticJobContext), TimeZoneInfo.Local);
					if (automaticJobStore.ChannelProgramDictionary[channelData.ChannelId].Contains(automaticJobContext.ChannelProgramData.Guid) == false)
					{
						// new Entry was added
						automaticJobStore.ChannelProgramDictionary[channelData.ChannelId].Add(automaticJobContext.ChannelProgramData.Guid);
					}
				}
			}
		}

		private static string GetExecutionCron(AutomaticJobContext automaticJobContext)
		{
			string weekdays = "";
			IEnumerable<ChannelProgramData.Weekdays> weekdayses = Enum
				.GetValues(automaticJobContext.ChannelProgramData.ChoosenWeekdays.GetType()).Cast<Enum>()
				.Where(automaticJobContext.ChannelProgramData.ChoosenWeekdays.HasFlag)
				.Cast<ChannelProgramData.Weekdays>();
			foreach (ChannelProgramData.Weekdays weekday in weekdayses)
			{
				switch (weekday)
				{
					case ChannelProgramData.Weekdays.None:
						throw new IndexOutOfRangeException("ChannelProgramData.Weekdays.None ist kein verwendbarer Wochentag!");

					case ChannelProgramData.Weekdays.Montag:
						weekdays += ",MON";
						break;

					case ChannelProgramData.Weekdays.Dienstag:
						weekdays += ",TUE";
						break;

					case ChannelProgramData.Weekdays.Mittwoch:
						weekdays += ",WED";
						break;

					case ChannelProgramData.Weekdays.Donnerstag:
						weekdays += ",THU";
						break;

					case ChannelProgramData.Weekdays.Freitag:
						weekdays += ",FRI";
						break;

					case ChannelProgramData.Weekdays.Samstag:
						weekdays += ",SAT";
						break;

					case ChannelProgramData.Weekdays.Sonntag:
						weekdays += ",SUN";
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			weekdays = weekdays.TrimStart(',');
			return $"{automaticJobContext.ChannelProgramData.StartTime.Minutes} {automaticJobContext.ChannelProgramData.StartTime.Hours} * * {weekdays}";
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

		private static bool GetGlobalScheduleEnabled()
		{
			using (ConnectionPool.ConnectionUsable usable = new ConnectionPool.ConnectionUsable())
			{
				using (NpgsqlCommand command = usable.Connection.CreateCommand())
				{
					command.CommandText = "select enabled from t_settings;";
					using (NpgsqlDataReader dataReader = command.ExecuteReader())
					{
						if (dataReader.Read() == false)
						{
							throw new Exception("Es existieren keine SettingsDaten!");
						}

						return dataReader.GetBoolean(0);
					}
				}
			}
		}

		private static double GetHumidityMultiplicator(bool humiditySensorEnabled, string humiditySensor)
		{
			return 1;
			//TODO: Implement
		}

		private static double GetWeatherMultiplicator(bool activateWeatherInfo)
		{
			return 1;
			//TODO: Implement
		}

		private static bool IsInvalid(ChannelProgramData data)
		{
			if (ChannelProgramData.Weekdays.None == (ChannelProgramData.Weekdays.None & data.ChoosenWeekdays))
			{
				return true;
			}

			if (data.Duration == TimeSpan.Zero)
			{
				return true;
			}

			return false;
		}

		internal class AutomaticJobStore
		{
			public readonly Dictionary<int, List<string>> ChannelProgramDictionary;

			public AutomaticJobStore()
			{
				ChannelProgramDictionary = new Dictionary<int, List<string>>();
			}
		}
	}
}

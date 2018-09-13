using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TabNoc.PiWeb.DataTypes.WateringWeb.Channels;
using TabNoc.PiWeb.DataTypes.WateringWeb.Overview;
using TabNoc.PiWeb.WateringWebServer.other.Scheduler.Manual;
using TabNoc.PiWeb.WateringWebServer.other.Storage;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OverviewController : ControllerBase
	{
		[HttpDelete("deleteManualEntry")]
		public ActionResult DeleteManualEntry(int number)
		{
			ChainScheduleManager<ManualChainedActionExecution>.DeleteEntry(DataBaseObjectStorage.LoadFromDataBase(() => new ChainScheduleManager<ManualChainedActionExecution>()).Jobs[number]);

			return Ok();
		}

		[HttpGet]
		public ActionResult<OverviewData> Get()
		{
			OverviewData data = new OverviewData
			{
				Valid = true,
				AutomaticOverviewEntries = GetAutomaticEntries(),
				ManualOverviewEntries = GetManualEnties()
			};
			return Ok(data);
		}

		[HttpGet("enabled")]
		public ActionResult<bool> GetEnabled()
		{
			return Ok(true);
		}

		private List<AutomaticOverviewEntry> GetAutomaticEntries()
		{
			List<AutomaticOverviewEntry> returnval = new List<AutomaticOverviewEntry>();
			ChannelsData loadFromDataBase = DataBaseObjectStorage.LoadFromDataBase(ChannelsData.CreateNew);
			foreach (ChannelData channel in loadFromDataBase.Channels)
			{
				returnval.AddRange(GetAutomaticEntries(channel, channel));
			}

			return returnval;
		}

		private IEnumerable<AutomaticOverviewEntry> GetAutomaticEntries(ChannelData channel, ChannelData parentChannel)
		{
			List<AutomaticOverviewEntry> returnval = new List<AutomaticOverviewEntry>();

			foreach (ChannelProgramData programData in channel.ProgramList)
			{
				returnval.Add(new AutomaticOverviewEntry()
				{
					ActiveWeekdays = programData.ChoosenWeekdays,
					ChannelEnabled = programData.Enabled,
					ChannelName = parentChannel.Name + " : " + programData.Name,
					EndTime = programData.StartTime + (programData.Duration/*TODO: multiply by multiplicator from AutomaticScheduler*/),
					MasterEnabled = programData.EnableMasterChannel,
					StartTime = programData.StartTime,
					WeatherEnabled = programData.ActivateWeatherInfo
				});
			}

			return returnval;
		}

		private string GetChannelName(int channelId)
		{
			//TODO: implement ChannelName
			return channelId.ToString();
		}

		private List<ManualOverviewEntry> GetManualEnties()
		{
			List<ManualOverviewEntry> returnval = new List<ManualOverviewEntry>();
			int count = 0;

			//TODO: Group by Name, create Duration if it doesn't exists set startetime to entime from last execution
			ChainScheduleManager<ManualChainedActionExecution> loadedData = DataBaseObjectStorage.LoadFromDataBase(() => new ChainScheduleManager<ManualChainedActionExecution>());
			foreach (ChainScheduleManager<ManualChainedActionExecution>.ChainedExecutionData chainedExecutionData in loadedData.Jobs)
			{
				returnval.Add(new ManualOverviewEntry()
				{
					StartTime = chainedExecutionData.StartTime,
					ActionName = chainedExecutionData.ElementEventSource,
					ActivationPriority = count++,
					ChannelName = GetChannelName(chainedExecutionData.ChainedActionExecutionData.ManualActionExecution.ChannelId),
					EndTime = chainedExecutionData.StartTime + chainedExecutionData.Duration,
					MasterEnabled = chainedExecutionData.ChainedActionExecutionData.ManualActionExecution.ActivateMasterChannel
				});
			}

			return returnval;
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using TabNoc.PiWeb.DataTypes.WateringWeb.Channels;
using TabNoc.PiWeb.WateringWebServer.other;
using TabNoc.PiWeb.WateringWebServer.other.Scheduler;
using TabNoc.PiWeb.WateringWebServer.other.Storage;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChannelsController : ControllerBase
	{
		public ChannelsController()
		{
		}

		[HttpGet]
		public ActionResult<ChannelsData> Get() => Ok(DataBaseObjectStorage.LoadFromDataBase<ChannelsData>(ChannelsData.CreateNew));

		[HttpGet("enabled")]
		public ActionResult<bool> GetEnabled()
		{
			return Ok(true);
		}

		[HttpPut]
		public ActionResult Put([FromBody] ChannelsData channelsDatalDataData)
		{
			DataBaseObjectStorage.SaveToDataBase(channelsDatalDataData);
			AutomaticScheduler.Setup(channelsDatalDataData);
			return NoContent();
		}
	}
}

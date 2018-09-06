using Microsoft.AspNetCore.Mvc;
using TabNoc.PiWeb.DataTypes.WateringWeb.Manual;
using TabNoc.PiWeb.DataTypes.WateringWeb.Settings;
using TabNoc.PiWeb.WateringWebServer.other;
using TabNoc.PiWeb.WateringWebServer.other.Storage;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ManualController : ControllerBase
	{
		public ManualController()
		{
		}

		[HttpGet]
		public ActionResult<ManualData> Get() => Ok(DataBaseObjectStorage.LoadFromDataBase<ManualData>(ManualData.CreateNew));

		[HttpGet("enabled")]
		public ActionResult<bool> GetEnabled()
		{
			return Ok(true);
		}

		[HttpPut]
		public ActionResult Put([FromBody] ManualData manualDataData)
		{
			DataBaseObjectStorage.SaveToDataBase(manualDataData);
			return NoContent();
		}
	}
}

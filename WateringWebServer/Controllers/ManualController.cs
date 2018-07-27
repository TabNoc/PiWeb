using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TabNoc.PiWeb.DataTypes.WateringWeb.Manual;
using TabNoc.PiWeb.WateringWebServer.other;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ManualController : ControllerBase
	{
		public ManualController()
		{
		}

		[HttpPost]
		public ActionResult CreateNewEntries(ManualActionExecutionData element)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			foreach (ManualActionExecutionData.ManualActionExecution manualActionExecution in element.ExecutionList)
			{
				ManualScheduleManager.AddEntry(manualActionExecution, element.EventSource);
			}

			return Ok();
		}

		[HttpPost("entry")]
		public ActionResult CreateNewEntry(ManualActionExecutionData.ManualActionExecution element, [FromQuery(Name = "eventSource")] string eventSource)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			ManualScheduleManager.AddEntry(element, eventSource);

			return Ok();
		}

		[HttpGet("enabled")]
		public ActionResult<bool> GetEnabled()
		{
			return Ok(true);
		}

		private string GetCollumnName(int collumn)
		{
			switch (collumn)
			{
				case 0:
					return "msgtimestamp";

				case 1:
					return "source";

				case 2:
					return "status";

				case 3:
					return "message";

				default:
					throw new IndexOutOfRangeException();
			}
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using System;
using TabNoc.PiWeb.DataTypes.WateringWeb.Manual;
using TabNoc.PiWeb.WateringWebServer.other.Scheduler;

namespace TabNoc.PiWeb.WateringWebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ManualActionExecutionController : ControllerBase
	{
		public ManualActionExecutionController()
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
				ChainScheduleManager<ManualChainedActionExecution>.AddEntry(new ManualChainedActionExecution(manualActionExecution), element.Name);
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
			ChainScheduleManager<ManualChainedActionExecution>.AddEntry(new ManualChainedActionExecution(element), eventSource);

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

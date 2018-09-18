using Hangfire.Dashboard;

namespace TabNoc.PiWeb.WateringWebServer
{
	public class MyAuthorizationFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize(DashboardContext context)
		{
			return context.Request.RemoteIpAddress == "::ffff:192.168.1.137" || context.Request.RemoteIpAddress == "::1";
		}
	}
}
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace TabNoc.PiWeb.WateringWebServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>().ConfigureLogging(logBuilder =>
				{
					logBuilder.AddFilter((provider, category, logLevel) =>
					{
						if (provider == "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor[1]" || provider == "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[1]" || provider == "Microsoft.AspNetCore.Mvc.StatusCodeResult[1]")
						{
							return false;
						}
						return true;
					});
				});
	}
}

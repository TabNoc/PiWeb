using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TabNoc.PiWeb.WateringWebServer
{
	public class Program
	{
		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();

		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args)
				.UseKestrel()
				.UseUrls("http://*:5000/")
				.Build().Run();
		}
	}
}

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using TabNoc.PiWeb.WateringWebServer.other.Hardware;
using TabNoc.PiWeb.WateringWebServer.TempManager;

namespace TabNoc.PiWeb.WateringWebServer
{
	public class Program
	{
		//ds1820.py -> Steckdose (1 = Innen; 2 = Aussen[Anzeige Only])
		//bmp280.py -> Pi Intern
		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();

		public static void Main(string[] args)
		{
			Console.WriteLine(new string('#', 30));
			Console.WriteLine("Program.Main");
			Console.WriteLine(new string('#', 30));

			WaterRelaisControl.DeactivateAll("Startup");
			TemperatureManager manager = new TemperatureManager();

			CreateWebHostBuilder(args)
				.UseKestrel()
				.UseUrls("http://*:5000/")
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
				})
				.Build().Run();

			manager.Dispose();

			Console.WriteLine(new string('+', 30));
			Console.WriteLine("Program.Main finished! Bye");
			Console.WriteLine(new string('-', 30));
		}
	}
}

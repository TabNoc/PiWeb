using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using TabNoc.PiWeb.WateringWebServer.other.Binder;

namespace TabNoc.PiWeb.WateringWebServer
{
	public class Startup
	{
		private readonly List<DateTime> _test = new List<DateTime>();
		private bool _isDevelopment;

		public IConfiguration Configuration
		{
			get;
		}

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void Cleanup()
		{
			Task.Run(() =>
			{
				DateTime dateTime = DateTime.Now;
				Console.Write(dateTime.ToString(CultureInfo.InvariantCulture) + "Cleanup caller");

				lock (_test)
				{
					_test.Add(dateTime);
				}

				System.Threading.Thread.Sleep(2000);
				lock (_test)
				{
					if (_test.Count > 1)
					{
						Console.WriteLine(new string('*', 30));
						Console.WriteLine(new string('*', 30));
						Console.WriteLine("Startup.Cleanup found issue!");
						Console.WriteLine(new string('*', 30));
						Console.WriteLine(new string('*', 30));
					}

					_test.Remove(dateTime);
				}
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			Console.WriteLine(new string('#', 30));
			Console.WriteLine("Startup.Configure");
			Console.WriteLine(new string('#', 30));
			_isDevelopment = env.IsDevelopment();
			if (_isDevelopment)
			{
				//app.UseDeveloperExceptionPage();
			}
			//app.UseDeveloperExceptionPage();
			app.UseMvc();
			loggerFactory.AddConsole((s, level) =>
			{
				if (s == "Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor" || s == "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker" || s == "Microsoft.AspNetCore.Mvc.StatusCodeResult")
				{
					return false;
				}
				if (level > LogLevel.Debug)
				{
					Console.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture));
					return true;
				}
				return false;
			});
#if !DebugWithoutHangfire
			app.UseHangfireServer(new BackgroundJobServerOptions()
			//{
			//	SchedulePollingInterval = TimeSpan.FromMilliseconds(6000)
			//}
			);
			app.UseHangfireDashboard(options: new DashboardOptions() { AppPath = $"http://{PrivateData.RemoteHostName}:8080/", Authorization = new IDashboardAuthorizationFilter[] { new MyAuthorizationFilter() } });

			//RecurringJob.AddOrUpdate(() => GC.Collect(), Cron.Minutely);
			RecurringJob.RemoveIfExists("GC.Collect");
			//RecurringJob.AddOrUpdate("pg_dump", () => PG_Dump(), Cron.Yearly);
			//BackgroundJob.Enqueue(() => PG_Dump());
			RecurringJob.RemoveIfExists("pg_dump");

			RecurringJob.AddOrUpdate("Cleanup", () => Cleanup(), Cron.Hourly);
#endif

			Console.WriteLine(new string('v', 30));
			Console.WriteLine("Startup.Configure done");
			Console.WriteLine(new string('v', 30));
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			Console.WriteLine(new string('#', 30));
			Console.WriteLine("Startup.ConfigureServices");
			Console.WriteLine(new string('#', 30));
			services
				.AddMvc(options =>
				{
					if (_isDevelopment)
					{
						options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
					}
				})
				//.AddMvc()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
#if !DebugWithoutHangfire
			services.AddHangfire(config =>
			{
				config.UsePostgreSqlStorage(PrivateData.ConnectionStringBuilder.ToString(), new PostgreSqlStorageOptions() { ConnectionsCount = 2 });
			});
#endif

			Console.WriteLine(new string('v', 30));
			Console.WriteLine("Startup.ConfigureServices done");
			Console.WriteLine(new string('v', 30));
		}

		public void PG_Dump()
		{
			Console.WriteLine("\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\nStart pg_dump execution!\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n");
			Process p = new Process
			{
				StartInfo = new ProcessStartInfo("/usr/bin/pg_dump", "--host localhost --port 5433 --username \"postgres\" --no-password  --format custom --blobs --verbose \"piweb\"")
				{
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				}
			};
			p.OutputDataReceived += (sender, args) =>
			{
				lock (p)
				{
					Console.WriteLine("received output: {0}", args.Data);
				}
			};
			p.ErrorDataReceived += (sender, args) =>
			{
				lock (p)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("received error: {0}", args.Data);
					Console.ResetColor();
				}
			};
			p.Start();
			p.BeginOutputReadLine();
			p.BeginErrorReadLine();
		}
	}
}

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
using System.Diagnostics;
using System.Globalization;
using TabNoc.PiWeb.WateringWebServer.other;

namespace TabNoc.PiWeb.WateringWebServer
{
	public class Startup
	{
		private bool _isDevelopment;

		public IConfiguration Configuration
		{
			get;
		}

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
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
			app.UseHangfireServer(new BackgroundJobServerOptions { SchedulePollingInterval = TimeSpan.FromMilliseconds(2000) });
			app.UseHangfireDashboard(options: new DashboardOptions() { AppPath = "http://piw:8080/", Authorization = new IDashboardAuthorizationFilter[1] { new MyAuthorizationFilter() } });

			RecurringJob.AddOrUpdate(() => GC.Collect(), Cron.Minutely);
			//RecurringJob.AddOrUpdate("pg_dump", () => PG_Dump(), Cron.Yearly);
			//BackgroundJob.Enqueue(() => PG_Dump());
			RecurringJob.RemoveIfExists("pg_dump");
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
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

			services.AddHangfire(config =>
			{
				config.UsePostgreSqlStorage(PrivateData.ConnectionStringBuilder.ToString());
			});
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

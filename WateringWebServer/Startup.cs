using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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

			app.UseHangfireServer();
			app.UseHangfireDashboard();

			RecurringJob.AddOrUpdate(() => GC.Collect(), Cron.Minutely);
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
	}
}

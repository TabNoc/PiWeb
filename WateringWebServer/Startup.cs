using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;

namespace TabNoc.PiWeb.WateringWebServer
{
	public class DateTimeModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
				throw new ArgumentNullException(nameof(bindingContext));

			// Try to fetch the value of the argument by name
			var modelName = bindingContext.ModelName;
			var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
			if (valueProviderResult == ValueProviderResult.None)
				return Task.CompletedTask;

			bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

			var dateStr = valueProviderResult.FirstValue;
			// Here you define your custom parsing logic, i.e. using "de-DE" culture
			if (!DateTime.TryParse(dateStr, new CultureInfo("de-DE"), DateTimeStyles.None, out DateTime date))
			{
				bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "DateTime should be in format 'dd.MM.yyyy HH:mm:ss'");
				return Task.CompletedTask;
			}

			bindingContext.Result = ModelBindingResult.Success(date);
			return Task.CompletedTask;
		}
	}

	public class DateTimeModelBinderProvider : IModelBinderProvider
	{
		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (context.Metadata.ModelType == typeof(DateTime) ||
				context.Metadata.ModelType == typeof(DateTime?))
			{
				return new DateTimeModelBinder();
			}

			return null;
		}
	}

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
			//app.UseHangfireDashboard();

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

			NpgsqlConnection connection = new NpgsqlConnection(PrivateData.ConnectionStringBuilder.ToString());
			connection.Open();
			services.AddSingleton<NpgsqlConnection>(connection);

			services.AddHangfire(config =>
			{
				config.UsePostgreSqlStorage(PrivateData.ConnectionStringBuilder.ToString());
			});
		}
	}
}

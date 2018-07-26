using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace TabNoc.PiWeb.WateringWebServer
{
	public class Startup
	{
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
			if (env.IsDevelopment())
			{
				//app.UseDeveloperExceptionPage();
			}
			//app.UseDeveloperExceptionPage();
			// loggerFactory.AddConsole(LogLevel.Critical);
			app.UseMvc();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc(options =>
				options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider())
				).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			NpgsqlConnection connection = new NpgsqlConnection(PrivateData.ConnectionStringBuilder.ToString());
			connection.Open();
			services.AddSingleton<NpgsqlConnection>(connection);
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
}

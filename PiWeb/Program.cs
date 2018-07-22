using Ooui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Storage;
using TabNoc.PiWeb.PagePublisher;
using TabNoc.PiWeb.PagePublisher.WateringWeb;
using TabNoc.PiWeb.Storage.WateringWeb.Channels;
using TabNoc.PiWeb.Storage.WateringWeb.Settings;

namespace TabNoc.PiWeb
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			UI.HeadHtml = "<script src=\"https://code.jquery.com/jquery-3.3.1.min.js\" crossorigin=\"anonymous\"></script>";
			UI.HeadHtml += "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js\" integrity=\"sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49\" crossorigin=\"anonymous\"></script>";
			UI.HeadHtml += "<script src=\"https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js\" integrity=\"sha384-smHYKdLADwkXOn1EmN1qk/HfnUcbVRZyYmZ4qpPea6sjB/pTJ0euyQp0Mk8ck+5T\" crossorigin=\"anonymous\"></script>";
			UI.HeadHtml += "<link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css\" integrity=\"sha384-WskhaSGFgHYWDcbwN70/dfYBj47jz9qbsMId/iRN3ewGhXQFZCSftd1LZCfmhktB\" crossorigin=\"anonymous\">";

			UI.HeadHtml += "<link rel=\"stylesheet\" href=\"https://use.fontawesome.com/releases/v5.1.0/css/all.css\" integrity=\"sha384-lKuwvrZot6UHsBSfcMvOkWwlCMgc0TaWr+30HWe3a4ltaBwTZhyTEggF5tJv8tbt\" crossorigin=\"anonymous\">";

			BackendData.Setup(new Dictionary<string, BackedProperties>()
			{
				{ "Humidity", new BackedProperties("", false) },
				{ "Settings", new BackedProperties("", false) },
				{ "Manual", new BackedProperties("", false) },
				{ "Overview", new BackedProperties("", false) },
				{ "ManualActionExecution", new BackedProperties("", false) },
				{ "History", new BackedProperties("", false) },
				{ "Channels", new BackedProperties("", false) }
			});

			#region WateringWeb

			new SettingsPagePublisher("/settings").Publish();
			new ChannelsPagePublisher("/channels").Publish();
			new OverviewPagePublisher("/overview").Publish();
			new ManualPagePublisher("/manual").Publish();
			new HistoryPagePublisher("/history").Publish();

			UI.PublishJson("/settings/WeatherLocations.json", () =>
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				//Assembly.GetExecutingAssembly().GetManifestResourceNames();
				string resourceName = "TabNoc.PiWeb.Storage.WateringWeb.Settings.external_WeatherLocations.WeatherLocations.json";

				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				using (StreamReader reader = new StreamReader(stream, Encoding.Default))
				{
					return reader.ReadToEnd();
				}
			});
			UI.PublishJson("/lib/bootstrap3-typeahead.min.js", () =>
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				//Assembly.GetExecutingAssembly().GetManifestResourceNames();
				string resourceName = "TabNoc.PiWeb.Storage.lib.bootstrap3-typeahead.min.js";

				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				using (StreamReader reader = new StreamReader(stream, Encoding.Default))
				{
					return reader.ReadToEnd();
				}
			});

			#endregion WateringWeb

			#region PiWeb

			new PiWebPublisher("/").Publish();

			#endregion PiWeb

			Console.ReadLine();
			PageStorage<ChannelsData>.Instance.Dispose();
			PageStorage<SettingsData>.Instance.Dispose();
		}
	}
}

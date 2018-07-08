using Ooui;
using System;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.PagePublisher;
using TabNoc.Ooui.PagePublisher.WateringWeb;
using TabNoc.Ooui.Storage.Channels;
using TabNoc.Ooui.Storage.Settings;

namespace TabNoc.Ooui
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			UI.HeadHtml = "<script src=\"https://code.jquery.com/jquery-3.3.1.slim.min.js\" integrity=\"sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo\" crossorigin=\"anonymous\"></script>";
			UI.HeadHtml += "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js\" integrity=\"sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49\" crossorigin=\"anonymous\"></script>";
			UI.HeadHtml += "<script src=\"https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/js/bootstrap.min.js\" integrity=\"sha384-smHYKdLADwkXOn1EmN1qk/HfnUcbVRZyYmZ4qpPea6sjB/pTJ0euyQp0Mk8ck+5T\" crossorigin=\"anonymous\"></script>";
			UI.HeadHtml += "<link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.1.1/css/bootstrap.min.css\" integrity=\"sha384-WskhaSGFgHYWDcbwN70/dfYBj47jz9qbsMId/iRN3ewGhXQFZCSftd1LZCfmhktB\" crossorigin=\"anonymous\">";

			#region WateringWeb

			new ChannelsPagePublisher("/channels").Publish();
			new OverviewPagePublisher("/overview").Publish();
			new ManualPagePublisher("/manual").Publish();
			new SettingsPagePublisher("/settings").Publish();
			new HistoryPagePublisher("/history").Publish();

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

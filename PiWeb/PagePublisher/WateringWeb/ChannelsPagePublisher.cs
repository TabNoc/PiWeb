using Ooui;
using System;
using System.Net.Http;
using System.Text;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.PiWeb.DataTypes.WateringWeb.Channels;
using TabNoc.PiWeb.DataTypes.WateringWeb.History;
using TabNoc.PiWeb.Pages.WateringWeb.Channels;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class ChannelsPagePublisher : WateringPublisher
	{
		public ChannelsPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<ChannelsData>.Instance.Initialize("Channels", new TimeSpan(0, 0, 5));
		}

		protected override Element CreatePage()
		{
			ChannelsPage channelsPage = new ChannelsPage(PageStorage<ChannelsData>.Instance);
			channelsPage.AddStyling(StylingOption.MarginRight, 5);
			channelsPage.AddStyling(StylingOption.MarginLeft, 1);
			channelsPage.ClassName += " col-xl-10";
			return channelsPage;
		}

		protected override void Initialize()
		{
			Console.WriteLine("Initialize" + this.GetType().Name);
			new HttpClient().PostAsync("http://localhost:5000/api/history",
				new StringContent(JsonConvert.SerializeObject(
					new HistoryElement(DateTime.Now, "Test", "Info", "Initialized " + this.GetType().Name)), Encoding.UTF8, "application/json"));
		}
	}
}

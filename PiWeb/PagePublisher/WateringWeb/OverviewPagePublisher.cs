using Ooui;
using System;
using System.Net.Http;
using System.Text;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.PiWeb.Pages.WateringWeb.Overview;
using TabNoc.PiWeb.Storage.WateringWeb.History;
using TabNoc.PiWeb.Storage.WateringWeb.Overview;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class OverviewPagePublisher : WateringPublisher
	{
		public OverviewPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<OverviewData>.Instance.ReadOnly = true;
			PageStorage<OverviewData>.Instance.Initialize("Overview", new TimeSpan(0, 0, 5));
		}

		protected override Element CreatePage() => new OverviewPage();

		protected override void Initialize()
		{
			Console.WriteLine("Initialize" + this.GetType().Name);
			new HttpClient().PostAsync("http://localhost:5000/api/history",
				new StringContent(JsonConvert.SerializeObject(
					new HistoryElement(DateTime.Now, "Test", "Info", "Initialized " + this.GetType().Name)), Encoding.UTF8, "application/json"));
		}
	}
}

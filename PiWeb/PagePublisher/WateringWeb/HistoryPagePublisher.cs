using Ooui;
using System;
using System.Net.Http;
using System.Text;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.PiWeb.Pages.WateringWeb.History;
using TabNoc.PiWeb.Storage.WateringWeb.History;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class HistoryPagePublisher : WateringPublisher
	{
		public HistoryPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<HistoryData>.Instance.ReadOnly = true;
			PageStorage<HistoryData>.Instance.Initialize("History", new TimeSpan(0, 0, 5));
		}

		protected override Element CreatePage()
		{
			return new HistoryPage();
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

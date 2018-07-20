using Ooui;

using System.Net.Http;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Pages.WateringWeb.Overview;
using TabNoc.Ooui.Storage.WateringWeb.History;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class HistoryPagePublisher : WateringPublisher
	{
		public HistoryPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<HistoryData>.Instance.ReadOnly = true;
			PageStorage<HistoryData>.Instance.Initialize(LoadDataCallback, null);
		}

		private string LoadDataCallback()
		{
			if (PageStorage<SettingsData>.Instance.StorageData.Backend_HistoryEnabled)
			{
				HttpClient httpClient = new HttpClient();
				return httpClient.GetStringAsync(PageStorage<SettingsData>.Instance.StorageData.Backend_HistoryPath).Result;
			}
			else
			{
				return "";
			}
		}

		protected override void Initialize()
		{
		}

		protected override Element CreatePage()
		{
			return new HistoryPage();
		}
	}
}
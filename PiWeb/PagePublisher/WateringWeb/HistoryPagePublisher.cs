using Ooui;
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
			PageStorage<HistoryData>.Instance.Initialize(LoadDataCallback, null);
		}

		private string LoadDataCallback()
		{
			//TODO: Implement
			return "";
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

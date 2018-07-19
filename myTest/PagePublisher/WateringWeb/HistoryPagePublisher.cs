using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Pages.WateringWeb.Overview;
using TabNoc.Ooui.Storage.WateringWeb.History;

namespace TabNoc.Ooui.PagePublisher.WateringWeb
{
	internal class HistoryPagePublisher : WateringPublisher
	{
		public HistoryPagePublisher (string publishPath) : base (publishPath)
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

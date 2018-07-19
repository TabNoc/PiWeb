using Ooui;
using System.IO;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages.WateringWeb.Overview;
using TabNoc.Ooui.Storage.WateringWeb.Overview;
using TabNoc.Ooui.Storage.WateringWeb.Settings;

namespace TabNoc.Ooui.PagePublisher.WateringWeb
{
	internal class OverviewPagePublisher : WateringPublisher
	{
		public OverviewPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<OverviewData>.Instance.ReadOnly = true;
			PageStorage<OverviewData>.Instance.Initialize(LoadDataCallback, null);
		}

		protected override void Initialize()
		{
	}

		protected override Element CreatePage() => new OverviewPage();

		

		private string LoadDataCallback()
		{
			if (File.Exists("demo_Overview.json"))
			{
				FileInfo fileInfo = new FileInfo("demo_Overview.json");
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					return streamReader.ReadToEnd();
				}
			}
			else
			{
				return "";
			}
		}
	}
}

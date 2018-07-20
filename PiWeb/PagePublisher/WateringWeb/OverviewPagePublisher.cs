using Ooui;
using System.IO;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.PiWeb.Pages.WateringWeb.Overview;
using TabNoc.PiWeb.Storage.WateringWeb.Overview;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
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

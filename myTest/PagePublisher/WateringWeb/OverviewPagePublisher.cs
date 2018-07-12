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
			Initialize();
		}

		protected override void Initialize()
		{
			PageStorage<OverviewData>.Instance.Initialize(LoadDataCallback, SaveDataCallback);
		}

		protected override Element CreatePage()
		{
			OverviewPage overviewPage = new OverviewPage();
			overviewPage.AddStyling(StylingOption.MarginRight, 5);
			overviewPage.AddStyling(StylingOption.MarginLeft, 1);
			overviewPage.ClassName += " col-xl-10";
			return overviewPage;
		}

		private void SaveDataCallback(string data)
		{
			FileInfo fileInfo = new FileInfo("demo_Overview.json");
			using (StreamWriter streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}

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

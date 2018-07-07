using Ooui;
using System.IO;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages.Settings;

namespace TabNoc.Ooui
{
	internal class SettingsPagePublisher : WateringPublisher
	{
		public SettingsPagePublisher(string publishPath) : base(publishPath)
		{
		}

		protected override void Initialize()
		{
			Storage.Settings.Instance.Initialize(LoadDataCallback, SaveDataCallback);
		}

		protected override Element CreatePage()
		{
			SettingsPage settingsPage = new SettingsPage(Storage.Settings.Instance);
			settingsPage.AddStyling(StylingOption.MarginRight, 5);
			settingsPage.ClassName += " col-xl-10";
			return settingsPage;
		}

		private void SaveDataCallback(string data)
		{
			FileInfo fileInfo = new FileInfo("demo.json");
			using (StreamWriter streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write(data);
				streamWriter.Flush();
			}
		}

		private string LoadDataCallback()
		{
			if (File.Exists("demo.json"))
			{
				FileInfo fileInfo = new FileInfo("demo.json");
				return fileInfo.OpenText().ReadToEnd();
			}
			else
			{
				return "";
			}
		}
	}
}

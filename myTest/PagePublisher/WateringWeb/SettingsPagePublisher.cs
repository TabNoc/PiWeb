using Ooui;
using System.IO;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages.WateringWeb.Settings;
using TabNoc.Ooui.Storage.Settings;

namespace TabNoc.Ooui.PagePublisher.WateringWeb
{
	internal class SettingsPagePublisher : WateringPublisher
	{
		public SettingsPagePublisher(string publishPath) : base(publishPath)
		{
		}

		protected override void Initialize()
		{
			PageStorage<SettingsData>.Instance.Initialize(LoadDataCallback_Settings, SaveDataCallback_Settings);
			PageStorage<HumiditySensorData>.Instance.Initialize(LoadDataCallback_Humidity, SaveDataCallback_Humidity);
		}

		protected override Element CreatePage()
		{
			SettingsPage settingsPage = new SettingsPage(PageStorage<SettingsData>.Instance);
			settingsPage.AddStyling(StylingOption.MarginRight, 5);
			settingsPage.AddStyling(StylingOption.MarginLeft, 1);
			settingsPage.ClassName += " col-xl-10";
			return settingsPage;
		}

		private void SaveDataCallback_Settings(string data)
		{
			FileInfo fileInfo = new FileInfo($"demo_Settings.json");
			using (StreamWriter streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}

		private string LoadDataCallback_Settings()
		{
			if (File.Exists($"demo_Settings.json"))
			{
				FileInfo fileInfo = new FileInfo($"demo_Settings.json");
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

		private void SaveDataCallback_Humidity(string data)
		{
			FileInfo fileInfo = new FileInfo($"demo_Humidity.json");
			using (StreamWriter streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}

		private string LoadDataCallback_Humidity()
		{
			if (File.Exists($"demo_Humidity.json"))
			{
				FileInfo fileInfo = new FileInfo($"demo_Humidity.json");
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

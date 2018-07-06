using Ooui;
using System;
using System.IO;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages;
using TabNoc.Ooui.UiComponents;

namespace TabNoc.Ooui
{
	internal class SettingsPublisher : Publishable
	{
		public SettingsPublisher(string publishPath) : base(publishPath)
		{
		}

		protected override Element PopulateAppElement()
		{
			Storage.Settings.Instance.Initialize(LoadDataCallback, SaveDataCallback);

			Grid grid = new Grid();

			NavigationBar navBar = new NavigationBar("Demo", "/settings");
			navBar.AddElement(true, "Settings", "/settings");
			navBar.AddElement(false, "test", "/test");
			navBar.AddElement(false, "tabTest", "/tabTest");
			navBar.AddElement(false, "buttonTest", "/buttonTest");
			grid.AddRow().AppendCollum(navBar);

			SettingsPage settingsPage = new SettingsPage(Storage.Settings.Instance);
			settingsPage.AddStyling(StylingOption.MarginRight, 5);
			settingsPage.ClassName += " col-xl-10";
			grid.AddRow().AppendCollum(settingsPage);
			Console.WriteLine(grid.OuterHtml.Length);
			return grid;
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

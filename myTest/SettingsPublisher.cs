using System;
using Ooui;
using System.IO;
using TabNoc.Ooui.Interfaces;
using TabNoc.Ooui.Interfaces.AbstractObjects;
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
			grid.AddRow().AppendCollum(new SettingsPage(TabNoc.Ooui.Storage.Settings.Instance), 8);
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

using Ooui;
using System.IO;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Pages.WateringWeb.Overview;
using TabNoc.Ooui.Storage.WateringWeb.Manual;

namespace TabNoc.Ooui.PagePublisher.WateringWeb
{
	internal class ManualPagePublisher : WateringPublisher
	{
		public ManualPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<ManualActionExecutionData>.Instance.WriteOnly = true;
			PageStorage<ManualActionExecutionData>.Instance.Initialize(null, SaveManualActionExecutionDataCallback);
			PageStorage<ManualData>.Instance.Initialize(LoadDataCallback, SaveDataCallback);
		}

		protected override Element CreatePage()
		{
			return new ManualPage();
		}

		protected override void Initialize()
		{
		}

		private string LoadDataCallback()
		{
			if (File.Exists("demo_Manual.json"))
			{
				FileInfo fileInfo = new FileInfo("demo_Manual.json");
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

		private void SaveDataCallback(string data)
		{
			FileInfo fileInfo = new FileInfo("demo_Manual.json");
			using (StreamWriter streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}

		private void SaveManualActionExecutionDataCallback(string data)
		{
			FileInfo fileInfo = new FileInfo("demo_ManualActionExecution.json");
			using (StreamWriter streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}
	}
}

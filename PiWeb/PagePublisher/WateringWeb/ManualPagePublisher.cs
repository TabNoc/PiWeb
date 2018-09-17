using Ooui;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using TabNoc.MyOoui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Storage;
using TabNoc.PiWeb.DataTypes.WateringWeb.Manual;
using TabNoc.PiWeb.Pages.WateringWeb.Manual;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class ManualPagePublisher : WateringPublisher
	{
		public ManualPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<ManualActionExecutionData>.Instance.WriteOnly = true;
			PageStorage<ManualActionExecutionData>.Instance.Initialize(null, SaveManualActionExecutionDataCallback, new TimeSpan(0, 0, 5));
			PageStorage<ManualData>.Instance.Initialize("Manual", new TimeSpan(0, 0, 5));
		}

		protected override Element CreatePage()
		{
			return new ManualPage();
		}

		protected override void Initialize()
		{
		}

		private void SaveManualActionExecutionDataCallback(string data)
		{
			const string key = "ManualActionExecution";
			string url = PageStorage<BackendData>.Instance.StorageData.GetUrl(key);
			if (url == "")
			{
				new HttpClient().PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")).EnsureResultSuccessStatusCode();
			}
			else
			{
				FileInfo fileInfo = new FileInfo($"demo_{key}.json");
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
}

using Ooui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Storage;
using TabNoc.PiWeb.Pages.WateringWeb.Manual;
using TabNoc.PiWeb.Storage.WateringWeb.Manual;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class ManualPagePublisher : WateringPublisher
	{
		public ManualPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<ManualActionExecutionData>.Instance.WriteOnly = true;
			PageStorage<ManualActionExecutionData>.Instance.Initialize(null, SaveManualActionExecutionDataCallback, new TimeSpan(0, 0, 5));
			// TODO implement manualExecutionData Server Comunication
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
			Dictionary<string, BackedProperties> backedPropertieses = PageStorage<BackendData>.Instance.StorageData.BackedPropertieses;
			if (backedPropertieses.ContainsKey(key) && backedPropertieses[key].SendDataToBackend == true)
			{
				HttpClient httpClient = new HttpClient();
				StringContent httpContent = new StringContent(data, Encoding.UTF8, "application/json");
				HttpResponseMessage message = httpClient.PostAsync(backedPropertieses[key].DataSourcePath, httpContent).Result;
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

using Ooui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Storage;
using TabNoc.PiWeb.DataTypes.WateringWeb.History;
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
			// TODO implement manualExecutionData Server Comunication
			PageStorage<ManualData>.Instance.Initialize("Manual", new TimeSpan(0, 0, 5));
		}

		protected override Element CreatePage()
		{
			return new ManualPage();
		}

		protected override void Initialize()
		{
			Console.WriteLine("Initialize" + this.GetType().Name);
			new HttpClient().PostAsync("http://localhost:5000/api/history",
				new StringContent(JsonConvert.SerializeObject(
					new HistoryElement(DateTime.Now, "Test", "Info", "Initialized " + this.GetType().Name)), Encoding.UTF8, "application/json"));
		}

		private void SaveManualActionExecutionDataCallback(string data)
		{//TODO: implement
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

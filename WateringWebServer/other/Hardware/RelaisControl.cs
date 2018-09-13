using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TabNoc.MyOoui;
using TabNoc.PiWeb.DataTypes.WateringWeb.History;
using TabNoc.PiWeb.WateringWebServer.Controllers;

namespace TabNoc.PiWeb.WateringWebServer.other.Hardware
{
	internal static class RelaisControl
	{
		//TODO: move http query into method, change to https, move uri to static context
		public static void Activate(int channelId, bool activateWithMasterChannel, string operatingMode, TimeSpan duration)
		{
			if (WaterRelaisControl.Instance.Activate(channelId, activateWithMasterChannel, operatingMode, duration, out bool activateMasterChannel))
			{
				string result = new HttpClient(GetHttpClientHandler()).GetAsync($"https://{PrivateData.RemoteHostName}/watering/ctrl/{channelId}/on/?master={(activateMasterChannel ? "on" : "off")}").EnsureResultSuccessStatusCode().Result.Content.ReadAsStringAsync().Result;
				ResultClass deserializeObject = JsonConvert.DeserializeObject<ResultClass>(result);
				if (deserializeObject.CurrentState == true)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, operatingMode, "OK", $"Der Kanal {channelId} wurde {(activateMasterChannel ? "mit" : "ohne")} dem MasterKanal aktiviert.\r\nDie Antwort des Servers lautet: {result}"));
				}
				else
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, operatingMode, "Error", $"Es wurde versucht, den Kanal {channelId} {(activateMasterChannel ? "mit" : "ohne")} dem MasterKanal zu aktiviert, dies ist Fehlgeschlagen.\r\nDie Antwort des Servers lautet: {result}"));
				}
			}
		}

		public static void Deactivate(int channelId, string operatingMode)
		{
			if (WaterRelaisControl.Instance.Deactivate(channelId, operatingMode, out bool deactivateMasterChannel))
			{
				string result = new HttpClient(GetHttpClientHandler()).GetAsync($"https://{PrivateData.RemoteHostName}/watering/ctrl/{channelId}/off/").EnsureResultSuccessStatusCode().Result.Content.ReadAsStringAsync().Result;
				ResultClass deserializeObject = JsonConvert.DeserializeObject<ResultClass>(result);
				if (deserializeObject.CurrentState == false)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, operatingMode, "OK", $"Der Kanal {channelId} wurde deaktiviert.\r\nDie Antwort des Servers lautet: {result}"));
				}
				else
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, operatingMode, "Error", $"Es wurde versucht, den Kanal {channelId} zu deaktiviert, dies ist Fehlgeschlagen.\r\nDie Antwort des Servers lautet: {result}"));
				}
			}
			else if (deactivateMasterChannel)
			{
				string result = new HttpClient(GetHttpClientHandler()).GetAsync($"https://{PrivateData.RemoteHostName}/watering/ctrl/{0}/off/").EnsureResultSuccessStatusCode().Result.Content.ReadAsStringAsync().Result;
				ResultClass deserializeObject = JsonConvert.DeserializeObject<ResultClass>(result);
				if (deserializeObject.CurrentState == false)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, operatingMode, "OK", $"Der Kanal {channelId} wurde deaktiviert.\r\nDie Antwort des Servers lautet: {result}"));
				}
				else
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, operatingMode, "Error", $"Es wurde versucht, den Kanal {channelId} zu deaktiviert, dies ist Fehlgeschlagen.\r\nDie Antwort des Servers lautet: {result}"));
				}
			}
		}

		private static HttpClientHandler GetHttpClientHandler() => new HttpClientHandler
		{
			ClientCertificateOptions = ClientCertificateOption.Manual,
			SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls,
			ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true, //TODO: remove this SecurityCritical Line
			ClientCertificates = { new X509Certificate2(Encoding.ASCII.GetBytes(PrivateData.Certificate)) } //TODO: and fix this line so the above is not needed
		};

		// ReSharper disable once ClassNeverInstantiated.Local
		private class ResultClass
		{
#pragma warning disable 169
#pragma warning disable 649
			public string Channel;
			public bool CurrentState;
			public bool MasterState;
			public int OnlineChannels;
			public string RequestedState;
			public string Result;
#pragma warning restore 649
#pragma warning restore 169
		}

		public static void DeactivateAll(string operatingMode)
		{
			if (WaterRelaisControl.Instance.DeactivateAll(operatingMode))
			{
				string result = new HttpClient(GetHttpClientHandler()).GetAsync($"https://{PrivateData.RemoteHostName}/watering/ctrl/99/off/").EnsureResultSuccessStatusCode().Result.Content.ReadAsStringAsync().Result;
				ResultClass deserializeObject = JsonConvert.DeserializeObject<ResultClass>(result);
				if (deserializeObject.CurrentState == false)
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, operatingMode, "OK", $"Alle Kanäle wurden deaktiviert.\r\nDie Antwort des Servers lautet: {result}"));
				}
				else
				{
					HistoryController.AddLogEntry(new HistoryElement(DateTime.Now, operatingMode, "Error", $"Es wurde versucht alle Kanäle zu deaktivieren, dies ist Fehlgeschlagen.\r\nDie Antwort des Servers lautet: {result}"));
				}
			}
		}
	}
}

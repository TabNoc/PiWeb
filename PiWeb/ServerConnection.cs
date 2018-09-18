using System.Net.Http;
using System.Threading.Tasks;
using TabNoc.MyOoui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Storage;

namespace TabNoc.PiWeb
{
	internal class ServerConnection
	{
		public const string DockerHostName = "172.18.0.1";
		public const string RemoteHostName = "piw";

		public static Task<HttpResponseMessage> DeleteAsync(string api, string apiPath)
		{
			return new HttpClient().DeleteAsync($"{PageStorage<BackendData>.Instance.StorageData.GetUrl(api)}/{apiPath}").EnsureResultSuccessStatusCode();
		}
	}
}

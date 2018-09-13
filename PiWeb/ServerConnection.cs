using System.Net.Http;
using System.Threading.Tasks;
using TabNoc.MyOoui;

namespace TabNoc.PiWeb
{
	internal class ServerConnection
	{
		public const string RemoteHostName = "piw";
		public const string DockerHostName = "172.18.0.1";

		public static Task<HttpResponseMessage> DeleteAsync(string apiPath)
		{
			return new HttpClient().DeleteAsync($"http://{DockerHostName}:5000/api/{apiPath}").EnsureResultSuccessStatusCode();
		}
	}
}

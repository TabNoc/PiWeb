using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TabNoc.MyOoui
{
	public static class HttpExtensions
	{
		public static Task<HttpResponseMessage> EnsureResultSuccessStatusCode(this Task<HttpResponseMessage> task)
		{
			task.Wait();
			if (task.Exception != null)
			{
				Console.WriteLine("EnsureResultSuccessStatusCode>Invalid Response: task.Exception != null\r\n" + task.Exception.ToString());
			}
			if (task.Result.IsSuccessStatusCode == false)
			{
				throw new HttpRequestException(task.Result.ReasonPhrase + "(" + (int)task.Result.StatusCode + "):" + task.Result.Content.ReadAsStringAsync().Result);
			}
			else
			{
				Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] EnsureResultSuccessStatusCode>Successful Response ({task.Result.Content.ReadAsStringAsync().Result.Length}B) from {task.Result.RequestMessage.RequestUri}");
			}

			return task;
		}

		public static string GetQueryString(string baseQueryString, string subQueryString, params (string, object)[] valueTuples) =>
			baseQueryString + (subQueryString.Length > 0 ? "/" : "") + subQueryString + "?" + valueTuples.Aggregate("",
				(s, tuple) => $"{s}{(s.Length > 0 ? "&" : "")}{tuple.Item1}={tuple.Item2.ToString()}");
	}
}

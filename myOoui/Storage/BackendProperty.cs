using System.Diagnostics;

namespace TabNoc.MyOoui.Storage
{
	public class BackendProperty
	{
		public string DataSourcePath;
		public bool RequestDataFromBackend;

		public BackendProperty(string dataSourcePath, bool requestDataFromBackend)
		{
			DataSourcePath = dataSourcePath;
			RequestDataFromBackend = requestDataFromBackend;
		}
	}
}

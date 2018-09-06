using System.Diagnostics;

namespace TabNoc.MyOoui.Storage
{
	public class BackedProperties
	{
		public string DataSourcePath;
		public bool RequestDataFromBackend;

		public BackedProperties(string dataSourcePath, bool requestDataFromBackend)
		{
			DataSourcePath = dataSourcePath;
			RequestDataFromBackend = requestDataFromBackend;
		}
	}
}

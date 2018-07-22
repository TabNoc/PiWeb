using System.Diagnostics;

namespace TabNoc.MyOoui.Storage
{
	public class BackedProperties
	{
		public string DataSourcePath;
		public bool RequestDataFromBackend;
		public bool SendDataToBackend;

		public BackedProperties(string dataSourcePath, bool requestDataFromBackend)
		{
			DataSourcePath = dataSourcePath;
			RequestDataFromBackend = requestDataFromBackend;
			SendDataToBackend = requestDataFromBackend;
			if (SendDataToBackend == false && RequestDataFromBackend || RequestDataFromBackend == false && SendDataToBackend)
			{
				Debug.Fail("Dies sollte nur zu Testzwecken durchgeführt werden");
			}
		}
	}
}

using System;
using System.Collections.Generic;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.PiWeb.DataTypes;

namespace TabNoc.MyOoui.Storage
{
	public class BackendData : PageData
	{
		public Dictionary<string, BackendProperty> BackendProperties;

		private static Dictionary<string, BackendProperty> _setupBackedPropertieses;
		public bool SingleApiConfiguration;
		public bool MultiApiRequestDataFromBackend;
		public string MultiApiDataSourcePath;

		public new static BackendData CreateNew()
		{
			if (_setupBackedPropertieses == null)
			{
				throw new InvalidOperationException("BackendData.Setup has to be called before using PageStorage");
			}
			return new BackendData()
			{
				BackendProperties = new Dictionary<string, BackendProperty>(_setupBackedPropertieses),
				Valid = true,
				SingleApiConfiguration = true,
				MultiApiRequestDataFromBackend = false,
				MultiApiDataSourcePath = ""
			};
		}

		public static void Setup(Dictionary<string, BackendProperty> backedPropertieses)
		{
			if (_setupBackedPropertieses != null)
			{
				throw new InvalidOperationException("Setup can only be called once");
			}
			_setupBackedPropertieses = backedPropertieses;
			PageStorage<BackendData>.Instance.Initialize("Backend", TimeSpan.MaxValue);
		}

		public string GetUrl(string api)
		{
			if (SingleApiConfiguration)
			{
				if (_setupBackedPropertieses[api].RequestDataFromBackend)
				{
					return _setupBackedPropertieses[api].DataSourcePath;
				}
				else
				{
					//TODO: safe result
					return "";
				}
			}
			else
			{
				if (MultiApiRequestDataFromBackend)
				{
					return MultiApiDataSourcePath + "/" + api;
				}
				else
				{
					//TODO: safe result
					return "";
				}
			}
		}
	}
}

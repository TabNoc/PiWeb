using System;
using System.Collections.Generic;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.Storage
{
	public class BackendData : PageData
	{
		public Dictionary<string, BackedProperties> BackedPropertieses;

		private static Dictionary<string, BackedProperties> _setupBackedPropertieses;

		public new static BackendData CreateNew()
		{
			if (_setupBackedPropertieses == null)
			{
				throw new InvalidOperationException("BackendData.Setup has to be called before using PageStorage");
			}
			return new BackendData()
			{
				BackedPropertieses = new Dictionary<string, BackedProperties>(_setupBackedPropertieses),
				Valid = true
			};
		}

		public static void Setup(Dictionary<string, BackedProperties> backedPropertieses)
		{
			if (_setupBackedPropertieses != null)
			{
				throw new InvalidOperationException("Setup can only be called once");
			}
			_setupBackedPropertieses = backedPropertieses;
			PageStorage<BackendData>.Instance.Initialize("Backend", TimeSpan.MaxValue);
		}
	}
}

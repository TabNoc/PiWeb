using System.Collections.Generic;
using System.Threading.Tasks;

namespace TabNoc.MyOoui.Interfaces.delegates
{
	public delegate Task<List<(T, List<string>)>> SearchFetchEntriesDelegate<T>(string searchString, int collumn, int amount);
}
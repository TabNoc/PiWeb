using System.Collections.Generic;
using System.Threading.Tasks;

namespace TabNoc.MyOoui.Interfaces.delegates
{
	public delegate Task<List<(T, List<string>)>> FetchEntriesDelegate<T>(T fetchFromPrimaryKey, int takeAmount);
}
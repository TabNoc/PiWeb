using Ooui;
using System.Collections.Generic;

namespace TabNoc.MyOoui.Interfaces
{
	internal interface IAutoComplete
	{
		void ActivateAutocomplete(string src, Dictionary<string, TextInput> resulTextInputs);
	}
}

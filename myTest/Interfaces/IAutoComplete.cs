using System.Collections.Generic;
using Ooui;

namespace TabNoc.Ooui.Interfaces
{
	internal interface IAutoComplete
	{
		void ActivateAutocomplete(string src, Dictionary<string, TextInput> resulTextInputs);
	}
}

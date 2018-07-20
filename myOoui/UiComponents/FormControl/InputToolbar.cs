using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents.FormControl
{
	internal class InputToolbar : global::Ooui.FormControl
	{
		public InputToolbar(string groupLabel) : base("div")
		{
			//<div class=" mb-3" role="toolbar" aria-label="Toolbar with button groups">
			ClassName = "btn-toolbar";
			SetAttribute("role", "toolbar");
			SetAttribute("aria-label", groupLabel);
		}

		public void AppendGroup(InputGroupControl group)
		{
			AppendChild(group);
		}
	}
}

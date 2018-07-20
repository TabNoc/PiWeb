using Ooui;

namespace TabNoc.MyOoui.HtmlElements
{
	internal class Script : Element
	{
		public Script(string scriptContent) : base("script")
		{
			this.Text = scriptContent;
		}
	}
}
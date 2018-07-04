using Ooui;

namespace TabNoc.Ooui.HtmlElements
{
	internal class Script : Element
	{
		public Script(string scriptContent) : base("script")
		{
			this.Text = scriptContent;
		}
	}
}
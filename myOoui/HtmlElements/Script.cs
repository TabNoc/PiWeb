using Ooui;

namespace TabNoc.MyOoui.HtmlElements
{
	internal class Script : Element
	{
		public Script() : base("script")
		{
		}

		public Script SetContent(string scriptContent)
		{
			this.Text = scriptContent;
			return this;
		}

		public Script SetSource(string souce)
		{
			this.SetAttribute("src", souce);
			return this;
		}
	}
}

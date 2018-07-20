using Ooui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents
{
	public class Container : StylableElement
	{
		public Container() : base("div")
		{
			ClassName = "container";
		}

		public Container(Element content) : this()
		{
			AppendChild(content);
		}
	}
}

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

		public Container(Element parent) : this()
		{
			parent.AppendChild(this);
		}
	}
}

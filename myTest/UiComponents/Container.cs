using System;
using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents
{
	internal class Container : StylableElement
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

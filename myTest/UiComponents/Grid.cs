using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents
{
	internal class Grid : StylableElement
	{
		public Grid(Element parent) : base("div")
		{
			parent.AppendChild(this);
		}

		public Row AddRow()
		{
			Row addRow = new Row();
			AppendChild(addRow);
			return addRow;
		}
	}
}

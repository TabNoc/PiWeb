using Ooui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents
{
	public class Grid : StylableElement
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

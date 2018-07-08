using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents
{
	internal class Grid : StylableElement
	{
		public Grid() : base("div")
		{
		}

		public Row AddRow()
		{
			Row addRow = new Row();
			AppendChild(addRow);
			return addRow;
		}
	}
}

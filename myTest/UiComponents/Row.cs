using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents
{
	internal class Row : StylableElement
	{
		public Row() : base("div")
		{
			ClassName = "row";
		}

		public void AddNewLine()
		{
			AppendChild(new Div
			{
				ClassName = "w-100"
			});
		}

		public void AppendCollum(Element content, int sizing = 0, bool autoSize = false)
		{
			Div newChild = new Div()
			{
				ClassName = "col" + (sizing > 0 ? $"-{sizing}" : "") + (autoSize == true ? "-auto" : "")
			};
			newChild.AppendChild(content);
			AppendChild(newChild);
		}
	}
}

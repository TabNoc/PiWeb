using Ooui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents
{
	public class Row : StylableElement
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

		public T AppendCollum<T>(T content, int sizing = 0, bool autoSize = false) where T : Element
		{
			Div newChild = new Div()
			{
				ClassName = "col" + (sizing > 0 ? $"-{sizing}" : "") + (autoSize == true ? "-auto" : "")
			};
			newChild.AppendChild(content);
			AppendChild(newChild);
			return content;
		}
	}
}

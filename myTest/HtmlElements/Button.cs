using System;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.HtmlElements
{
	internal class Button : StylableElement
	{
		public Button(StylingColor buttonType = StylingColor.Primary, bool asOutline = false, ButtonSize size = ButtonSize.Normal, bool asBlock = false, string text = "", int widthInPx = -1) : base("button")
		{
			SetAttribute("type", "button");
			ClassName = $"btn btn-{(asOutline == true ? "outline-" : "")}{Enum.GetName(typeof(StylingColor), buttonType).ToLower()}";
			ClassName += (size == ButtonSize.Large ? " btn-lg" : size == ButtonSize.Small ? " btn-sm" : "");
			ClassName += (asBlock == true ? " btn-block" : "");
			if (string.IsNullOrWhiteSpace(text) == false)
			{
				Text = text;
			}

			if (widthInPx > 0)
			{
				Style.Width = widthInPx;
			}
		}

		internal enum ButtonSize
		{
			Small,
			Normal,
			Large
		}
	}
}

using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.Interfaces
{
	internal interface IStylableElement
	{
		bool IsDisabled
		{
			get;
			set;
		}

		void AddStyling(StylingOption styling, int value = 0, BreakPoint breakPoint = BreakPoint.None);

		void RemoveStyling(StylingOption styling, int amount, BreakPoint breakPoint);

		void SetBorder(BorderKind borderKind, StylingColor borderColor);

		void SetValidation(bool isValid, bool isInvalid);

		void SetToolTip(ToolTipLocation location, string tooltipText);
	}
}

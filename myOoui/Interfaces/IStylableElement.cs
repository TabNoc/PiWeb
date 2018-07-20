using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;

namespace TabNoc.MyOoui.Interfaces
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

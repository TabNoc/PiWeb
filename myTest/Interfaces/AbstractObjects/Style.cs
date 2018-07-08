﻿using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.Interfaces.AbstractObjects
{
	internal class Style
	{
		internal readonly StylingOption StylingOption;
		internal readonly int Amount;
		internal readonly BreakPoint StylingBreakpoint;

		public Style(StylingOption stylingOption, int amount, BreakPoint stylingBreakpoint)
		{
			StylingOption = stylingOption;
			Amount = amount;
			StylingBreakpoint = stylingBreakpoint;
		}
	}
}
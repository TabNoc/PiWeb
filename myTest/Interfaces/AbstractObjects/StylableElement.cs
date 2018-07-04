using Ooui;
using System;
using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.Interfaces.AbstractObjects
{
	internal abstract class StylableElement : Element
	{
		private string _className = "";
		private BorderKind _borderKind;
		private StylingColor _borderStylingColor;

		public bool IsDisabled
		{
			get => GetBooleanAttribute("disabled");
			set => SetBooleanAttributeProperty("disabled", value);
		}

		protected StylableElement(string tagName) : base(tagName)
		{
			_borderKind = BorderKind.Rounded_0;
			_borderStylingColor = StylingColor.Info;
		}

		private readonly Dictionary<StylingOption, int> _stylingDictionary = new Dictionary<StylingOption, int>();

		protected new string ClassName
		{
			get => _className;
			set
			{
				_className = value;
				CalculateClassName();
			}
		}

		public void AddStyling(StylingOption styling, int value = 0)
		{
			if (value > 5 || value < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}
			_stylingDictionary.Add(styling, value);
			CalculateClassName();
		}

		public void RemoveStyling(StylingOption styling)
		{
			_stylingDictionary.Remove(styling);
			CalculateClassName();
		}

		private void CalculateClassName()
		{
			string returnval = ClassName;
			foreach (KeyValuePair<StylingOption, int> pair in _stylingDictionary)
			{
				switch (pair.Key)
				{
					case StylingOption.MarginTop:
						returnval += $" mt-{pair.Value}";
						break;

					case StylingOption.MarginBottom:
						returnval += $" mb-{pair.Value}";
						break;

					case StylingOption.MarginRight:
						returnval += $" mr-{pair.Value}";
						break;

					case StylingOption.MarginLeft:
						returnval += $" ml-{pair.Value}";
						break;

					case StylingOption.PaddingTop:
						returnval += $" pt-{pair.Value}";
						break;

					case StylingOption.PaddingBottom:
						returnval += $" pb-{pair.Value}";
						break;

					case StylingOption.PaddingRight:
						returnval += $" pr-{pair.Value}";
						break;

					case StylingOption.PaddingLeft:
						returnval += $" pl-{pair.Value}";
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (_borderKind != BorderKind.Rounded_0)
			{
				returnval += $" border border-{Enum.GetName(typeof(StylingColor), _borderStylingColor).ToLower()} {Enum.GetName(typeof(BorderKind), _borderKind).ToLower().Replace("_", "-")}";
			}
			base.ClassName = returnval;
		}

		public void SetBorder(BorderKind borderKind, StylingColor borderColor)
		{
			_borderKind = borderKind;
			_borderStylingColor = borderColor;
			CalculateClassName();
		}

		//public void SetBreakPoint(BreakPoint breakPoint, int value = 0)
		//{
		//	if (value > 5 || value < 0)
		//	{
		//		throw new ArgumentOutOfRangeException(nameof(value));
		//	}
		//	_stylingDictionary.Add(styling, value);
		//	CalculateClassName();
		//}
	}

	internal enum BreakPoint
	{
		ExtraSmall,
		Small,
		Medium,
		Large,
		ExtraLarge
	}
}

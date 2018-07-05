using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.Interfaces.AbstractObjects
{
	internal class StylableTextInput : TextInput, IStylableElement
	{
		private string _className = "";
		private BorderKind _borderKind;
		private StylingColor _borderStylingColor;
		private bool _isValid = false;
		private bool _isInvalid = false;

		public StylableTextInput() : base()
		{
			_borderKind = BorderKind.Rounded_0;
			_borderStylingColor = StylingColor.Info;
		}

		private readonly List<Style> _styles = new List<Style>();

		public new string ClassName
		{
			get => _className;
			set
			{
				_className = value;
				CalculateClassName();
			}
		}

		public void AddStyling(StylingOption styling, int value = 0, BreakPoint breakPoint = BreakPoint.None)
		{
			if (value > 5 || value < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			_styles.Add(new Style(styling, value, breakPoint));
			CalculateClassName();
		}

		public void RemoveStyling(StylingOption styling, int amount, BreakPoint breakPoint)
		{
			_styles.Remove(_styles.First(style => style.StylingBreakpoint == breakPoint && style.Amount == amount && style.StylingOption == styling));
			CalculateClassName();
		}

		private void CalculateClassName()
		{
			string returnval = ClassName;
			foreach (Style style in _styles)
			{
				switch (style.StylingOption)
				{
					case StylingOption.MarginTop:
						returnval += " mt";
						break;

					case StylingOption.MarginBottom:
						returnval += " mb";
						break;

					case StylingOption.MarginRight:
						returnval += " mr";
						break;

					case StylingOption.MarginLeft:
						returnval += " ml";
						break;

					case StylingOption.PaddingTop:
						returnval += " pt";
						break;

					case StylingOption.PaddingBottom:
						returnval += " pb";
						break;

					case StylingOption.PaddingRight:
						returnval += " pr";
						break;

					case StylingOption.PaddingLeft:
						returnval += " pl";
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}

				if (style.StylingBreakpoint != BreakPoint.None)
				{
					switch (style.StylingBreakpoint)
					{
						case BreakPoint.ExtraSmall:
							returnval += "-xs";
							break;

						case BreakPoint.Small:
							returnval += "-sm";
							break;

						case BreakPoint.Medium:
							returnval += "-md";
							break;

						case BreakPoint.Large:
							returnval += "-lg";
							break;

						case BreakPoint.ExtraLarge:
							returnval += "-xl";
							break;

						case BreakPoint.None:
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				returnval += $"-{style.Amount}";
			}

			if (_borderKind != BorderKind.Rounded_0)
			{
				returnval += $" border border-{Enum.GetName(typeof(StylingColor), _borderStylingColor).ToLower()} {Enum.GetName(typeof(BorderKind), _borderKind).ToLower().Replace("_", "-")}";
			}

			if (_isInvalid)
			{
				returnval += " is-invalid";
			}

			if (_isValid)
			{
				returnval += " is-valid";
			}

			base.ClassName = returnval;
		}

		public void SetBorder(BorderKind borderKind, StylingColor borderColor)
		{
			_borderKind = borderKind;
			_borderStylingColor = borderColor;
			CalculateClassName();
		}

		public void SetValidation(bool isValid, bool isInvalid)
		{
			if (isInvalid && isValid)
			{
				throw new ArgumentOutOfRangeException(nameof(isValid) + " & " + nameof(isInvalid), "Validation darf nicht inValid und Valid zugleich sein");
			}
			this._isValid = isValid;
			this._isInvalid = isInvalid;

			CalculateClassName();
		}
	}
}

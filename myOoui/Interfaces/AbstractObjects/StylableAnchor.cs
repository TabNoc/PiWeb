using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.Ooui.HtmlElements;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.Interfaces.AbstractObjects
{
	public class StylableAnchor : Anchor, IStylableElement
	{
		private string _className = "";
		private BorderKind _borderKind = BorderKind.Rounded_0;
		private StylingColor _borderStylingColor = StylingColor.Info;
		private bool _isValid = false;
		private bool _isInvalid = false;

		private readonly List<Style> _styles = new List<Style>();

		public StylableAnchor()
			: base()
		{
		}

		public StylableAnchor(string href)
			: base(href)
		{
		}

		public StylableAnchor(string href, string text)
			: base(href, text)
		{
		}

		public new string ClassName
		{
			get => _className;
			set
			{
				_className = value;
				CalculateClassName();
			}
		}

		public bool IsDisabled
		{
			get; set;
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

		public void SetToolTip(ToolTipLocation location, string tooltipText)
		{
			if (GetAttribute("data-toggle") != null)
			{
				throw new InvalidOperationException("Der Element hat bereits ein \"data-toggle\" Attribut, daher kann kein Tooltip hinzugefügt werden!");
			}
			SetAttribute("data-toggle", "tooltip");
			SetAttribute("data-placement", Enum.GetName(typeof(ToolTipLocation), location).ToLower());
			SetAttribute("title", tooltipText);

			AppendChild(new Script("$('#" + Id + "').tooltip()"));
		}
	}
}

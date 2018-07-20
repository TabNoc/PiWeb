using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.MyOoui.HtmlElements;
using TabNoc.MyOoui.Interfaces.Enums;

namespace TabNoc.MyOoui.Interfaces.AbstractObjects
{
	public class StylableTextInput : TextInput, IStylableElement, IAutoComplete
	{
		private readonly List<Style> _styles = new List<Style>();
		private BorderKind _borderKind;
		private StylingColor _borderStylingColor;
		private string _className = "";
		private bool _isInvalid = false;
		private bool _isValid = false;

		public new string ClassName
		{
			get => _className;
			set
			{
				_className = value;
				CalculateClassName();
			}
		}

		public StylableTextInput() : base()
		{
			_borderKind = BorderKind.Rounded_0;
			_borderStylingColor = StylingColor.Info;
		}

		public void ActivateAutocomplete(string src, Dictionary<string, TextInput> resulTextInputs)
		{
			string resultOutputData = "({";
			foreach ((string key, TextInput textInput) in resulTextInputs)
			{
				if (resultOutputData.Length > 2)
				{
					resultOutputData += ",";
				}
				resultOutputData += $"{key}: \"{textInput.Id}\"";
			}
			resultOutputData += "})";

			Script script = new Script($@"
$.get(""{src}"", function(data) {{
	$(""#{Id}"").typeahead({{ source:data, afterSelect:function(selectedElement) {{
		try {{
			let resultOutputData = {resultOutputData};
			let element = $(""#{Id}"");
			for (let index in selectedElement) {{
				send(JSON.stringify({{
		            m: ""event"",
		            id: resultOutputData[index],
		            k: ""change"",
					v: selectedElement[index]
		        }}));
			}}
		}}
		catch (exc) {{
			console.error(exc);
		}}
	}}}});
}},'json');");
			AppendChild(script);
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

		public void SetBorder(BorderKind borderKind, StylingColor borderColor)
		{
			_borderKind = borderKind;
			_borderStylingColor = borderColor;
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
			SetAttribute("title", Text);
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
	}
}

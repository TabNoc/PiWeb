using Ooui;
using System;
using TabNoc.Ooui.Interfaces;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using Button = TabNoc.Ooui.HtmlElements.Button;

namespace TabNoc.Ooui.UiComponents.FormControl
{
	internal class Dropdown : StylableElement
	{
		public readonly Button Button;
		public readonly Button SplitButton;
		private readonly Div _dropDownMenu;

		public Dropdown(Button button, DropdownDirection dropdownDirection = DropdownDirection.DropDown, bool asSplitButton = false) : base("div")
		{
			Button = button;
			switch (dropdownDirection)
			{
				case DropdownDirection.DropDown:
					ClassName = "dropdown";
					break;

				case DropdownDirection.DropUp:
					ClassName = "btn-group dropup";
					break;

				case DropdownDirection.DropRight:
					ClassName = "btn-group dropright";
					break;

				case DropdownDirection.DropLeft:
					ClassName = "btn-group dropleft";
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(dropdownDirection), dropdownDirection, null);
			}

			if (asSplitButton)
			{
				SplitButton = new Button() { ClassName = button.ClassName + "dropdown-toggle dropdown-toggle-split" };
				SplitButton.SetAttribute("data-toggle", "dropdown");
				SplitButton.SetAttribute("aria-haspopup", "true");
				SplitButton.SetAttribute("aria-expanded", "false");
				AppendChild(button);
				AppendChild(SplitButton);
			}
			else
			{
				button.ClassName += " dropdown-toggle";
				button.SetAttribute("data-toggle", "dropdown");
				button.SetAttribute("aria-haspopup", "true");
				button.SetAttribute("aria-expanded", "false");
				AppendChild(button);
			}

			_dropDownMenu = new Div()
			{
				ClassName = "dropdown-menu"
			};
			AppendChild(_dropDownMenu);
		}

		public StylableAnchor AddEntry(string text, bool active = false)
		{
			StylableAnchor anchor = new StylableAnchor("#", text) { ClassName = "dropdown-item" + (active == true ? " active" : "") };
			_dropDownMenu.AppendChild(anchor);
			return anchor;
		}

		public void AddLabel(string text)
		{
			_dropDownMenu.AppendChild(new Span(text) { ClassName = "dropdown-item-text" });
		}

		public void AddHeaderLabel(string text)
		{
			_dropDownMenu.AppendChild(new Heading(6, text) { ClassName = "dropdown-header" });
		}

		public void AddDivider()
		{
			_dropDownMenu.AppendChild(new Div() { ClassName = "dropdown-divider" });
		}
	}
}

using System;
using TabNoc.MyOoui.HtmlElements;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;

namespace TabNoc.MyOoui.UiComponents.FormControl.InputGroups.Components
{
	public class OverrideInputGroup : StylableElement
	{
		public readonly StylableTextInput OverrideTextInput;

		public int Value
		{
			get
			{
				if (int.TryParse(OverrideTextInput.Value.Replace("%", ""), out int number))
				{
					return number;
				}

				throw new FormatException();
			}
		}

		public OverrideInputGroup(int startValue, string labelText = "Dauer Relativ", int labelSizeInPx = -1, int increment = 5) : base("div")
		{
			MultiInputGroup multiInputGroup = new MultiInputGroup();
			multiInputGroup.AppendLabel(labelText, labelSizeInPx);
			OverrideTextInput = multiInputGroup.AppendTextInput("100%", startText: startValue + "%");
			OverrideTextInput.Style.Width = 110;
			multiInputGroup.AppendValidation("", "Bitte nur ganze zahlen mit optionalem % Zeichen angeben!", false);
			Button incrementOverrideButton = new Button(StylingColor.Secondary, true, text: "+", widthInPx: 35);
			incrementOverrideButton.Click += (sender, args) =>
			{
				if (int.TryParse(OverrideTextInput.Value.Replace("%", ""), out int number))
				{
					OverrideTextInput.Value = number + increment + "%";
					OverrideTextInput.SetValidation(false, false);
				}
				else
				{
					OverrideTextInput.SetValidation(false, true);
				}
			};
			multiInputGroup.AppendCustomElement(incrementOverrideButton, false);

			Button decrementOverrideButton = new Button(StylingColor.Secondary, true, text: "-", widthInPx: 35);
			decrementOverrideButton.Click += (sender, args) =>
			{
				if (int.TryParse(OverrideTextInput.Value.Replace("%", ""), out int number))
				{
					OverrideTextInput.Value = number - increment + "%";
					OverrideTextInput.SetValidation(false, false);
				}
				else
				{
					OverrideTextInput.SetValidation(false, true);
				}
			};
			multiInputGroup.AppendCustomElement(decrementOverrideButton, false);

			multiInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			multiInputGroup.AddStyling(StylingOption.MarginTop, 2);
			AppendChild(multiInputGroup);
		}
	}
}

using Ooui;
using System;
using TabNoc.MyOoui.HtmlElements;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents.FormControl
{
	internal class RadioButtonGroup : InputGroupControl
	{
		private bool _hasActiveRadioButton = false;

		public RadioButtonGroup()
		{
			ClassName = "btn-group btn-group-toggle";
			SetAttribute("data-toggle", "buttons");
		}

		/*
<div class="btn-group btn-group-toggle" data-toggle="buttons">
	<label class="btn btn-secondary active">
		<input type="radio" name="options" id="option1" autocomplete="off" checked> Active
	</label>
	<label class="btn btn-secondary">
		<input type="radio" name="options" id="option2" autocomplete="off"> Radio
	</label>
	<label class="btn btn-secondary">
		<input type="radio" name="options" id="option3" autocomplete="off"> Radio
	</label>
</div>
		 */

		public void AddRadioButton(string buttonText, bool pressedState)
		{
			if (_hasActiveRadioButton == true && pressedState == true)
			{
				throw new ArgumentException("Es darf nur ein RadioButton gedrückt sein", nameof(pressedState));
			}
			_hasActiveRadioButton |= pressedState;
			Label label = new Label
			{
				ClassName = "btn btn-secondary" + (pressedState ? " active" : "")
			};

			SimpleInput input = new SimpleInput();
			input.SetAttribute("type", "radio");
			input.SetAttribute("name", "options");
			input.SetAttribute("autocomplete", "off");
			if (pressedState)
			{
				input.SetAttribute("checked", "");
			}
			label.Text = buttonText;
			label.AppendChild(input);
			AppendChild(label);
		}
	}
}

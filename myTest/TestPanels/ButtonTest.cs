using Ooui;
using System;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.UiComponents.FormControl;
using TabNoc.Ooui.UiComponents.FormControl.InputGroups;

namespace TabNoc.Ooui.TestPanels
{
	internal class ButtonTest : Publishable
	{
		public ButtonTest(string publishPath) : base(publishPath)
		{
		}

		protected override Element PopulateAppElement()
		{
			Div div = new Div();

			ButtonGroup buttonGroup = new ButtonGroup();
			buttonGroup.AddButton("123");
			buttonGroup.AddButton("456");
			buttonGroup.AddButton("789");

			div.AppendChild(buttonGroup);

			ToggleButtonGroup toggleButtonGroup = new ToggleButtonGroup(StylingColor.Primary, StylingColor.Secondary);
			toggleButtonGroup.AddToggleButton("123", true);
			toggleButtonGroup.AddToggleButton("456", false);
			toggleButtonGroup.AddToggleButton("789", false);

			div.AppendChild(toggleButtonGroup);

			RadioButtonGroup radioButtonGroup = new RadioButtonGroup();
			radioButtonGroup.AddRadioButton("123", true);
			radioButtonGroup.AddRadioButton("456", false);
			radioButtonGroup.AddRadioButton("789", false);

			div.AppendChild(radioButtonGroup);

			InputToolbar inputToolbar = new InputToolbar("apfel");
			ButtonGroup inputButtonGroup = new ButtonGroup();
			inputButtonGroup.AddButton("1");
			inputButtonGroup.AddButton("2");
			inputButtonGroup.AddButton("3");
			inputToolbar.AppendGroup(inputButtonGroup);
			TextInputGroup inputGroup = new TextInputGroup("Hallo?>", "name?");
			inputToolbar.AppendGroup(inputGroup);
			inputGroup.TextInput.Change += (sender, args) =>
			{
				Console.WriteLine(inputGroup.TextInput.Value);
			};

			div.AppendChild(inputToolbar);
			return div;
		}
	}
}

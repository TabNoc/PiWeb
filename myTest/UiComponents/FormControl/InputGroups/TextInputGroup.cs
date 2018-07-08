using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents.FormControl.InputGroups
{
	internal class TextInputGroup : InputGroupControl
	{
		public readonly StylableTextInput TextInput;

		public TextInputGroup(string labelText, string textBoxDescriptionMessage, int sizeInPx = -1, string validFeedback = "", string inValidFeedback = "", bool feedbackAsTooltip = false, bool centeredText = false)
		{
			ClassName = "input-group";

			Div div1 = new Div
			{
				ClassName = "input-group-prepend",
				Style = { Width = sizeInPx }
			};
			AppendChild(div1);

			Div div2 = new Div
			{
				ClassName = "input-group-text w-100",
				Text = labelText
			};
			div1.AppendChild(div2);
			TextInput = new StylableTextInput();
			TextInput.SetAttribute("type", "text");
			TextInput.ClassName = "form-control" + (centeredText ? " text-center"  :"");
			TextInput.SetAttribute("placeholder", textBoxDescriptionMessage);
			TextInput.SetAttribute("aria-label", textBoxDescriptionMessage);
			TextInput.SetAttribute("aria-describedby", div2.Id);

			AppendChild(TextInput);
			ValidationFeedback.AppendValidationFeedbackElements(this, validFeedback, inValidFeedback, feedbackAsTooltip);
		}

		public void AddFormElement(StylableElement content)
		{
			Div div1 = new Div
			{
				ClassName = "input-group-append"
			};
			AppendChild(div1);
			div1.AppendChild(content);
		}
	}
}

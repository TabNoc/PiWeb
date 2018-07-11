using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents.FormControl.InputGroups
{
	internal class MultiInputGroup : StylableElement
	{
		public MultiInputGroup() : base("div")
		{
			ClassName = "input-group";
		}

		public T AppendCustomElement<T>(T content, bool asExternalFormControl = true) where T : Element
		{
			Div contentDivWrapper = new Div()
			{
				ClassName = $"input-group-{GetClassType()} {(asExternalFormControl == true ? "form-control" : "")}"
			};
			contentDivWrapper.AppendChild(content);

			AppendChild(contentDivWrapper);

			return content;
		}

		public void AppendLabel(string labelText, int labelSizeInPx = -1)
		{
			Div div1 = new Div
			{
				ClassName = $"input-group-{GetClassType()}"
			};
			if (labelSizeInPx > 0)
			{
				div1.Style.Width = labelSizeInPx;
			}
			AppendChild(div1);

			Div div2 = new Div
			{
				ClassName = "input-group-text w-100",
				Text = labelText
			};
			div1.AppendChild(div2);
		}

		public void AppendValidation(string validFeedback = "", string inValidFeedback = "", bool feedbackAsTooltip = false)
		{
			ValidationFeedback.AppendValidationFeedbackElements(this, validFeedback, inValidFeedback, feedbackAsTooltip);
		}

		public StylableTextInput AppendTextInput(string textInputGhostMessage, bool centeredText = false)
		{
			StylableTextInput textInput = new StylableTextInput();
			textInput.SetAttribute("type", "text");
			textInput.ClassName = "form-control" + (centeredText ? " text-center" : "");
			textInput.SetAttribute("placeholder", textInputGhostMessage);
			textInput.SetAttribute("aria-label", textInputGhostMessage);

			AppendChild(textInput);
			return textInput;
		}

		private string GetClassType()
		{
			return (Children.Count == 0 ? "prepend" : "append");
		}
	}
}

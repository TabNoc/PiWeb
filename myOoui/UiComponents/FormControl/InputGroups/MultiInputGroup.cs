using Ooui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents.FormControl.InputGroups
{
	public class MultiInputGroup : StylableElement
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

		public StylableTextInput AppendTextInput(string textInputGhostMessage, bool centeredText = false, string startText = "")
		{
			StylableTextInput textInput = new StylableTextInput();
			textInput.SetAttribute("type", "text");
			textInput.ClassName = "form-control" + (centeredText ? " text-center" : "");
			textInput.SetAttribute("placeholder", textInputGhostMessage);
			textInput.SetAttribute("aria-label", textInputGhostMessage);
			textInput.Value = startText;

			AppendChild(textInput);
			return textInput;
		}

		public void AppendValidation(string validFeedback = "", string inValidFeedback = "", bool feedbackAsTooltip = false)
		{
			ValidationFeedback.AppendValidationFeedbackElements(this, validFeedback, inValidFeedback, feedbackAsTooltip);
		}

		private string GetClassType()
		{
			return (Children.Count == 0 ? "prepend" : "append");
		}
	}
}

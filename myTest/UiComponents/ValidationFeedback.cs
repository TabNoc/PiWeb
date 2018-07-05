using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents
{
	internal class ValidationFeedback : Element
	{
		public ValidationFeedback(Element content, string validFeedback, string inValidFeedback, bool asTooltip = false) : base("div")
		{
			AppendChild(content);
			AppendValidationFeedbackElements(this, validFeedback, inValidFeedback, asTooltip);
		}

		public static void AppendValidationFeedbackElements(Element parent, string validFeedback, string inValidFeedback, bool asTooltip = false)
		{
			if (string.IsNullOrWhiteSpace(validFeedback) == false)
			{
				Div validFeedbackWrapperDiv = new Div
				{
					ClassName = $"valid-{(asTooltip ? "tooltip" : "feedback")}",
					Text = validFeedback
				};
				parent.AppendChild(validFeedbackWrapperDiv);
			}
			if (string.IsNullOrWhiteSpace(inValidFeedback) == false)
			{
				Div inValidFeedbackWrapperDiv = new Div
				{
					ClassName = $"invalid-{(asTooltip ? "tooltip" : "feedback")}",
					Text = inValidFeedback
				};
				parent.AppendChild(inValidFeedbackWrapperDiv);
			}
		}
	}
}

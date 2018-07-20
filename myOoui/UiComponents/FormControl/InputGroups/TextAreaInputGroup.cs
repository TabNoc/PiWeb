using Ooui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents.FormControl.InputGroups
{
	public class TextAreaInputGroup : InputGroupControl
	{
		public readonly TextArea TextArea;

		public TextAreaInputGroup(string labelText, string textBoxDescriptionMessage, int sizeInPx = -1)
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

			TextArea = new TextArea();
			TextArea.SetAttribute("type", "text");
			TextArea.ClassName = "form-control";
			TextArea.SetAttribute("placeholder", textBoxDescriptionMessage);
			TextArea.SetAttribute("aria-label", textBoxDescriptionMessage);
			TextArea.SetAttribute("aria-describedby", div2.Id);

			AppendChild(TextArea);
		}
	}
}

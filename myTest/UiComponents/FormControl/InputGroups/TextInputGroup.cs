using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using Button = TabNoc.Ooui.HtmlElements.Button;

namespace TabNoc.Ooui.UiComponents.FormControl.InputGroups
{
	internal class TextInputGroup : InputGroupControl
	{
		public readonly TextInput TextInput;

		public TextInputGroup(string labelText, string textBoxDescriptionMessage, int sizeInPx = -1)
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

			TextInput = new TextInput();
			TextInput.SetAttribute("type", "text");
			TextInput.ClassName = "form-control";
			TextInput.SetAttribute("placeholder", textBoxDescriptionMessage);
			TextInput.SetAttribute("aria-label", textBoxDescriptionMessage);
			TextInput.SetAttribute("aria-describedby", div2.Id);

			AppendChild(TextInput);
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

	internal class TextAreaInputGroup : InputGroupControl
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

using System;
using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.UiComponents.FormControl.InputGroups
{
	internal class CheckBoxLabeledInputGroup : InputGroupControl
	{
		private readonly Input _checkBox;

		public bool Checked
		{
			get => _checkBox.IsChecked;
			set => _checkBox.IsChecked = value;
		}

		public CheckBoxLabeledInputGroup(bool @checked, string labelText)
		{
			ClassName = "input-group";

			Div checkBoxDiv1 = new Div
			{
				ClassName = "input-group-prepend"
			};
			AppendChild(checkBoxDiv1);

			Div checkBoxDiv2 = new Div
			{
				ClassName = "input-group-text"
			};
			checkBoxDiv1.AppendChild(checkBoxDiv2);

			_checkBox = new Input();
			_checkBox.SetAttribute("type", "checkbox");
			_checkBox.SetAttribute("aria-label", new Guid().ToString());
			checkBoxDiv2.AppendChild(_checkBox);

			// end checkbox

			Div labelDiv1 = new Div
			{
				ClassName = "input-group-append"
			};
			AppendChild(labelDiv1);

			Span labelSpan = new Span(labelText)
			{
				ClassName = "input-group-text"
			};
			labelDiv1.AppendChild(labelSpan);

			Checked = @checked;
		}
	}
	internal class CheckBoxInputGroup : InputGroupControl
	{
		private readonly Input _checkBox;

		public bool Checked
		{
			get => _checkBox.IsChecked;
			set => _checkBox.IsChecked = value;
		}

		public CheckBoxInputGroup(bool @checked, StylableElement content)
		{
			ClassName = "input-group";

			Div checkBoxDiv1 = new Div
			{
				ClassName = "input-group-prepend"
			};
			AppendChild(checkBoxDiv1);

			Div checkBoxDiv2 = new Div
			{
				ClassName = "input-group-text"
			};
			checkBoxDiv1.AppendChild(checkBoxDiv2);

			_checkBox = new Input();
			_checkBox.SetAttribute("type", "checkbox");
			_checkBox.SetAttribute("aria-label", new Guid().ToString());
			checkBoxDiv2.AppendChild(_checkBox);

			// end checkbox

			Div contentDivWrapper = new Div()
			{
				ClassName = "input-group-append form-control"
			};
			contentDivWrapper.AppendChild(content);

			AppendChild(contentDivWrapper);

			Checked = @checked;
		}
	}
}

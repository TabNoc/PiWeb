using System;
using Ooui;
using TabNoc.Ooui.HtmlElements;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents.FormControl.InputGroups
{
	public class RadioButtonInputGroup : InputGroupControl
	{
		private readonly RadioButton _radioButton;

		public bool Checked
		{
			get => _radioButton.IsChecked;
			set => _radioButton.IsChecked = value;
		}

		public readonly string RadioButtonGroupName;

		public RadioButtonInputGroup(bool @checked, StylableElement content, string radioButtonGroupName = "")
		{
			if (string.IsNullOrWhiteSpace(radioButtonGroupName))
			{
				radioButtonGroupName = Guid.NewGuid().ToString();
			}

			RadioButtonGroupName = radioButtonGroupName;

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

			_radioButton = new RadioButton(radioButtonGroupName, @checked);
			checkBoxDiv2.AppendChild(_radioButton);

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

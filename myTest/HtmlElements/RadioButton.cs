using System;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.UiComponents.FormControl;

namespace TabNoc.Ooui.HtmlElements
{
	internal class RadioButton : StylableElement, IDisposable
	{
		private readonly string _radioButtonGroupName;

		public RadioButton(string radioButtonGroupName, bool isChecked) : base("button")
		{
			SetBorder(BorderKind.Rounded_Circle, StylingColor.Dark);
			Style.Height = 18;
			Style.Width = 18;
			_radioButtonGroupName = radioButtonGroupName;
			RadioButtonStorage.RegisterRadioButton(_radioButtonGroupName, this);

			if (isChecked)
			{
				RadioButtonStorage.AssertPositiveStartValueOfRadioButtonGroupIsFree(radioButtonGroupName);
			}
			SetAttribute("type", "radio");
			SetAttribute("name", radioButtonGroupName);
			Click += RadioButton_Click;

			IsChecked = isChecked;
		}

		private void RadioButton_Click(object sender, global::Ooui.TargetEventArgs e)
		{
			RadioButtonStorage.ChangeRadioButtonState(_radioButtonGroupName, (RadioButton)sender, e);
		}

		public bool IsChecked
		{
			get => RadioButtonStorage.GetRadioButtonState(_radioButtonGroupName, this);
			set => RadioButtonStorage.SetRadioButtonState(_radioButtonGroupName, this, value);
		}

		public void Dispose()
		{
			RadioButtonStorage.UnRegisterRadioButton(_radioButtonGroupName, this);
		}

		~RadioButton()
		{
			this.Dispose();
		}
	}
}

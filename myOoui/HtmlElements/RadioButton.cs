using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.MyOoui.Storage;

namespace TabNoc.MyOoui.HtmlElements
{
	internal class RadioButton : StylableElement
	{
		private bool _disposed;

		private readonly string _radioButtonGroupName;

		public RadioButton(string radioButtonGroupName, bool isChecked) : base("button")
		{
			SetBorder(BorderKind.Rounded_Circle, StylingColor.Dark);
			Style.Height = 18;
			Style.Width = 18;
			_radioButtonGroupName = radioButtonGroupName;
			RadioButtonStorage.RegisterRadioButton(_radioButtonGroupName, this, isChecked);

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

		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					RadioButtonStorage.UnRegisterRadioButton(_radioButtonGroupName, this);
				}

				_disposed = true;
			}

			base.Dispose(disposing);
		}

		~RadioButton()
		{
			this.Dispose();
		}
	}
}

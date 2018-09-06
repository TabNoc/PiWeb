using System;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.MyOoui.Storage;

namespace TabNoc.MyOoui.HtmlElements
{
	internal class RadioButton : StylableElement, IDisposable
	{
		private readonly string _radioButtonGroupName;

		public bool IsChecked
		{
			get => RadioButtonStorage.GetRadioButtonState(_radioButtonGroupName, this);
			set => RadioButtonStorage.SetRadioButtonState(_radioButtonGroupName, this, value);
		}

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

		#region Dispose Pattern

		private bool _disposed;

		~RadioButton()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		public new void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!_disposed)
				{
					RadioButtonStorage.UnRegisterRadioButton(_radioButtonGroupName, this);

					_disposed = true;
				}

				base.Dispose(disposing);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		#endregion Dispose Pattern
	}
}

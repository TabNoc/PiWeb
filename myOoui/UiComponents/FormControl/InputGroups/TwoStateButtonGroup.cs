using Ooui;
using System;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents.FormControl.InputGroups
{
	public class TwoStateButtonGroup : InputGroupControl
	{
		/*
  <div class="btn-group mr-2" role="group" aria-label="First group">
    <button type="button" class="btn btn-secondary">1</button>
    <button type="button" class="btn btn-secondary">2</button>
    <button type="button" class="btn btn-secondary">3</button>
    <button type="button" class="btn btn-secondary">4</button>
  </div>
		 */
		public readonly global::Ooui.Button FirstButton;
		public readonly global::Ooui.Button SecondButton;

		public new bool IsDisabled
		{
			get => FirstButton.IsDisabled && SecondButton.IsDisabled;
			set
			{
				FirstButton.IsDisabled = value;
				SecondButton.IsDisabled = value;
				HandleButtonState(null, null);
			}
		}

		public TwoStateButtonGroup(string firstButtonText, string secondButtonText, bool firstPressedState = false, bool secondPressedState = false)
		{
			ClassName = "btn-group";
			SetAttribute("role", "group");

			if (firstPressedState && secondPressedState)
			{
				throw new ArgumentException("Es darf nur einen aktiven Status geben");
			}

			FirstButton = new global::Ooui.Button(firstButtonText)
			{
				ClassName = "btn " + (firstPressedState ? "btn-primary" : "btn-secondary")
			};
			FirstButton.SetAttribute("type", "button");
			FirstButton.SetAttribute("data-active", firstPressedState.ToString());
			FirstButton.Click += HandleButtonState;
			AppendChild(FirstButton);

			SecondButton = new global::Ooui.Button(secondButtonText)
			{
				ClassName = "btn " + (secondPressedState ? "btn-primary" : "btn-secondary")
			};
			SecondButton.SetAttribute("type", "button");
			SecondButton.SetAttribute("data-active", secondPressedState.ToString());
			SecondButton.Click += HandleButtonState;
			AppendChild(SecondButton);

			HandleButtonState(null, null);
		}

		public bool FirstButtonActive => !IsDisabled && Convert.ToBoolean(FirstButton.GetAttribute("data-active"));
		public bool SecondButtonActive => !IsDisabled && Convert.ToBoolean(SecondButton.GetAttribute("data-active"));

		private void HandleButtonState(object sender, TargetEventArgs args)
		{
			bool firstButtonOldState = FirstButtonActive;
			bool secondButtonOldState = SecondButtonActive;
			global::Ooui.Button button = (global::Ooui.Button)sender;
			if (button == FirstButton && SecondButtonActive)
			{
				FirstButton.SetAttribute("data-active", !FirstButtonActive);
				SecondButton.SetAttribute("data-active", !SecondButtonActive);
			}
			if (button == SecondButton && FirstButtonActive)
			{
				FirstButton.SetAttribute("data-active", !FirstButtonActive);
				SecondButton.SetAttribute("data-active", !SecondButtonActive);
			}
			if (FirstButtonActive)
			{
				if (FirstButton.ClassName.Contains("btn-secondary"))
				{
					FirstButton.ClassName = FirstButton.ClassName.Replace("btn-secondary", "btn-primary");
				}
			}
			else
			{
				if (FirstButton.ClassName.Contains("btn-primary"))
				{
					FirstButton.ClassName = FirstButton.ClassName.Replace("btn-primary", "btn-secondary");
				}
			}
			if (SecondButtonActive)
			{
				if (SecondButton.ClassName.Contains("btn-secondary"))
				{
					SecondButton.ClassName = SecondButton.ClassName.Replace("btn-secondary", "btn-primary");
				}
			}
			else
			{
				if (SecondButton.ClassName.Contains("btn-primary"))
				{
					SecondButton.ClassName = SecondButton.ClassName.Replace("btn-primary", "btn-secondary");
				}
			}
			if (firstButtonOldState != FirstButtonActive)
			{
				FirstButtonStateChange?.Invoke(FirstButton, new ButtonChangeEventHandlerArgs(firstButtonOldState, FirstButtonActive));
			}

			if (secondButtonOldState != SecondButtonActive)
			{
				SecondButtonStateChange?.Invoke(SecondButton, new ButtonChangeEventHandlerArgs(secondButtonOldState, SecondButtonActive));
			}
		}

		public event ButtonChangeEventHandler FirstButtonStateChange;

		public event ButtonChangeEventHandler SecondButtonStateChange;
	}
}

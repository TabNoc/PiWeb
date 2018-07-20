using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.UiComponents.FormControl
{
	public class ToggleButtonGroup : InputGroupControl
	{
		private readonly string _activeButtonClassName;
		private readonly string _inActiveButtonClassName;

		public ToggleButtonGroup(StylingColor activeButtonStylingColor, StylingColor inactiveButtonStylingColor)
		{
			ClassName = "btn-group";
			SetAttribute("role", "group");
			_activeButtonClassName = "btn-" + Enum.GetName(typeof(StylingColor), activeButtonStylingColor).ToLower();
			_inActiveButtonClassName = "btn-" + Enum.GetName(typeof(StylingColor), inactiveButtonStylingColor).ToLower();
		}

		private readonly List<Button> _buttons = new List<Button>();

		public void AddToggleButton(string buttonText, bool pressedState)
		{
			Button button = new Button(buttonText)
			{
				ClassName = "btn " + (pressedState ? _activeButtonClassName : _inActiveButtonClassName)
			};
			button.SetAttribute("type", "button");
			button.Click += (sender, args) =>
			{
				if (button.ClassName.Contains(_activeButtonClassName))
				{
					button.ClassName = button.ClassName.Replace(_activeButtonClassName, _inActiveButtonClassName);
				}
				else
				{
					button.ClassName = button.ClassName.Replace(_inActiveButtonClassName, _activeButtonClassName);
				}
			};
			_buttons.Add(button);
			AppendChild(button);
		}

		public List<Button> GetActiveButtons()
		{
			return _buttons.Where(button => button.ClassName.Contains(_activeButtonClassName)).ToList();
		}
	}
}

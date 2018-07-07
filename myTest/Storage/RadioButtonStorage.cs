using Ooui;
using System;
using System.Collections.Generic;
using TabNoc.Ooui.HtmlElements;

namespace TabNoc.Ooui.Storage
{
	internal static class RadioButtonStorage
	{
		private static readonly Dictionary<string, bool> RadioButtonGroupStorage = new Dictionary<string, bool>();
		private static readonly Dictionary<string, List<RadioButton>> RadioButtonDictionary = new Dictionary<string, List<RadioButton>>();

		public static void ChangeRadioButtonState(string radioButtonGroupName, RadioButton sender, TargetEventArgs targetEventArgs)
		{
			SetRadioButtonState(radioButtonGroupName, sender, true);
		}

		public static bool GetRadioButtonState(string radioButtonGroupName, RadioButton radioButton)
		{
			return radioButton.ClassName.Contains(" btn-primary");
		}

		public static void SetRadioButtonState(string radioButtonGroupName, RadioButton radioButton, bool value)
		{
			if (value == true)
			{
				foreach (RadioButton radio in RadioButtonDictionary[radioButtonGroupName])
				{
					radio.ClassName = radio.ClassName.Replace(" btn-primary", "");
				}
				if (radioButton.ClassName.Contains(" btn-primary") == false)
				{
					radioButton.ClassName += " btn-primary";
				}
			}
			else
			{
				radioButton.ClassName = radioButton.ClassName.Replace(" btn-primary", "");
			}
		}

		public static void UnRegisterRadioButton(string radioButtonGroupName, RadioButton radioButton)
		{
			if (RadioButtonDictionary.ContainsKey(radioButtonGroupName) == false)
			{
				throw new InvalidOperationException("Es kann kein RadioButton entfernt werden, welcher zuvor nicht hinzugefügt wurde");
			}
			RadioButtonDictionary[radioButtonGroupName].Remove(radioButton);
			if (RadioButtonDictionary[radioButtonGroupName].Count == 0)
			{
				RadioButtonDictionary.Remove(radioButtonGroupName);
				RadioButtonGroupStorage.Remove(radioButtonGroupName);
			}
		}

		public static void RegisterRadioButton(string radioButtonGroupName, RadioButton radioButton, bool startValue)
		{
			if (RadioButtonDictionary.ContainsKey(radioButtonGroupName) == false)
			{
				RadioButtonDictionary.Add(radioButtonGroupName, new List<RadioButton>());
			}
			RadioButtonDictionary[radioButtonGroupName].Add(radioButton);

			#region checkStartValue

			if (startValue)
			{
				if (RadioButtonGroupStorage.ContainsKey(radioButtonGroupName) == false)
				{
					RadioButtonGroupStorage.Add(radioButtonGroupName, true);
				}
				else
				{
					if (RadioButtonGroupStorage[radioButtonGroupName] == true)
					{
						throw new InvalidOperationException($"Die radioButton Gruppe \"{radioButtonGroupName}\" hat bereits ein aktiviertes Element!");
					}
					else
					{
						RadioButtonGroupStorage[radioButtonGroupName] = true;
					}
				}
			}

			#endregion checkStartValue
		}
	}
}

using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.MyOoui.HtmlElements;
using TabNoc.MyOoui.UiComponents;

namespace TabNoc.MyOoui.Storage
{
	internal static class RadioButtonStorage
	{
		private static readonly Dictionary<string, RadioButtonStorageEntry> RadioButtonDictionary = new Dictionary<string, RadioButtonStorageEntry>();

		public static void ChangeRadioButtonState(string radioButtonGroupName, RadioButton sender, TargetEventArgs targetEventArgs)
		{
			SetRadioButtonState(radioButtonGroupName, sender, true);
		}

		public static bool GetRadioButtonState(string radioButtonGroupName, RadioButton radioButton)
		{
			return radioButton.ClassName.Contains(" btn-primary");
		}

		public static void RegisterRadioButton(string radioButtonGroupName, RadioButton radioButton, bool startValue)
		{
			// Spätestens nach 24 Stunden müssen die Radiobuttons gelöscht werden (Speicher)
			List<string> removeList = RadioButtonDictionary.Where(pair => pair.Value.AddedTime > DateTime.Now.AddDays(1)).Select(pair => pair.Key).ToList();
			if (removeList.Count > 0)
			{
				Logging.WriteLog("Develop", "Error", "Es wurden Einträge im RadioButtonDictionary gefunden die älter aus 24h sind");
			}
			foreach (string removeEntry in removeList)
			{
				RadioButtonDictionary.Remove(removeEntry);
			}

			if (RadioButtonDictionary.ContainsKey(radioButtonGroupName) == false)
			{
				RadioButtonDictionary.Add(radioButtonGroupName, new RadioButtonStorageEntry(new List<RadioButton>(), false));
			}
			RadioButtonDictionary[radioButtonGroupName].RadioButtons.Add(radioButton);

			#region checkStartValue

			if (startValue)
			{
				if (RadioButtonDictionary[radioButtonGroupName].HasActiveElement == true)
				{
					throw new InvalidOperationException($"Die radioButton Gruppe \"{radioButtonGroupName}\" hat bereits ein aktiviertes Element!");
				}
				else
				{
					RadioButtonDictionary[radioButtonGroupName].HasActiveElement = true;
				}
			}

			#endregion checkStartValue
		}

		public static void SetRadioButtonState(string radioButtonGroupName, RadioButton radioButton, bool value)
		{
			if (value == true)
			{
				foreach (RadioButton radio in RadioButtonDictionary[radioButtonGroupName].RadioButtons)
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
			RadioButtonDictionary[radioButtonGroupName].RadioButtons.Remove(radioButton);
			if (RadioButtonDictionary[radioButtonGroupName].RadioButtons.Count == 0)
			{
				RadioButtonDictionary.Remove(radioButtonGroupName);
			}
		}

		private class RadioButtonStorageEntry
		{
			public DateTime AddedTime;
			public bool HasActiveElement;
			public List<RadioButton> RadioButtons;

			public RadioButtonStorageEntry(List<RadioButton> radioButtons, bool hasActiveElement)
			{
				RadioButtons = radioButtons;
				HasActiveElement = hasActiveElement;
				AddedTime = DateTime.Now;
			}
		}
	}
}

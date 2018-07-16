using Ooui;
using System.Collections.Generic;
using System.Linq;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Storage.WateringWeb.Settings;
using TabNoc.Ooui.UiComponents;
using TabNoc.Ooui.UiComponents.FormControl;
using TabNoc.Ooui.UiComponents.FormControl.InputGroups;
using TabNoc.Ooui.UiComponents.FormControl.InputGroups.Components;
using Button = TabNoc.Ooui.HtmlElements.Button;

namespace TabNoc.Ooui.Pages.WateringWeb.Settings
{
	internal class SettingsPage : StylableElement
	{
		private readonly Dropdown _humidityDropdown;
		private readonly TextInputGroup _humiditySensorTextInputGroup;
		private readonly OverrideInputGroup _overrideInputGroup;
		private readonly PageStorage<SettingsData> _settingsData;

		public SettingsPage(PageStorage<SettingsData> settingsData) : base("div")
		{
			const int labelSize = 100;

			_settingsData = settingsData;

			#region Initialize Grid

			Grid grid = new Grid(this);
			grid.AddStyling(StylingOption.MarginRight, 2);
			grid.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddStyling(StylingOption.MarginTop, 4);
			grid.AddStyling(StylingOption.MarginBottom, 2);

			#endregion Initialize Grid

			#region AutoEnabled

			TwoStateButtonGroup enabledButtonGroup = new TwoStateButtonGroup("Aktiv", "Inaktiv", settingsData.StorageData.Enabled, !settingsData.StorageData.Enabled);
			enabledButtonGroup.FirstButtonStateChange += (sender, args) => settingsData.StorageData.Enabled = args.NewButtonState;
			InputGroup inputGroup = new InputGroup("Automatik", enabledButtonGroup, labelSize);
			inputGroup.AddStyling(StylingOption.MarginBottom, 2);
			grid.AddRow().AppendCollum(inputGroup, autoSize: true);

			#endregion AutoEnabled

			#region WeatherEnabled

			TwoStateButtonGroup weatherEnabledButtonGroup = new TwoStateButtonGroup("Aktiv", "Inaktiv", settingsData.StorageData.WeatherEnabled, !settingsData.StorageData.WeatherEnabled);
			weatherEnabledButtonGroup.FirstButtonStateChange += (sender, args) => settingsData.StorageData.WeatherEnabled = args.NewButtonState;
			InputGroup weatherInputGroup = new InputGroup("Wetter", weatherEnabledButtonGroup, labelSize);
			weatherInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			grid.AddRow().AppendCollum(weatherInputGroup, autoSize: true);

			#endregion WeatherEnabled

			#region Location

			Row locationRow = grid.AddRow();
			locationRow.AddStyling(StylingOption.MarginBottom, 2);
			MultiInputGroup weatherLocationMultiInputGroup = new MultiInputGroup();
			weatherLocationMultiInputGroup.AppendLabel("Standort", labelSize);

			StylableTextInput weatherLocationTextInput = weatherLocationMultiInputGroup.AppendTextInput("Bitte Eintragen...", false);
			weatherLocationTextInput.Value = settingsData.StorageData.LocationName;

			#region Hidden TextInputs

			TextInput weatherLocationChangeTextInput = new TextInput { IsHidden = true };

			locationRow.AppendChild(weatherLocationChangeTextInput);
			TextInput weatherLocationNameChangeTextInput = new TextInput { IsHidden = true };

			locationRow.AppendChild(weatherLocationNameChangeTextInput);

			#endregion Hidden TextInputs

			Button weatherLocationActivateAutocompleteButton = weatherLocationMultiInputGroup.AppendCustomElement(new Button(StylingColor.Secondary, true, Button.ButtonSize.Small, false, "v"), false);
			weatherLocationActivateAutocompleteButton.Click += (sender, args) =>
			{
				weatherLocationActivateAutocompleteButton.IsHidden = true;
				ActivateAutoComplete(weatherLocationTextInput, weatherLocationChangeTextInput, weatherLocationNameChangeTextInput);
			};
			locationRow.AppendCollum(weatherLocationMultiInputGroup, autoSize: true);

			#region Save Button

			Button saveLocationButton = new Button(StylingColor.Success, true, text: "Übernehmen");
			saveLocationButton.Click += (sender, args) =>
			{
				if (weatherLocationChangeTextInput.Value == "")
				{
					weatherLocationTextInput.SetValidation(false, true);
				}
				else
				{
					weatherLocationTextInput.SetValidation(false, false);
					settingsData.StorageData.Location = weatherLocationChangeTextInput.Value;
					settingsData.StorageData.LocationName = weatherLocationNameChangeTextInput.Value;
					weatherLocationTextInput.Value = settingsData.StorageData.LocationName;
				}
			};
			locationRow.AppendCollum(saveLocationButton, autoSize: true);

			#endregion Save Button

			#endregion Location

			#region Override

			_overrideInputGroup = new OverrideInputGroup(_settingsData.StorageData.OverrideValue);
			grid.AddRow().AppendCollum(_overrideInputGroup, autoSize: true);

			#endregion Override

			#region Rename HumiditySensors

			Row humidityRow = grid.AddRow();
			humidityRow.AppendChild(new Heading(3, "Feuchigkeitssensoren Umbenennen"));
			humidityRow.AddNewLine();

			#region Sync Server HumidityList with Storage

			foreach (string humiditySensor in PageStorage<HumiditySensorData>.Instance.StorageData.HumiditySensors)
			{
				if (settingsData.StorageData.HumiditySensors.ContainsKey(humiditySensor) == false)
				{
					settingsData.StorageData.HumiditySensors.Add(humiditySensor, humiditySensor);
				}
			}

			List<string> removeList = new List<string>();
			foreach ((string realSensorName, string _) in settingsData.StorageData.HumiditySensors)
			{
				if (PageStorage<HumiditySensorData>.Instance.StorageData.HumiditySensors.Contains(realSensorName) == false)
				{
					removeList.Add(realSensorName);
				}
			}

			foreach (string s in removeList)
			{
				settingsData.StorageData.HumiditySensors.Remove(s);
			}

			#endregion Sync Server HumidityList with Storage

			_humidityDropdown = new Dropdown(new Button(StylingColor.Secondary, true, widthInPx: 150));
			humidityRow.AppendCollum(_humidityDropdown, autoSize: true);

			foreach (string humiditySensor in PageStorage<HumiditySensorData>.Instance.StorageData.HumiditySensors)
			{
				StylableAnchor stylableAnchor = _humidityDropdown.AddEntry(humiditySensor);
				stylableAnchor.Click += (sender, args) => SelectHumiditySensor(humiditySensor);
			}

			_humiditySensorTextInputGroup = new TextInputGroup("Freundlicher Name", "Bitte Eingeben!");
			humidityRow.AppendCollum(_humiditySensorTextInputGroup, autoSize: true);

			Button button = new Button(StylingColor.Success, true, text: "Übernehmen");
			button.Click += (sender, args) =>
			{
				if (_humidityDropdown.Button.Text != "")
				{
					_settingsData.StorageData.HumiditySensors[_humidityDropdown.Button.Text] = _humiditySensorTextInputGroup.TextInput.Value;
				}
			};

			humidityRow.AppendCollum(button, autoSize: true);
			if (PageStorage<HumiditySensorData>.Instance.StorageData.HumiditySensors.Count > 0)
			{
				SelectHumiditySensor(PageStorage<HumiditySensorData>.Instance.StorageData.HumiditySensors.First());
			}
			else
			{
				humidityRow.IsHidden = true;
			}

			#endregion Rename HumiditySensors
		}

		public void SelectHumiditySensor(string humiditySensor)
		{
			_humidityDropdown.Button.Text = humiditySensor;
			_humiditySensorTextInputGroup.TextInput.Value = _settingsData.StorageData.HumiditySensors[humiditySensor];
		}

		protected override void Dispose(bool disposing)
		{
			_settingsData.StorageData.OverrideValue = _overrideInputGroup.Value;
			_settingsData.Save();
			base.Dispose(disposing);
		}

		private void ActivateAutoComplete(StylableTextInput weatherLocationTextInput, TextInput locationChangeTextInput, TextInput nameChangeTextInput)
		{
			weatherLocationTextInput.ActivateAutocomplete("/settings/WeatherLocations.json", new Dictionary<string, TextInput>()
			{
				{"location", locationChangeTextInput },
				{"name", nameChangeTextInput }
			});
		}
	}
}

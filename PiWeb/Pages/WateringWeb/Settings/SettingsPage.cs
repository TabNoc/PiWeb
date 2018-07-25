using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.MyOoui.Storage;
using TabNoc.MyOoui.UiComponents;
using TabNoc.MyOoui.UiComponents.FormControl;
using TabNoc.MyOoui.UiComponents.FormControl.InputGroups;
using TabNoc.MyOoui.UiComponents.FormControl.InputGroups.Components;
using TabNoc.PiWeb.DataTypes.WateringWeb.Settings;
using Button = TabNoc.MyOoui.HtmlElements.Button;

namespace TabNoc.PiWeb.Pages.WateringWeb.Settings
{
	internal class SettingsPage : StylableElement
	{
		private readonly Dropdown _humidityDropdown;
		private readonly TextInputGroup _humiditySensorTextInputGroup;
		private readonly OverrideInputGroup _overrideInputGroup;
		private readonly PageStorage<SettingsData> _settingsData;

		public SettingsPage(PageStorage<SettingsData> settingsData) : base("div")
		{
			this.AddScriptDependency("/lib/bootstrap3-typeahead.min.js");
			const int labelSize = 115;

			_settingsData = settingsData;

			#region Initialize Grid

			Container wrappingContainer = new Container(this);
			Grid grid = new Grid(wrappingContainer);

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

			#region Autocomplete

			weatherLocationTextInput.ActivateAutocomplete("/settings/WeatherLocations.json", new Dictionary<string, TextInput>()
			{
				{"location", weatherLocationChangeTextInput },
				{"name", weatherLocationNameChangeTextInput }
			});

			#endregion Autocomplete

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

			_overrideInputGroup = new OverrideInputGroup(_settingsData.StorageData.OverrideValue, labelSizeInPx: labelSize);
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

			#region Backend Server Path

			grid.AddRow().AppendCollum(new Heading(3, "Backend Server Schnittstelle einstellen") { ClassName = "text-center mb-4" });
			Row backendServerRow = grid.AddRow();
			backendServerRow.AddNewLine();

			foreach ((string name, BackedProperties backedProperties) in PageStorage<BackendData>.Instance.StorageData.BackedPropertieses)
			{
				backendServerRow.AddNewLine();
				backendServerRow.AppendCollum(CreateBackendCollum(name, backedProperties), autoSize: true);
			}

			#endregion Backend Server Path
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
			PageStorage<BackendData>.Instance.Save();
			base.Dispose(disposing);
		}

		private MultiInputGroup CreateBackendCollum(string name, BackedProperties backedProperties)
		{
			MultiInputGroup backendMultiInputGroup = new MultiInputGroup();
			backendMultiInputGroup.AppendLabel(name, 115 + 80);
			TwoStateButtonGroup backendEnabled = backendMultiInputGroup.AppendCustomElement(new TwoStateButtonGroup("Vom Server", "Als Debug", backedProperties.RequestDataFromBackend, !backedProperties.RequestDataFromBackend), false);
			StylableTextInput backendPath = backendMultiInputGroup.AppendTextInput("Pfad zur WebAPI", startText: backedProperties.DataSourcePath);
			backendMultiInputGroup.AppendValidation("Einstellungen OK", "Einstellungen sind nicht OK", false);
			Button backendSaveSettings = backendMultiInputGroup.AppendCustomElement(new Button(StylingColor.Success, true, text: "Speichern", fontAwesomeIcon: "save"), false);

			backendSaveSettings.Click += (sender, args) =>
			{
				backendPath.SetValidation(false, false);
				if (backendEnabled.FirstButtonActive && Uri.IsWellFormedUriString(backendPath.Value, UriKind.Absolute))
				{
					backendPath.SetValidation(true, false);
					backedProperties.RequestDataFromBackend = backendEnabled.FirstButtonActive;
					backedProperties.SendDataToBackend = backendEnabled.FirstButtonActive;
					backedProperties.DataSourcePath = backendPath.Value;
				}
				else if (backendEnabled.SecondButtonActive)
				{
					backendPath.SetValidation(true, false);
					backedProperties.RequestDataFromBackend = backendEnabled.FirstButtonActive;
					backedProperties.SendDataToBackend = backendEnabled.FirstButtonActive;
				}
				else
				{
					backendPath.SetValidation(false, true);
				}
			};
			backendMultiInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			return backendMultiInputGroup;
		}
	}
}

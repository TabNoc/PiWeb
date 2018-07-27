using Ooui;
using System;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.MyOoui.UiComponents;
using TabNoc.MyOoui.UiComponents.FormControl;
using TabNoc.MyOoui.UiComponents.FormControl.InputGroups;
using TabNoc.PiWeb.DataTypes.WateringWeb.Channels;

namespace TabNoc.PiWeb.Pages.WateringWeb.Channels
{
	internal class ChannelProgrammPage : StylableElement
	{
		private readonly ChannelProgramData _channelProgram;
		private readonly ChannelPage _parentChannelPage;

		#region FormElements

		private readonly TwoStateButtonGroup _activateMasterChannel;
		private readonly MyOoui.HtmlElements.Button _deleteProgrammButton;
		private readonly TextAreaInputGroup _descriptionInputGroup;
		private readonly TextInputGroup _durationInputGroup;
		private readonly TwoStateButtonGroup _programmEnabled;
		private readonly TextInputGroup _programmNameInputGroup;
		private readonly TextInputGroup _startTimeInputGroup;
		private readonly TwoStateButtonGroup _weatherInfo;
		private readonly RadioButtonInputGroup _weekDaysChoosenRadioButtonInputGroup;
		private readonly ToggleButtonGroup _weekdaysChoosenToggleButtonGroup;
		private readonly RadioButtonLabeledInputGroup _weekdaysDiDoRadioButtonLabeledInputGroup;
		private readonly RadioButtonLabeledInputGroup _weekdaysMoMiFrRadioButtonLabeledInputGroup;
		private readonly RadioButtonLabeledInputGroup _weekdaysSaSoRadioButtonLabeledInputGroup;

		#endregion FormElements

		public ChannelProgrammPage(ChannelProgramData channelProgram, ChannelPage parentChannelPage, bool isMasterChannel) : base("div")
		{
			const int labelSize = 235;
			_channelProgram = channelProgram;
			_parentChannelPage = parentChannelPage;

			#region Initialize Grid

			Grid grid = new Grid(this);
			grid.AddStyling(StylingOption.MarginRight, 2);
			grid.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddStyling(StylingOption.MarginTop, 2);
			grid.AddStyling(StylingOption.MarginBottom, 2);

			#endregion Initialize Grid

			#region TextInputGroup ProgrammName

			_programmNameInputGroup = new TextInputGroup("ProgrammName", "N/A", labelSize, centeredText: true);
			_programmNameInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_programmNameInputGroup.TextInput.Value = channelProgram.Name;
			_deleteProgrammButton = new MyOoui.HtmlElements.Button(StylingColor.Danger, asOutline: true, text: "Programm Löschen", fontAwesomeIcon: "trash");
			_deleteProgrammButton.Click += DeleteProgrammButtonOnClick;
			_programmNameInputGroup.AddFormElement(_deleteProgrammButton);
			grid.AddRow().AppendCollum(_programmNameInputGroup);

			#endregion TextInputGroup ProgrammName

			#region InputGroup > TwoStateButtonGroup ProgrammStatus

			MultiInputGroup programmEnabledMultiInputGroup = new MultiInputGroup();
			programmEnabledMultiInputGroup.AppendLabel("Programmstatus", labelSize);
			programmEnabledMultiInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_programmEnabled = programmEnabledMultiInputGroup.AppendCustomElement(new TwoStateButtonGroup("Aktiv", "Inaktiv", channelProgram.Enabled, !channelProgram.Enabled), false);
			grid.AddRow().AppendCollum(programmEnabledMultiInputGroup, autoSize: true);

			#endregion InputGroup > TwoStateButtonGroup ProgrammStatus

			#region TextInputGroup StartZeit

			_startTimeInputGroup = new TextInputGroup("StartZeit", "N/A", labelSize, "", "Das angegebene Zeitformat passt nicht. Bitte Zeiten im Format  hh:mm:ss  angeben.");
			_startTimeInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_startTimeInputGroup.TextInput.Value = channelProgram.StartTime.ToString();
			grid.AddRow().AppendCollum(_startTimeInputGroup);

			#endregion TextInputGroup StartZeit

			#region TextInputGroup Dauer

			_durationInputGroup = new TextInputGroup("Dauer", "N/A", labelSize, "", "Das angegebene Zeitformat passt nicht. Bitte Zeiten im Format  hh:mm:ss  angeben.");
			_durationInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_durationInputGroup.TextInput.Value = channelProgram.Duration.ToString();
			grid.AddRow().AppendCollum(_durationInputGroup);

			#endregion TextInputGroup Dauer

			#region Row Declaration

			Row downRow = new Row();
			grid.AddRow().AppendCollum(downRow);
			Row weekDaysRow = new Row();
			downRow.AppendCollum(weekDaysRow, sizing: 4);
			Row otherRow = new Row();
			downRow.AppendCollum(otherRow, sizing: 8);

			#endregion Row Declaration

			#region Weekdays

			_weekdaysMoMiFrRadioButtonLabeledInputGroup = new RadioButtonLabeledInputGroup(GetCheckedWeekdays(channelProgram.ChoosenWeekdays) == 1, "Mo, Mi, Fr");
			//_weekdaysMoMiFrRadioButtonLabeledInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_weekdaysMoMiFrRadioButtonLabeledInputGroup.AddStyling(StylingOption.MarginLeft, 2);
			weekDaysRow.AppendCollum(_weekdaysMoMiFrRadioButtonLabeledInputGroup);
			weekDaysRow.AddNewLine();

			_weekdaysDiDoRadioButtonLabeledInputGroup = new RadioButtonLabeledInputGroup(GetCheckedWeekdays(channelProgram.ChoosenWeekdays) == 2, "Di, Do", _weekdaysMoMiFrRadioButtonLabeledInputGroup.RadioButtonGroupName);
			//_weekdaysDiDoRadioButtonLabeledInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_weekdaysDiDoRadioButtonLabeledInputGroup.AddStyling(StylingOption.MarginLeft, 2);
			weekDaysRow.AppendCollum(_weekdaysDiDoRadioButtonLabeledInputGroup);
			weekDaysRow.AddNewLine();

			_weekdaysSaSoRadioButtonLabeledInputGroup = new RadioButtonLabeledInputGroup(GetCheckedWeekdays(channelProgram.ChoosenWeekdays) == 3, "Sa, So", _weekdaysMoMiFrRadioButtonLabeledInputGroup.RadioButtonGroupName);
			//_weekdaysSaSoRadioButtonLabeledInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_weekdaysSaSoRadioButtonLabeledInputGroup.AddStyling(StylingOption.MarginLeft, 2);
			weekDaysRow.AppendCollum(_weekdaysSaSoRadioButtonLabeledInputGroup);
			weekDaysRow.AddNewLine();

			_weekdaysChoosenToggleButtonGroup = new ToggleButtonGroup(StylingColor.Primary, StylingColor.Secondary);
			_weekdaysChoosenToggleButtonGroup.AddToggleButton("Mo", (channelProgram.ChoosenWeekdays & ChannelProgramData.Weekdays.Montag) != ChannelProgramData.Weekdays.None);
			_weekdaysChoosenToggleButtonGroup.AddToggleButton("Di", (channelProgram.ChoosenWeekdays & ChannelProgramData.Weekdays.Dienstag) != ChannelProgramData.Weekdays.None);
			_weekdaysChoosenToggleButtonGroup.AddToggleButton("Mi", (channelProgram.ChoosenWeekdays & ChannelProgramData.Weekdays.Mittwoch) != ChannelProgramData.Weekdays.None);
			_weekdaysChoosenToggleButtonGroup.AddToggleButton("Do", (channelProgram.ChoosenWeekdays & ChannelProgramData.Weekdays.Donnerstag) != ChannelProgramData.Weekdays.None);
			_weekdaysChoosenToggleButtonGroup.AddToggleButton("Fr", (channelProgram.ChoosenWeekdays & ChannelProgramData.Weekdays.Freitag) != ChannelProgramData.Weekdays.None);
			_weekdaysChoosenToggleButtonGroup.AddToggleButton("Sa", (channelProgram.ChoosenWeekdays & ChannelProgramData.Weekdays.Samstag) != ChannelProgramData.Weekdays.None);
			_weekdaysChoosenToggleButtonGroup.AddToggleButton("So", (channelProgram.ChoosenWeekdays & ChannelProgramData.Weekdays.Sonntag) != ChannelProgramData.Weekdays.None);
			_weekDaysChoosenRadioButtonInputGroup = new RadioButtonInputGroup(GetCheckedWeekdays(channelProgram.ChoosenWeekdays) == 4, _weekdaysChoosenToggleButtonGroup, _weekdaysMoMiFrRadioButtonLabeledInputGroup.RadioButtonGroupName, false);
			_weekDaysChoosenRadioButtonInputGroup.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddRow().AppendCollum(_weekDaysChoosenRadioButtonInputGroup, autoSize: true);

			#endregion Weekdays

			#region InputGroup > TwoStateButtonGroup WetterInfos

			MultiInputGroup weatherEnabledMultiInputGroup = new MultiInputGroup();
			weatherEnabledMultiInputGroup.AppendLabel("WetterInfos verwenden", labelSize);
			weatherEnabledMultiInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_weatherInfo = weatherEnabledMultiInputGroup.AppendCustomElement(new TwoStateButtonGroup("Aktiv", "Inaktiv", channelProgram.ActivateWeatherInfo, !channelProgram.ActivateWeatherInfo), false);
			otherRow.AppendCollum(weatherEnabledMultiInputGroup, autoSize: true);

			#endregion InputGroup > TwoStateButtonGroup WetterInfos

			otherRow.AddNewLine();

			#region InputGroup > TwoStateButtonGroup Master Kanal

			MultiInputGroup activateMasterMultiInputGroup = new MultiInputGroup();
			activateMasterMultiInputGroup.AppendLabel("Master Kanal auch Einschalten", labelSize);
			_activateMasterChannel = activateMasterMultiInputGroup.AppendCustomElement(new TwoStateButtonGroup("Aktiv", "Inaktiv", channelProgram.EnableMasterChannel, !channelProgram.EnableMasterChannel), false);
			otherRow.AppendCollum(activateMasterMultiInputGroup, autoSize: true);

			#endregion InputGroup > TwoStateButtonGroup Master Kanal

			#region TextAreaInputGroup Notizen

			_descriptionInputGroup = new TextAreaInputGroup("Notizen", "", labelSize);
			_descriptionInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_descriptionInputGroup.AddStyling(StylingOption.MarginTop, 2);
			_descriptionInputGroup.TextArea.Value = channelProgram.Description;
			grid.AddRow().AppendCollum(_descriptionInputGroup);

			#endregion TextAreaInputGroup Notizen

			if (isMasterChannel)
			{
				_activateMasterChannel.IsDisabled = true;
			}
		}

		public void Save()
		{
			_channelProgram.ActivateWeatherInfo = _weatherInfo.FirstButtonActive;
			_channelProgram.ChoosenWeekdays = GetWeekdays();

			string[] durationStrings = _durationInputGroup.TextInput.Value.Split(":");
			if (durationStrings.Length != 3 || _durationInputGroup.TextInput.Value.Contains("."))
			{
				_durationInputGroup.SetValidation(false, true);
			}
			else
			{
				try
				{
					_channelProgram.Duration = new TimeSpan(int.Parse(durationStrings[0]), int.Parse(durationStrings[1]), int.Parse(durationStrings[2]));
					_durationInputGroup.TextInput.SetValidation(false, false);
					_durationInputGroup.TextInput.Value = _channelProgram.Duration.ToString();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					_durationInputGroup.SetValidation(false, true);
				}
			}

			_channelProgram.EnableMasterChannel = _activateMasterChannel.FirstButtonActive;
			_channelProgram.Enabled = _programmEnabled.FirstButtonActive;

			string[] startTimeStrings = _startTimeInputGroup.TextInput.Value.Split(":");
			if (startTimeStrings.Length != 3 || _startTimeInputGroup.TextInput.Value.Contains("."))
			{
				_startTimeInputGroup.TextInput.SetValidation(false, true);
			}
			else
			{
				try
				{
					_channelProgram.StartTime = new TimeSpan(int.Parse(startTimeStrings[0]), int.Parse(startTimeStrings[1]), int.Parse(startTimeStrings[2]));
					_startTimeInputGroup.TextInput.SetValidation(false, false);
					_startTimeInputGroup.TextInput.Value = _channelProgram.StartTime.ToString();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					_startTimeInputGroup.SetValidation(false, true);
				}
			}

			_channelProgram.Description = _descriptionInputGroup.TextArea.Value;
			_channelProgram.Name = _programmNameInputGroup.TextInput.Value;

			_parentChannelPage.ApplyName(_channelProgram);
		}

		private void DeleteProgrammButtonOnClick(object sender, TargetEventArgs e)
		{
			const string confirmMessage = "Wirklich Löschen";
			if (_deleteProgrammButton.Text != confirmMessage)
			{
				_deleteProgrammButton.Text = confirmMessage;
				return;
			}
			else
			{
				_parentChannelPage.RemoveProgramm(this, _channelProgram);
			}
		}

		private int GetCheckedWeekdays(ChannelProgramData.Weekdays channelProgramChoosenWeekdays)
		{
			switch (channelProgramChoosenWeekdays)
			{
				case ChannelProgramData.Weekdays.Montag | ChannelProgramData.Weekdays.Mittwoch | ChannelProgramData.Weekdays.Freitag:
					return 1;

				case ChannelProgramData.Weekdays.Dienstag | ChannelProgramData.Weekdays.Donnerstag:
					return 2;

				case ChannelProgramData.Weekdays.Samstag | ChannelProgramData.Weekdays.Sonntag:
					return 3;

				default:
					return 4;
			}
		}

		private ChannelProgramData.Weekdays GetWeekday(string shortWeekday)
		{
			switch (shortWeekday)
			{
				case "Mo":
					return ChannelProgramData.Weekdays.Montag;

				case "Di":
					return ChannelProgramData.Weekdays.Dienstag;

				case "Mi":
					return ChannelProgramData.Weekdays.Mittwoch;

				case "Do":
					return ChannelProgramData.Weekdays.Donnerstag;

				case "Fr":
					return ChannelProgramData.Weekdays.Freitag;

				case "Sa":
					return ChannelProgramData.Weekdays.Samstag;

				case "So":
					return ChannelProgramData.Weekdays.Sonntag;
			}

			throw new IndexOutOfRangeException("Der angegebene String ist kein Wochentag");
		}

		private ChannelProgramData.Weekdays GetWeekdays()
		{
			if (_weekdaysDiDoRadioButtonLabeledInputGroup.Checked == true)
			{
				return (ChannelProgramData.Weekdays.Dienstag | ChannelProgramData.Weekdays.Donnerstag);
			}
			else if (_weekdaysMoMiFrRadioButtonLabeledInputGroup.Checked == true)
			{
				return (ChannelProgramData.Weekdays.Montag | ChannelProgramData.Weekdays.Mittwoch | ChannelProgramData.Weekdays.Freitag);
			}
			else if (_weekdaysSaSoRadioButtonLabeledInputGroup.Checked == true)
			{
				return (ChannelProgramData.Weekdays.Samstag | ChannelProgramData.Weekdays.Sonntag);
			}
			else if (_weekDaysChoosenRadioButtonInputGroup.Checked == true)

			{
				ChannelProgramData.Weekdays weekdays = ChannelProgramData.Weekdays.None;
				foreach (Button activeButton in _weekdaysChoosenToggleButtonGroup.GetActiveButtons())
				{
					weekdays |= GetWeekday(activeButton.Text);
				}

				return weekdays;
			}
			else
			{
				throw new IndexOutOfRangeException("Es wurde kein RadioButton aktiviert!");
			}
		}
	}
}

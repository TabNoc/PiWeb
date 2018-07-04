﻿using Ooui;
using System;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Storage;
using TabNoc.Ooui.UiComponents;
using TabNoc.Ooui.UiComponents.FormControl;
using TabNoc.Ooui.UiComponents.FormControl.InputGroups;

namespace TabNoc.Ooui.Pages
{
	internal class ChannelProgrammPage : StylableElement
	{
		private readonly ChannelProgramData _channelProgram;
		private readonly ChannelPage _parentChannelPage;

		#region FormElements

		private readonly TextInputGroup _programmNameInputGroup;
		private readonly HtmlElements.Button _deleteProgrammButton;
		private readonly TwoStateButtonGroup _programmEnabled;
		private readonly TextInputGroup _startTimeInputGroup;
		private readonly TextInputGroup _durationInputGroup;
		private readonly RadioButtonLabeledInputGroup _weekdaysMoMiFrRadioButtonLabeledInputGroup;
		private readonly RadioButtonLabeledInputGroup _weekdaysDiDoRadioButtonLabeledInputGroup;
		private readonly RadioButtonLabeledInputGroup _weekdaysSaSoRadioButtonLabeledInputGroup;
		private readonly RadioButtonInputGroup _weekDaysChoosenRadioButtonInputGroup;
		private readonly ToggleButtonGroup _weekdaysChoosenToggleButtonGroup;
		private readonly TwoStateButtonGroup _weatherInfo;
		private readonly TwoStateButtonGroup _activateMasterChannel;
		private readonly TextAreaInputGroup _descriptionInputGroup;

		#endregion FormElements

		public ChannelProgrammPage(ChannelProgramData channelProgram, ChannelPage parentChannelPage, bool isMasterChannel) : base("div")
		{
			const int labelSize = 235;

			#region Initialize Grid

			SetBorder(BorderKind.Rounded, StylingColor.Secondary);
			_channelProgram = channelProgram;
			_parentChannelPage = parentChannelPage;
			Grid grid = new Grid();
			grid.AddStyling(StylingOption.MarginRight, 2);
			grid.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddStyling(StylingOption.MarginTop, 2);
			grid.AddStyling(StylingOption.MarginBottom, 2);

			#endregion Initialize Grid

			#region TextInputGroup ProgrammName

			_programmNameInputGroup = new TextInputGroup("ProgrammName", "N/A", labelSize);
			_programmNameInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_programmNameInputGroup.TextInput.Value = channelProgram.Name;
			_deleteProgrammButton = new HtmlElements.Button(StylingColor.Danger, text: "Programm Löschen");
			_deleteProgrammButton.Click += DeleteProgrammButtonOnClick;
			_programmNameInputGroup.AddFormElement(_deleteProgrammButton);
			grid.AddRow().AppendCollum(_programmNameInputGroup);

			#endregion TextInputGroup ProgrammName

			#region InputGroup > TwoStateButtonGroup ProgrammStatus

			_programmEnabled = new TwoStateButtonGroup("Aktiv", "Inaktiv", channelProgram.Enabled, !channelProgram.Enabled);
			InputGroup programmEnabledInputGroup = new InputGroup("Programmstatus", _programmEnabled, labelSize);
			programmEnabledInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			grid.AddRow().AppendCollum(programmEnabledInputGroup, autoSize: true);

			#endregion InputGroup > TwoStateButtonGroup ProgrammStatus

			#region TextInputGroup StartZeit

			_startTimeInputGroup = new TextInputGroup("StartZeit", "N/A", labelSize);
			_startTimeInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_startTimeInputGroup.TextInput.Value = channelProgram.StartDateTime.ToLongTimeString();
			grid.AddRow().AppendCollum(_startTimeInputGroup);

			#endregion TextInputGroup StartZeit

			#region TextInputGroup Dauer

			_durationInputGroup = new TextInputGroup("Dauer", "N/A", labelSize);
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
			_weekDaysChoosenRadioButtonInputGroup = new RadioButtonInputGroup(GetCheckedWeekdays(channelProgram.ChoosenWeekdays) == 4, _weekdaysChoosenToggleButtonGroup, _weekdaysMoMiFrRadioButtonLabeledInputGroup.RadioButtonGroupName);
			_weekDaysChoosenRadioButtonInputGroup.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddRow().AppendCollum(_weekDaysChoosenRadioButtonInputGroup, autoSize: true);

			#endregion Weekdays

			#region InputGroup > TwoStateButtonGroup WetterInfos

			_weatherInfo = new TwoStateButtonGroup("Aktiv", "Inaktiv", channelProgram.ActivateWeatherInfo, !channelProgram.ActivateWeatherInfo);
			InputGroup idontKnowThatInputGroup = new InputGroup("WetterInfos verwenden", _weatherInfo, labelSize);
			idontKnowThatInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			otherRow.AppendCollum(idontKnowThatInputGroup, autoSize: true);
			otherRow.AddNewLine();

			#endregion InputGroup > TwoStateButtonGroup WetterInfos

			#region InputGroup > TwoStateButtonGroup Master Kanal

			_activateMasterChannel = new TwoStateButtonGroup("Aktiv", "Inaktiv", channelProgram.EnableMasterChannel, !channelProgram.EnableMasterChannel);
			InputGroup activateMasterChannelInputGroup = new InputGroup("Master Kanal auch Einschalten", _activateMasterChannel, labelSize);
			//activateMasterChannelInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			otherRow.AppendCollum(activateMasterChannelInputGroup, autoSize: true);

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

			AppendChild(grid);
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

		public void Save()
		{
			_channelProgram.ActivateWeatherInfo = _weatherInfo.FirstButtonActive;
			_channelProgram.ChoosenWeekdays = GetWeekdays();
			string[] durationStrings = _durationInputGroup.TextInput.Value.Split(":");
			_channelProgram.Duration = new TimeSpan(int.Parse(durationStrings[0]), int.Parse(durationStrings[1]), int.Parse(durationStrings[2]));
			_channelProgram.EnableMasterChannel = _activateMasterChannel.FirstButtonActive;
			_channelProgram.Enabled = _programmEnabled.FirstButtonActive;
			_channelProgram.StartDateTime = DateTime.Parse(_startTimeInputGroup.TextInput.Value);
			_channelProgram.Description = _descriptionInputGroup.TextArea.Value;
			_channelProgram.Name = _programmNameInputGroup.TextInput.Value;
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
	}
}

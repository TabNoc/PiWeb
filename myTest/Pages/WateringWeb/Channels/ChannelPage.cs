using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Storage.WateringWeb.Channels;
using TabNoc.Ooui.Storage.WateringWeb.Settings;
using TabNoc.Ooui.UiComponents;
using TabNoc.Ooui.UiComponents.FormControl;
using TabNoc.Ooui.UiComponents.FormControl.InputGroups;
using Button = TabNoc.Ooui.HtmlElements.Button;

namespace TabNoc.Ooui.Pages.WateringWeb.Channels
{
	internal class ChannelPage : StylableElement
	{
		private readonly ChannelData _channel;
		private readonly TextInputGroup _channelNameInputGroup;
		private readonly List<ChannelProgrammPage> _channelProgrammPages = new List<ChannelProgrammPage>();
		private readonly HtmlElements.Button _deleteChannelButton;
		private readonly bool _isMasterChannel;
		private readonly ChannelsPage _parentChannelsPage;
		private readonly Dictionary<ChannelProgramData, Anchor> _tabDictionary = new Dictionary<ChannelProgramData, Anchor>();
		private readonly TabNavigation _tabNavigation;
		private Dropdown _humiditySensorDropdown;
		private TwoStateButtonGroup _humiditySensorEnabledTwoStateButtonGroup;

		public ChannelPage(ChannelData channel, ChannelsPage parentChannelsPage, bool isMasterChannel) : base("div")
		{
			_channel = channel;
			_parentChannelsPage = parentChannelsPage;
			_isMasterChannel = isMasterChannel;
			SetBorder(BorderKind.Rounded, StylingColor.Secondary);

			#region Initialize Grid

			Grid grid = new Grid(this);
			grid.AddStyling(StylingOption.MarginRight, 2);
			grid.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddStyling(StylingOption.MarginTop, 4);
			grid.AddStyling(StylingOption.MarginBottom, 2);

			#endregion Initialize Grid

			#region TextInputGroup ProgrammName

			_channelNameInputGroup = new TextInputGroup("KanalName", "N/A", centeredText: true);
			_channelNameInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_channelNameInputGroup.TextInput.Value = channel.Name;
			if (!isMasterChannel)
			{
				_deleteChannelButton = new HtmlElements.Button(StylingColor.Danger, asOutline: true, text: "Kanal Löschen", fontAwesomeIcon: "trash");
				_deleteChannelButton.Click += DeleteChannelButtonOnClick;
				_channelNameInputGroup.AddFormElement(_deleteChannelButton);
			}
			else
			{
				_channelNameInputGroup.TextInput.IsDisabled = true;
			}
			grid.AddRow().AppendCollum(_channelNameInputGroup);

			#endregion TextInputGroup ProgrammName

			#region add TabNavigate

			_tabNavigation = new TabNavigation(true, true);
			_tabNavigation.AddButton.Click += (sender, args) =>
			{
				ChannelProgramData channelProgramData = ChannelProgramData.CreateNew(channel.ProgramList.Max(data => int.TryParse(data.Name, out int parsedInt) ? parsedInt : 1) + 1);
				channel.ProgramList.Add(channelProgramData);
				ChannelProgrammPage channelProgrammPage = new ChannelProgrammPage(channelProgramData, this, isMasterChannel);
				_channelProgrammPages.Add(channelProgrammPage);
				_tabDictionary.Add(channelProgramData, _tabNavigation.AddTab(channelProgramData.Id.ToString(), channelProgrammPage, channelProgramData.Id == 1));
			};
			grid.AddRow().AppendCollum(_tabNavigation);

			#endregion add TabNavigate

			#region add ChannelProgrammPages

			foreach (ChannelProgramData channelProgramData in channel.ProgramList)
			{
				ChannelProgrammPage channelProgrammPage = new ChannelProgrammPage(channelProgramData, this, isMasterChannel);
				_channelProgrammPages.Add(channelProgrammPage);
				_tabDictionary.Add(channelProgramData, _tabNavigation.AddTab(channelProgramData.Id.ToString(), channelProgrammPage, channelProgramData.Id == 1));
				ApplyName(channelProgramData);
			}

			#endregion add ChannelProgrammPages

			#region AddHumiditySensor

			AddHumiditySensor(channel, grid);

			_humiditySensorDropdown.Button.IsDisabled = _isMasterChannel;
			_humiditySensorEnabledTwoStateButtonGroup.IsDisabled = _isMasterChannel;

			#endregion AddHumiditySensor

			#region SaveChannel Button

			Button saveButton = new Button(StylingColor.Success, true, Button.ButtonSize.Normal, false, "Speichern", fontAwesomeIcon: "save");
			saveButton.AddStyling(StylingOption.MarginTop, 4);
			saveButton.AddStyling(StylingOption.MarginLeft, 4);
			saveButton.AddStyling(StylingOption.MarginBottom, 1);
			saveButton.AddStyling(StylingOption.PaddingLeft, 5);
			saveButton.AddStyling(StylingOption.PaddingRight, 5);
			saveButton.Click += SaveButton_Click;
			grid.AddRow().AppendCollum(saveButton);

			#endregion SaveChannel Button
		}

		public void ApplyName(ChannelProgramData channelProgram)
		{
			_tabDictionary[channelProgram].Text = channelProgram.Name.Substring(0, Math.Min(channelProgram.Name.Length, 3));
		}

		public void RemoveProgramm(ChannelProgrammPage channelProgrammPage, ChannelProgramData channelProgram)
		{
			_channelProgrammPages.Remove(channelProgrammPage);
			_channel.ProgramList.Remove(channelProgram);
			_tabNavigation.RemoveTab(channelProgram.Id.ToString(), channelProgrammPage);
		}

		private void AddHumiditySensor(ChannelData channel, Grid grid)
		{
			List<StylableAnchor> humiditySensorEntries = new List<StylableAnchor>();

			MultiInputGroup humiditySensorMultiInputGroup = new MultiInputGroup();
			humiditySensorMultiInputGroup.AddStyling(StylingOption.MarginTop, 4);
			humiditySensorMultiInputGroup.AppendLabel("Feuchtigkeitssensor");
			_humiditySensorEnabledTwoStateButtonGroup = new TwoStateButtonGroup("Aktiv", "Inaktiv", channel.HumiditySensorEnabled, !channel.HumiditySensorEnabled);
			humiditySensorMultiInputGroup.AppendCustomElement(_humiditySensorEnabledTwoStateButtonGroup, false);

			_humiditySensorDropdown = new Dropdown(new Button(StylingColor.Secondary, true, text: "N/A"));
			_humiditySensorDropdown.Button.SetAttribute("data-realName", channel.HumiditySensor);
			Anchor humiditySensorNone = _humiditySensorDropdown.AddEntry("Ohne", _humiditySensorDropdown.Button.GetAttribute("data-realName").ToString() == "");
			humiditySensorNone.Click += (sender, args) =>
			{
				_humiditySensorDropdown.Button.SetAttribute("data-realName", "");
				UpdateHumiditySensorDropDown(humiditySensorEntries, humiditySensorNone);
			};
			_humiditySensorDropdown.AddDivider();
			foreach ((string realSensorName, string customSensorName) in PageStorage<SettingsData>.Instance.StorageData.HumiditySensors)
			{
				StylableAnchor humiditySensorEntry = _humiditySensorDropdown.AddEntry(customSensorName, _humiditySensorDropdown.Button.GetAttribute("data-realName").ToString() == realSensorName);
				humiditySensorEntry.SetToolTip(ToolTipLocation.Right, realSensorName);
				humiditySensorEntry.Click += (sender, args) =>
				{
					_humiditySensorDropdown.Button.SetAttribute("data-realName", realSensorName);
					UpdateHumiditySensorDropDown(humiditySensorEntries, humiditySensorNone);
				};
				humiditySensorEntry.SetAttribute("data-realName", realSensorName);

				humiditySensorEntries.Add(humiditySensorEntry);
			}
			UpdateHumiditySensorDropDown(humiditySensorEntries, humiditySensorNone);
			humiditySensorMultiInputGroup.AppendCustomElement(_humiditySensorDropdown, false);

			grid.AddRow().AppendCollum(humiditySensorMultiInputGroup, autoSize: true);
		}

		private void DeleteChannelButtonOnClick(object sender, TargetEventArgs e)
		{
			const string confirmMessage = "Wirklich Löschen";
			if (_deleteChannelButton.Text != confirmMessage)
			{
				_deleteChannelButton.Text = confirmMessage;
				return;
			}
			else
			{
				_parentChannelsPage.RemoveChannel(this, _channel);
			}
		}

		private void SaveButton_Click(object sender, global::Ooui.TargetEventArgs e)
		{
			_channelProgrammPages.ForEach(page => page.Save());
			if (!_isMasterChannel)
			{
				_channel.Name = _channelNameInputGroup.TextInput.Value;
				_parentChannelsPage.ApplyName(_channel);
				_channel.HumiditySensorEnabled = _humiditySensorEnabledTwoStateButtonGroup.FirstButtonActive;
				_channel.HumiditySensor = _humiditySensorDropdown.Button.GetAttribute("data-realName").ToString();
			}
		}

		private void UpdateHumiditySensorDropDown(List<StylableAnchor> humiditySensorEntries, Anchor humiditySensorNone)
		{
			if (PageStorage<SettingsData>.Instance.StorageData.HumiditySensors.ContainsKey(_humiditySensorDropdown.Button.GetAttribute("data-realName").ToString()) == false)
			{
				_humiditySensorDropdown.Button.SetAttribute("data-realName", "");
			}

			_humiditySensorDropdown.Button.Text = _humiditySensorDropdown.Button.GetAttribute("data-realName").ToString() == "" ? "Keiner Gewählt" : PageStorage<SettingsData>.Instance.StorageData.HumiditySensors[_humiditySensorDropdown.Button.GetAttribute("data-realName").ToString()];
			if (_humiditySensorDropdown.Button.GetAttribute("data-realName").ToString() == "")
			{
				humiditySensorNone.ClassName = humiditySensorNone.ClassName.Replace(" active", "") + " active";
			}
			else
			{
				humiditySensorNone.ClassName = humiditySensorNone.ClassName.Replace(" active", "");
			}

			foreach (StylableAnchor sensorEntry in humiditySensorEntries)
			{
				if (_humiditySensorDropdown.Button.GetAttribute("data-realName").ToString() == sensorEntry.GetAttribute("data-realName").ToString())
				{
					sensorEntry.ClassName = sensorEntry.ClassName.Replace(" active", "") + " active";
				}
				else
				{
					sensorEntry.ClassName = sensorEntry.ClassName.Replace(" active", "");
				}
			}
		}
	}
}

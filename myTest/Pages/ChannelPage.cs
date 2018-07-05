using Ooui;
using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Storage;
using TabNoc.Ooui.UiComponents;
using TabNoc.Ooui.UiComponents.FormControl.InputGroups;
using Button = TabNoc.Ooui.HtmlElements.Button;

namespace TabNoc.Ooui.Pages
{
	internal class ChannelPage : StylableElement
	{
		private readonly ChannelData _channel;
		private readonly SettingsPage _parentSettingsPage;
		private readonly bool _isMasterChannel;
		private readonly TabNavigation _tabNavigation;
		private readonly TextInputGroup _channelNameInputGroup;
		private readonly HtmlElements.Button _deleteChannelButton;

		private readonly List<ChannelProgrammPage> _channelProgrammPages = new List<ChannelProgrammPage>();

		public ChannelPage(ChannelData channel, SettingsPage parentSettingsPage, bool isMasterChannel) : base("div")
		{
			_channel = channel;
			_parentSettingsPage = parentSettingsPage;
			_isMasterChannel = isMasterChannel;
			SetBorder(BorderKind.Rounded, StylingColor.Secondary);
			Grid grid = new Grid();
			grid.AddStyling(StylingOption.MarginRight, 2);
			grid.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddStyling(StylingOption.MarginTop, 5);
			grid.AddStyling(StylingOption.MarginBottom, 2);

			#region TextInputGroup ProgrammName

			_channelNameInputGroup = new TextInputGroup("KanalName", "N/A");
			_channelNameInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			_channelNameInputGroup.TextInput.Value = channel.Name;
			if (!isMasterChannel)
			{
				_deleteChannelButton = new HtmlElements.Button(StylingColor.Danger, text: "Kanal Löschen");
				_deleteChannelButton.Click += DeleteChannelButtonOnClick;
				_channelNameInputGroup.AddFormElement(_deleteChannelButton);
			}
			else
			{
				_channelNameInputGroup.IsDisabled = true;
			}
			grid.AddRow().AppendCollum(_channelNameInputGroup);

			#endregion TextInputGroup ProgrammName

			Row tabRow = new Row();
			_tabNavigation = new TabNavigation();
			foreach (ChannelProgramData channelProgramData in channel.ProgramList)
			{
				ChannelProgrammPage channelProgrammPage = new ChannelProgrammPage(channelProgramData, this, isMasterChannel);
				_channelProgrammPages.Add(channelProgrammPage);
				_tabNavigation.AddTab(channelProgramData.Id.ToString(), channelProgrammPage, channelProgramData.Id == 1);
			}
			tabRow.AppendCollum(_tabNavigation);

			#region Add Channel Programm Button

			Button addTabButton = new Button(StylingColor.Primary, true, Button.ButtonSize.Small, false, "+");
			addTabButton.Click += (sender, args) =>
			{
				ChannelProgramData channelProgramData = ChannelProgramData.CreateNew(channel.ProgramList.Count + 1);
				channel.ProgramList.Add(channelProgramData);
				ChannelProgrammPage channelProgrammPage = new ChannelProgrammPage(channelProgramData, this, isMasterChannel);
				_channelProgrammPages.Add(channelProgrammPage);
				_tabNavigation.AddTab(channelProgramData.Id.ToString(), channelProgrammPage, channelProgramData.Id == 1);
			};
			addTabButton.AddStyling(StylingOption.MarginTop, 1);
			tabRow.AppendCollum(addTabButton, autoSize: true);

			#endregion Add Channel Programm Button

			grid.AddRow().AppendCollum(tabRow);

			#region SaveChannel Button

			Button saveButton = new Button(StylingColor.Success, true, Button.ButtonSize.Normal, false, "Speichern");
			saveButton.AddStyling(StylingOption.MarginTop, 4);
			saveButton.AddStyling(StylingOption.MarginLeft, 4);
			saveButton.AddStyling(StylingOption.MarginBottom, 1);
			saveButton.Click += SaveButton_Click;
			grid.AddRow().AppendCollum(saveButton);

			#endregion SaveChannel Button

			AppendChild(grid);
		}

		private void SaveButton_Click(object sender, global::Ooui.TargetEventArgs e)
		{
			_channelProgrammPages.ForEach(page => page.Save());
			if (!_isMasterChannel)
			{
				_channel.Name = _channelNameInputGroup.TextInput.Value;
				_parentSettingsPage.ApplyName(_channel);
			}
		}

		public void RemoveProgramm(ChannelProgrammPage channelProgrammPage, ChannelProgramData channelProgram)
		{
			_channelProgrammPages.Remove(channelProgrammPage);
			_channel.ProgramList.Remove(channelProgram);
			_tabNavigation.RemoveTab(channelProgram.Id.ToString(), channelProgrammPage);
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
				_parentSettingsPage.RemoveChannel(this, _channel);
			}
		}
	}
}

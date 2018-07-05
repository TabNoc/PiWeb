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
	internal class SettingsPage : StylableElement
	{
		private readonly Settings _settingsData;
		private readonly VerticalPillNavigation _pillNavigation = new VerticalPillNavigation("col-3", "col-9");

		public SettingsPage(Settings settingsData) : base("div")
		{
			_settingsData = settingsData;
			TwoStateButtonGroup enabledButtonGroup = new TwoStateButtonGroup("Aktiv", "Inaktiv", settingsData.SettingsData.Enabled, !settingsData.SettingsData.Enabled);
			enabledButtonGroup.FirstButtonStateChange += (sender, args) => settingsData.SettingsData.Enabled = args.NewButtonState;
			InputGroup inputGroup = new InputGroup("Automatik", enabledButtonGroup);
			inputGroup.AddStyling(StylingOption.MarginBottom, 2);
			inputGroup.AddStyling(StylingOption.MarginTop, 2);
			Row row = new Row();
			row.AppendCollum(inputGroup, autoSize:true);
			AppendChild(row);

			HtmlElements.Button addChannel = new HtmlElements.Button(asOutline: true, size:Button.ButtonSize.Small);
			addChannel.Click += (sender, args) =>
			{
				ChannelData channelData = ChannelData.CreateNew();
				settingsData.SettingsData.Channels.Add(channelData);
				AddChannel(channelData.Name, channelData, false);
			};
			addChannel.Text = "Neuen Kanal hinzufügen";

			AddChannel("Master", settingsData.SettingsData.MasterChannel, true);

			foreach (ChannelData channel in settingsData.SettingsData.Channels)
			{
				AddChannel(channel.Name, channel, false);
			}

			AppendChild(_pillNavigation);
			AppendChild(addChannel);
		}

		private bool _hasActivePill = false;

		private readonly Dictionary<ChannelData, Anchor> _pillDictionary = new Dictionary<ChannelData, Anchor>();

		private void AddChannel(string channelName, ChannelData channel, bool isMasterChannel)
		{
			Anchor pill = _pillNavigation.AddPill(channelName, new ChannelPage(channel, this, isMasterChannel), _hasActivePill == false);
			_pillDictionary.Add(channel, pill);
			_hasActivePill = true;
		}

		public void ApplyName(ChannelData channel)
		{
			_pillDictionary[channel].Text = channel.Name;
		}

		public void RemoveChannel(ChannelPage channelPage, ChannelData channel)
		{
			_settingsData.SettingsData.Channels.Remove(channel);
			_pillNavigation.RemovePill(channel.Name, channelPage);
			
		}
	}
}

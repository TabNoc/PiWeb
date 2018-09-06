using Ooui;
using System;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.PiWeb.DataTypes.WateringWeb.Settings;
using TabNoc.PiWeb.Pages.WateringWeb.Settings;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class SettingsPagePublisher : WateringPublisher
	{
		public SettingsPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<SettingsData>.Instance.Initialize("Settings", new TimeSpan(0, 0, 5));
			PageStorage<HumiditySensorData>.Instance.ReadOnly = true;
			PageStorage<HumiditySensorData>.Instance.Initialize("Humidity", new TimeSpan(0, 0, 5));
		}

		protected override Element CreatePage()
		{
			SettingsPage settingsPage = new SettingsPage(PageStorage<SettingsData>.Instance);
			settingsPage.AddStyling(StylingOption.MarginRight, 5);
			settingsPage.AddStyling(StylingOption.MarginLeft, 1);
			settingsPage.ClassName += " col-xl-10";
			return settingsPage;
		}

		protected override void Initialize()
		{
		}
	}
}

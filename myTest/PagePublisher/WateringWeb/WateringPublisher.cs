using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.UiComponents;

namespace TabNoc.Ooui.PagePublisher.WateringWeb
{
	internal abstract class WateringPublisher : Publisher
	{
		protected WateringPublisher(string publishPath) : base(publishPath)
		{
		}

		protected override NavigationBar CreateNavigationBar()
		{
			NavigationBar navBar = new NavigationBar("WateringWeb", "/overview", new Anchor("/", "PiWeb"));

			string address = "/overview";
			navBar.AddElement(PublishPath == address, "Übersicht", address);

			address = "/channels";
			navBar.AddElement(PublishPath == address, "Kanäle", address);

			address = "/manual";
			navBar.AddElement(PublishPath == address, "Manuell", address);

			address = "/settings";
			navBar.AddElement(PublishPath == address, "Einstellungen", address);

			address = "/history";
			navBar.AddElement(PublishPath == address, "Verlauf", address);

			return navBar;
		}
	}
}

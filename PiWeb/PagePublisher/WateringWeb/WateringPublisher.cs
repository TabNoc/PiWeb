using Ooui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.UiComponents;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal abstract class WateringPublisher : Publisher
	{
		protected WateringPublisher(string publishPath) : base(publishPath)
		{
		}

		protected override NavigationBar CreateNavigationBar()
		{
			NavigationBar navBar = new NavigationBar("WateringWeb", "/overview", new Anchor("/", "PiWeb"), true);

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

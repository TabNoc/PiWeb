using Ooui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.UiComponents;
using TabNoc.PiWeb.Pages;

namespace TabNoc.PiWeb.PagePublisher
{
	internal class PiWebPublisher : Publisher
	{
		public PiWebPublisher(string publishPath) : base(publishPath)
		{
		}

		protected override void Initialize()
		{
		}

		protected override Element CreatePage()
		{
			PiWebPage piWebPage = new PiWebPage();
			return piWebPage;
		}

		protected override NavigationBar CreateNavigationBar()
		{
			return null;
		}
	}
}

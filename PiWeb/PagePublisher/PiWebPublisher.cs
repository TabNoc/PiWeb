using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Pages;
using TabNoc.Ooui.UiComponents;

namespace TabNoc.Ooui.PagePublisher
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

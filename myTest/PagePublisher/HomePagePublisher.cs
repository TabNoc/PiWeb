using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages.Home;

namespace TabNoc.Ooui
{
	internal class HomePagePublisher : WateringPublisher
	{
		public HomePagePublisher(string publishPath) : base(publishPath)
		{
		}

		protected override void Initialize()
		{
		}

		protected override Element CreatePage()
		{
			HomePage homePage = new HomePage();
			homePage.AddStyling(StylingOption.MarginRight, 5);
			homePage.ClassName += " col-xl-10";
			return homePage;
		}
	}
}

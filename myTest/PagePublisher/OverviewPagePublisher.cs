using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages;

namespace TabNoc.Ooui
{
	internal class OverviewPagePublisher : WateringPublisher
	{
		public OverviewPagePublisher(string publishPath) : base(publishPath)
		{
		}

		protected override void Initialize()
		{
		}

		protected override Element CreatePage()
		{
			OverviewPage overviewPage = new OverviewPage();
			overviewPage.AddStyling(StylingOption.MarginRight, 5);
			overviewPage.ClassName += " col-xl-10";
			return overviewPage;
		}
	}
}

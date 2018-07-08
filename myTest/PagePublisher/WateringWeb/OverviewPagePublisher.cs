using Ooui;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages.WateringWeb.Overview;

namespace TabNoc.Ooui.PagePublisher.WateringWeb
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
			overviewPage.AddStyling(StylingOption.MarginLeft, 1);
			overviewPage.ClassName += " col-xl-10";
			return overviewPage;
		}
	}
}

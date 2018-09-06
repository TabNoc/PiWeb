using Ooui;
using System;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.PiWeb.DataTypes.WateringWeb.Overview;
using TabNoc.PiWeb.Pages.WateringWeb.Overview;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class OverviewPagePublisher : WateringPublisher
	{
		public OverviewPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<OverviewData>.Instance.ReadOnly = true;
			PageStorage<OverviewData>.Instance.Initialize("Overview", new TimeSpan(0, 0, 5));
		}

		protected override Element CreatePage() => new OverviewPage();

		protected override void Initialize()
		{
		}
	}
}

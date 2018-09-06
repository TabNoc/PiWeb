using Ooui;
using System;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.UiComponents;
using TabNoc.PiWeb.DataTypes.WateringWeb.History;
using TabNoc.PiWeb.Pages.WateringWeb.History;

namespace TabNoc.PiWeb.PagePublisher.WateringWeb
{
	internal class HistoryPagePublisher : WateringPublisher
	{
		public HistoryPagePublisher(string publishPath) : base(publishPath)
		{
			PageStorage<HistoryData>.Instance.ReadOnly = true;
			PageStorage<HistoryData>.Instance.Initialize("History", new TimeSpan(0, 0, 5));
		}

		protected override Element CreatePage()
		{
			return new HistoryPage();
		}

		protected override void Initialize()
		{
			Console.WriteLine("Initialize" + this.GetType().Name);
		}
	}
}

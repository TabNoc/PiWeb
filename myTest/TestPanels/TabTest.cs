using System;
using Ooui;
using TabNoc.Ooui.Interfaces;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.UiComponents;

namespace TabNoc.Ooui.TestPanels
{
	public class TabTest : Publishable
	{
		protected override Element PopulateAppElement()
		{
			Div appElement = new Div();

			TabNavigation tabNavigation = new TabNavigation();
			tabNavigation.AddTab("test1", new Anchor("example.com", "testLink1"), true);

			TabNavigation content = new TabNavigation();
			content.AddTab("tabtab1", new Anchor("example2.com", "Hallo Test1!"), true);
			content.AddTab("tabtab2", new Anchor("example2.com", "Hallo Test2!"), false);
			tabNavigation.AddTab("Papas", content, false);

			tabNavigation.AddTab("test2", new Anchor("example.com", "testLink2"), false);
			tabNavigation.AddTab("test3", new Anchor("example.com", "testLink3"), false);

			Button button = new Button();
			button.Click += (sender, args) =>
			{
				Console.WriteLine(tabNavigation.OuterHtml);
			};
			tabNavigation.AddTab("test4", button, false);

			appElement.AppendChild(tabNavigation);
			return appElement;
		}

		public TabTest(string publishPath) : base(publishPath)
		{
		}
	}
}

﻿using Ooui;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TabNoc.Ooui.UiComponents;

namespace TabNoc.Ooui.Interfaces.AbstractObjects
{
	internal abstract class WateringPublisher : Publishable
	{
		private readonly string _publishPath;

		protected WateringPublisher(string publishPath) : base(publishPath)
		{
			_publishPath = publishPath;
		}

		protected abstract void Initialize();

		protected abstract Element CreatePage();

		protected override Element PopulateAppElement()
		{
			Stopwatch stopwatchMain = new Stopwatch();
			stopwatchMain.Start();
			Initialize();

			Grid grid = new Grid();

			Loading loading = new Loading();
			grid.AppendChild(loading);

			grid.AddRow().AppendCollum(CreateNavigationBar());

			Stopwatch stopwatchCreatePage = new Stopwatch();

			Task.Run((Func<Element>)CreatePage).ContinueWith(task =>
			{
				grid.AddRow().AppendCollum(task.Result);
				stopwatchCreatePage.Stop();
				Console.WriteLine($"\t\tSended Website Content: {grid.OuterHtml.Length}Byte\r\n\t\tElapsedTime from CreatePage: {stopwatchCreatePage.ElapsedMilliseconds}ms");
				grid.RemoveChild(loading);
			});
			stopwatchMain.Stop();
			Console.WriteLine($"\tSended Website Content: {grid.OuterHtml.Length}Byte\r\n\tElapsedTime in PopulateAppElement: {stopwatchMain.ElapsedMilliseconds}ms");
			stopwatchCreatePage.Start();
			return grid;
		}

		private NavigationBar CreateNavigationBar()
		{
			NavigationBar navBar = new NavigationBar("WateringWeb", "/home");

			string address = "/home";
			navBar.AddElement(_publishPath == address, "Home", address);

			address = "/overview";
			navBar.AddElement(_publishPath == address, "Overview", address);

			address = "/settings";
			navBar.AddElement(_publishPath == address, "Settings", address);

			address = "/test";
			navBar.AddElement(_publishPath == address, "test", address);

			address = "/tabTest";
			navBar.AddElement(_publishPath == address, "tabTest", address);

			address = "/buttonTest";
			navBar.AddElement(_publishPath == address, "buttonTest", address);

			return navBar;
		}
	}
}

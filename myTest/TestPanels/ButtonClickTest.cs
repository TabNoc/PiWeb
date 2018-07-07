using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Ooui;
using TabNoc.Ooui.Interfaces;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.UiComponents;

namespace TabNoc.Ooui.TestPanels
{
	public class ButtonClickTest : Publishable
	{
		protected override Element PopulateAppElement()
		{
			Div appElement = new Div();
			TabNavigation tabNavigation = new TabNavigation();
			tabNavigation.AddTab("test1", new Anchor("example.com", "testLink1"), true);
			tabNavigation.AddTab("test2", new Anchor("example.com", "testLink2"), false);
			tabNavigation.AddTab("test3", new Anchor("example.com", "testLink3"), false);
			// AppElement.AppendChild(tabNav.WrapperDiv);

			Div div = new Div();
			div.Text = "Hallo Papa!";
			appElement.AppendChild(div);

			Button testButton = new Button("Click mich!");
			testButton.Click += (sender, args) =>
			{
				div.Text = "Button gedrückt!";
			};
			appElement.AppendChild(testButton);

			Button testButton2 = new Button("Click mich2!");
			testButton2.Click += (sender, args) =>
			{
				try
				{
					div.Text = HttpGet();
				}
				catch (Exception e)
				{
					div.Text = e.ToString();
				}
			};
			appElement.AppendChild(testButton2);
			return appElement;
		}

		public ButtonClickTest(string publishPath) : base(publishPath)
		{
		}

		public string HttpGet()
		{
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
			string text = string.Empty;
			string url = @"https://piw/";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			return text;
		}
	}
}

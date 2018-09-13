using Ooui;
using System;
using System.Globalization;
using System.Timers;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents
{
	public class NavigationBar : StylableElement, IDisposable
	{
		/*
<nav class="navbar navbar-expand-lg navbar-light bg-light">
  <a class="navbar-brand" href="#">Navbar</a>
  <div class="collapse navbar-collapse" id="navbarNavAltMarkup">
    <div class="navbar-nav">
      <a class="nav-item nav-link active" href="#">Home <span class="sr-only">(current)</span></a>
      <a class="nav-item nav-link" href="#">Features</a>
      <a class="nav-item nav-link" href="#">Pricing</a>
      <a class="nav-item nav-link disabled" href="#">Disabled</a>
    </div>
  </div>
</nav>
		 */

		private readonly List _navigationList;
		private readonly Timer _showLocalDateTimeTimer;

		public NavigationBar(string brandName, string brandAddress, Anchor lastAnchor, bool showLocalDateTime) : base("nav")
		{
			ClassName = "navbar navbar-expand-sm navbar-light";
			Style.BackgroundColor = "#e3f2fd";

			Anchor brandAnchor = new Anchor(brandAddress, brandName)
			{
				ClassName = "navbar-brand"
			};
			AppendChild(brandAnchor);

			Div navigationDiv = new Div() { ClassName = "collapse navbar-collapse" };
			_navigationList = new List(false)
			{
				ClassName = "navbar-nav mr-auto"
			};
			AppendChild(navigationDiv);
			navigationDiv.AppendChild(_navigationList);

			if (showLocalDateTime)
			{
				Div showLocalDateTimeDiv = new Div() { ClassName = "navbar-nav mr-5" };

				navigationDiv.AppendChild(showLocalDateTimeDiv);

				_showLocalDateTimeTimer = new Timer(400);
				_showLocalDateTimeTimer.Elapsed += (sender, args) =>
				{
					try
					{
						showLocalDateTimeDiv.Text = DateTime.Now.ToString(CultureInfo.GetCultureInfo("de-de"));
						if (_navBarOpenDateTime.AddMinutes(-30) > DateTime.Now)
						{
							Console.WriteLine("Die Navigationsleiste ist länger als 30 Minuten offen -> der Refresh wird gestoppt");
							return;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				};
				_showLocalDateTimeTimer.Start();
			}

			lastAnchor.ClassName = "navbar-brand";
			navigationDiv.AppendChild(lastAnchor);
		}

		public void AddElement(bool active, string text, string address)
		{
			Anchor anchor = new Anchor(address, text) { ClassName = "nav-item nav-link" + (active == true ? " active" : ""), Style = { ZIndex = 1060 } };

			_navigationList.AppendChild(anchor);
		}

		#region Dispose Pattern

		private bool _disposed;
		private readonly DateTime _navBarOpenDateTime = DateTime.Now;

		~NavigationBar()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		public new void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!_disposed)
				{
					_showLocalDateTimeTimer.Stop();
					_showLocalDateTimeTimer.Dispose();
					Console.WriteLine("Disposed Navbar");

					_disposed = true;
				}

				base.Dispose(disposing);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		#endregion Dispose Pattern
	}
}

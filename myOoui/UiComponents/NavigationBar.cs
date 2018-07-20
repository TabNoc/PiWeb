using Ooui;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents
{
	public class NavigationBar : StylableElement
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

		public NavigationBar(string brandName, string brandAddress, Anchor lastAnchor) : base("nav")
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

			lastAnchor.ClassName = "navbar-brand";
			navigationDiv.AppendChild(lastAnchor);
		}

		public void AddElement(bool active, string text, string address)
		{
			Anchor anchor = new Anchor(address, text) { ClassName = "nav-item nav-link" + (active == true ? " active" : "") };

			_navigationList.AppendChild(anchor);
		}
	}
}

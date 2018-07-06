using Ooui;

namespace TabNoc.Ooui.UiComponents
{
	internal class NavigationBar : Element
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

		private readonly Div _navigationDiv;

		public NavigationBar(string brandName, string brandAddress) : base("nav")
		{
			ClassName = "navbar navbar-expand-sm navbar-light";
			Style.BackgroundColor = "#e3f2fd";

			Anchor brandAnchor = new Anchor(brandAddress, brandName)
			{
				ClassName = "navbar-brand"
			};
			AppendChild(brandAnchor);

			_navigationDiv = new Div() { ClassName = "navbar-nav" };
			AppendChild(_navigationDiv);
		}

		public void AddElement(bool active, string text, string address)
		{
			Anchor anchor = new Anchor(address, text) { ClassName = "nav-item nav-link" + (active == true ? " active" : "") };

			_navigationDiv.AppendChild(anchor);
		}
	}
}

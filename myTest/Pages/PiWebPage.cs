using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.Pages
{
	internal class PiWebPage : StylableElement
	{
		public PiWebPage() : base("div")
		{
			AddStyling(StylingOption.MarginTop,5);
			AddStyling(StylingOption.MarginLeft,5);
			Heading heading = new Heading(1, "Willkommen auf PiWeb.");
			Heading heading2 = new Heading(4, "Aktuell gibt es folgende Projekte:");
			List uList = new List();
			Anchor anchor = new Anchor("/overview", "WateringWeb");
			uList.AppendChild(anchor);

			AppendChild(heading);
			AppendChild(heading2);
			AppendChild(uList);

		}
	}
}

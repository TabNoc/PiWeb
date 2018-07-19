using Ooui;
using System;
using System.Linq;
using TabNoc.Ooui.HtmlElements;
using TabNoc.Ooui.Interfaces.Enums;
using Button = TabNoc.Ooui.HtmlElements.Button;

namespace TabNoc.Ooui.UiComponents
{
	internal class TabNavigation : Element
	{
		/*
			 <nav>
			  <div class="nav nav-tabs" role="tablist">
				<a class="nav-item nav-link active" id="nav-home-tab" data-toggle="tab" href="#nav-home" role="tab" aria-controls="nav-home" aria-selected="true">Home</a>
				<a class="nav-item nav-link" id="nav-profile-tab" data-toggle="tab" href="#nav-profile" role="tab" aria-controls="nav-profile" aria-selected="false">Profile</a>
				<a class="nav-item nav-link" id="nav-contact-tab" data-toggle="tab" href="#nav-contact" role="tab" aria-controls="nav-contact" aria-selected="false">Contact</a>
			  </div>
			</nav>
			<div class="tab-content" id="nav-tabContent">
			  <div class="tab-pane fade show active" id="nav-home" role="tabpanel" aria-labelledby="nav-home-tab">...</div>
			  <div class="tab-pane fade" id="nav-profile" role="tabpanel" aria-labelledby="nav-profile-tab">...</div>
			  <div class="tab-pane fade" id="nav-contact" role="tabpanel" aria-labelledby="nav-contact-tab">...</div>
			</div>
			 */
		public readonly Button AddButton;
		private readonly Div _contentDiv;
		private readonly Div _navigationDiv;
		private bool _hasActiveTab = false;

		public TabNavigation(bool asCardStyle = false, bool includeAddButton = false) : base("div")
		{
			Nav navElement = new Nav();
			_navigationDiv = new Div
			{
				ClassName = "nav nav-tabs"
			};
			if (asCardStyle)
			{
				ClassName = "card text-center";
				navElement.ClassName = "card-header";
				_navigationDiv.ClassName += " card-header-tabs";
			}
			_navigationDiv.SetAttribute("role", "tablist");
			navElement.AppendChild(_navigationDiv);

			_contentDiv = new Div
			{
				ClassName = "tab-content"
			};

			base.AppendChild(navElement);
			base.AppendChild(_contentDiv);

			if (includeAddButton)
			{
				AddButton = new Button(StylingColor.Primary, true, Button.ButtonSize.Small, false, "+");
				AddButton.AddStyling(StylingOption.MarginTop, 1);
				AddButton.AddStyling(StylingOption.MarginRight, 3);
				AddButton.AddStyling(StylingOption.Height, 50);
				_navigationDiv.AppendChild(AddButton);
			}
		}

		public Anchor AddTab(string tabName, Element content, bool active)
		{
			if (_hasActiveTab == true && active == true)
			{
				throw new ArgumentException("Es darf nur ein Tab Aktiv sein", nameof(active));
			}

			_hasActiveTab |= active;
			Anchor anchor = new Anchor("n/A", tabName)
			{
				ClassName = "nav-item nav-link" + (active ? " active" : "")
			};
			anchor.SetAttribute("data-toggle", "tab");
			anchor.SetAttribute("role", "tab");
			anchor.SetAttribute("aria-selected", active ? "true" : "false");
			_navigationDiv.AppendChild(anchor);

			Div div = new Div
			{
				ClassName = "tab-pane fade" + (active ? " show active" : "")
			};
			div.SetAttribute("role", "tabpanel");
			div.SetAttribute("aria-labelledby", anchor.Id);
			div.AppendChild(content);
			_contentDiv.AppendChild(div);

			anchor.SetAttribute("aria-controls", div.Id);
			anchor.HRef = "#" + div.Id;

			_contentDiv.AppendChild(div);

			return anchor;
		}

		public void RemoveTab(string tabName, Element content)
		{
			_contentDiv.RemoveChild(_contentDiv.Children.First(node => node.Children.Contains(content)));
			Node removeChild = _navigationDiv.RemoveChild(_navigationDiv.Children.First(node => node is Anchor anchor && anchor.Text == tabName));
			if (removeChild is Anchor removedAnchor)
			{
				if (removedAnchor.ClassName.Contains("active"))
				{
					_hasActiveTab = false;
				}
			}
		}
	}
}

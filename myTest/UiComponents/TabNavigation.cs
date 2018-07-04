using Ooui;
using System;
using System.Linq;
using TabNoc.Ooui.HtmlElements;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Pages;

namespace TabNoc.Ooui.UiComponents
{
	public class TabNavigation : Element
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
		private readonly Div _navigationDiv;
		private readonly Div _contentDiv;
		private bool _hasActiveTab = false;

		public TabNavigation() : base("div")
		{
			Nav navElement = new Nav();
			_navigationDiv = new Div
			{
				ClassName = "nav nav-tabs"
			};
			_navigationDiv.SetAttribute("role", "tablist");
			navElement.AppendChild(_navigationDiv);

			_contentDiv = new Div
			{
				ClassName = "tab-content"
			};

			base.AppendChild(navElement);
			base.AppendChild(_contentDiv);
		}

		public void AddTab(string tabName, Element content, bool active)
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
		}

		public void RemoveTab(string tabName, Element content)
		{
			_contentDiv.RemoveChild(_contentDiv.Children.First(node => node.Children.Contains(content)));
			_navigationDiv.RemoveChild(_navigationDiv.Children.First(node => node is Anchor anchor && anchor.Text == tabName));
		}
	}
}

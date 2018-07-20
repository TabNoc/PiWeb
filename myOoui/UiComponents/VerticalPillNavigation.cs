using Ooui;
using System;
using System.Linq;
using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents
{
	public class VerticalPillNavigation : StylableElement
	{
		/*
		<div class="row">
		  <div class="col-3">
		    <div class="nav flex-column nav-pills" role="tablist" aria-orientation="vertical">
		      <a class="nav-link active" id="v-pills-home-tab" data-toggle="pill" href="#v-pills-home" role="tab" aria-controls="v-pills-home" aria-selected="true">Home</a>
		      <a class="nav-link" id="v-pills-profile-tab" data-toggle="pill" href="#v-pills-profile" role="tab" aria-controls="v-pills-profile" aria-selected="false">Profile</a>
		      <a class="nav-link" id="v-pills-messages-tab" data-toggle="pill" href="#v-pills-messages" role="tab" aria-controls="v-pills-messages" aria-selected="false">Messages</a>
		      <a class="nav-link" id="v-pills-settings-tab" data-toggle="pill" href="#v-pills-settings" role="tab" aria-controls="v-pills-settings" aria-selected="false">Settings</a>
		    </div>
		  </div>
		  <div class="col-9">
		    <div class="tab-content">
		      <div class="tab-pane fade show active" id="v-pills-home" role="tabpanel" aria-labelledby="v-pills-home-tab">...</div>
		      <div class="tab-pane fade" id="v-pills-profile" role="tabpanel" aria-labelledby="v-pills-profile-tab">...</div>
		      <div class="tab-pane fade" id="v-pills-messages" role="tabpanel" aria-labelledby="v-pills-messages-tab">...</div>
		      <div class="tab-pane fade" id="v-pills-settings" role="tabpanel" aria-labelledby="v-pills-settings-tab">...</div>
		    </div>
		  </div>
		</div>
			 */
		private readonly Div _navigationDiv;
		private readonly Div _contentDiv;
		private bool _hasActivePill = false;

		public VerticalPillNavigation(int navigationSize, int contentSize, bool asCardStyle = false) : this("col-" + navigationSize, "col-" + contentSize, asCardStyle)
		{
			if (navigationSize + contentSize > 12)
			{
				throw new ArgumentException("The Sum of " + nameof(navigationSize) + " and " + nameof(contentSize) + " must be smaller then 12!");
			}
		}

		//("col-3", "col-9")
		public VerticalPillNavigation(string navigationDivWrapperClassName, string contentDivWrapperClassName, bool asCardStyle = false) : base("div")
		{
			ClassName = "row" + (asCardStyle ? " ml-0" : "");

			Div navigationDivWrapper = new Div
			{
				ClassName = navigationDivWrapperClassName + (asCardStyle ? " card text-center card-header rounded" : "")
			};
			AppendChild(navigationDivWrapper);

			_navigationDiv = new Div
			{
				ClassName = "nav flex-column nav-pills"
			};
			_navigationDiv.SetAttribute("role", "tablist");
			_navigationDiv.SetAttribute("aria-orientation", "vertical");
			navigationDivWrapper.AppendChild(_navigationDiv);

			Div contentDivWrapper = new Div
			{
				ClassName = contentDivWrapperClassName
			};
			AppendChild(contentDivWrapper);

			_contentDiv = new Div
			{
				ClassName = "tab-content"
			};
			contentDivWrapper.AppendChild(_contentDiv);
		}

		public Anchor AddPill(string pillName, Element content, bool active)
		{
			if (_hasActivePill == true && active == true)
			{
				throw new ArgumentException("Es darf nur ein Tab Aktiv sein", nameof(active));
			}

			_hasActivePill |= active;
			Anchor anchor = new Anchor("n/A", pillName)
			{
				ClassName = "nav-link" + (active ? " active" : "")
			};
			anchor.SetAttribute("data-toggle", "pill");
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

		public void RemovePill(string pillName, Element content)
		{
			_contentDiv.RemoveChild(_contentDiv.Children.First(node => node.Children.Contains(content)));
			_navigationDiv.RemoveChild(_navigationDiv.Children.First(node => node is Anchor anchor && anchor.Text == pillName));
		}

		public void Clear()
		{
			while (_contentDiv.FirstChild != null)
			{
				_contentDiv.RemoveChild(_contentDiv.FirstChild);
			}
			while (_navigationDiv.FirstChild != null)
			{
				_navigationDiv.RemoveChild(_navigationDiv.FirstChild);
			}

			_hasActivePill = false;
		}
	}
}

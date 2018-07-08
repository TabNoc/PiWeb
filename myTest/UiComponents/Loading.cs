using Ooui;
using System;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.UiComponents.FormControl.InputGroups;
using Button = TabNoc.Ooui.HtmlElements.Button;

namespace TabNoc.Ooui.UiComponents
{
	internal class Loading : StylableElement
	{
		/*
<div class="modal fade show" tabindex="-1" role="dialog" aria-labelledby="exampleModalLiveLabel" style="padding-right: 17px; display: block;">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="exampleModalLiveLabel">Please Wait, Loading</h5>
      </div>
      <div class="modal-body">
        <div class="progress">
		  <div role="progressbar" class="progress-bar progress-bar-striped progress-bar-animated w-100"></div>
		</div>
      </div>
    </div>
  </div>
</div>
		 */
		private readonly Heading _loadingText;
		private Div bodyWrapper;
		private readonly Div _progressBarDiv;
		private Div contentWrapper;

		public Loading(string text = "Please Wait, Loading...") : base("div")
		{
			ClassName = "modal fade show";
			SetAttribute("tabindex", "-1");
			SetAttribute("role", "dialog");
			Style.PaddingRight = "17px";
			Style.Display = "block";

			Div dialogWrapper = new Div() { ClassName = "modal-dialog" };
			dialogWrapper.SetAttribute("role", "document");
			AppendChild(dialogWrapper);

			contentWrapper = new Div() { ClassName = "modal-content" };
			dialogWrapper.AppendChild(contentWrapper);

			Div headerWrapper = new Div() { ClassName = "modal-header" };
			contentWrapper.AppendChild(headerWrapper);

			_loadingText = new Heading(5, text)
			{
				ClassName = "modal-title"
			};
			headerWrapper.AppendChild(_loadingText);

			bodyWrapper = new Div()
			{
				ClassName = "modal-body"
			};
			contentWrapper.AppendChild(bodyWrapper);

			Div progressDiv = new Div()
			{
				ClassName = "progress"
			};
			bodyWrapper.AppendChild(progressDiv);

			_progressBarDiv = new Div()
			{
				ClassName = "progress-bar progress-bar-striped progress-bar-animated w-100"
			};
			_progressBarDiv.SetAttribute("role", "progressbar");
			progressDiv.AppendChild(_progressBarDiv);

			SetAttribute("aria-labelledby", _loadingText.Id.ToString());
		}

		public void ShowError(Exception exception, string msg)
		{
			_progressBarDiv.ClassName += " bg-danger";
			_loadingText.Text = msg;

			Div footer = new Div
			{
				ClassName = "modal-footer"
			};
			contentWrapper.AppendChild(footer);

			TextInputGroup inputGroup = new TextInputGroup("", "Passwort", inValidFeedback: "Falsches Passwort", centeredText: true)
			{
				IsHidden = true
			};
			Button button = new Button(StylingColor.Info, true, text:"Infos anzeigen");
			button.Click += (sender, args) =>
			{
				if (inputGroup.IsHidden == true)
				{
					inputGroup.IsHidden = false;
				}
				else
				{
					if (inputGroup.TextInput.Value == "huhu")
					{
						Span span = new Span(exception.ToString());
						footer.AppendChild(span);
						footer.RemoveChild(inputGroup);
						footer.RemoveChild(button);
					}
					else
					{
						inputGroup.TextInput.SetValidation(false, true);
					}
				}
			};

			footer.AppendChild(inputGroup);
			footer.AppendChild(button);
		}
	}
}

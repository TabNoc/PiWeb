using Ooui;
using TabNoc.Ooui.HtmlElements;
using TabNoc.Ooui.Interfaces.AbstractObjects;

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

			Div contentWrapper = new Div() { ClassName = "modal-content" };
			dialogWrapper.AppendChild(contentWrapper);

			Div headerWrapper = new Div() { ClassName = "modal-header" };
			contentWrapper.AppendChild(headerWrapper);

			H5 loadingText = new H5
			{
				ClassName = "modal-title",
				Text = text
			};
			headerWrapper.AppendChild(loadingText);

			Div bodyWrapper = new Div()
			{
				ClassName = "modal-body"
			};
			contentWrapper.AppendChild(bodyWrapper);

			Div progressDiv = new Div()
			{
				ClassName = "progress"
			};
			bodyWrapper.AppendChild(progressDiv);

			Div progressBarDiv = new Div()
			{
				ClassName = "progress-bar progress-bar-striped progress-bar-animated w-100"
			};
			progressBarDiv.SetAttribute("role", "progressbar");
			progressDiv.AppendChild(progressBarDiv);

			SetAttribute("aria-labelledby", loadingText.Id.ToString());
		}
	}
}

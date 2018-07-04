using Ooui;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.UiComponents.FormControl.InputGroups
{
	internal class InputGroup : InputGroupControl
	{
		public InputGroup(string labelText, StylableElement content, int sizeInPx = -1)
		{
			ClassName = "input-group";

			Div div1 = new Div
			{
				ClassName = "input-group-prepend",
				Style = { Width = sizeInPx }
			};
			AppendChild(div1);

			Div div2 = new Div
			{
				ClassName = "input-group-text w-100",
				Text = labelText
			};
			div1.AppendChild(div2);

			Div contentDivWrapper = new Div()
			{
				ClassName = "input-group-append form-control"
			};
			contentDivWrapper.AppendChild(content);

			AppendChild(contentDivWrapper);
		}
	}

	/*

<div class="input-group mb-3">
  <div class="input-group-prepend">
    <div class="input-group-text">
      <input type="checkbox" aria-label="Checkbox for following text input">
    </div>
  </div>
  <input type="text" class="form-control" aria-label="Text input with checkbox">
</div>
<div class="input-group">
  <div class="input-group-prepend">
    <div class="input-group-text">
    <input type="radio" aria-label="Radio button for following text input">
    </div>
  </div>
  <input type="text" class="form-control" aria-label="Text input with radio button">
</div>

  <div class="input-group-prepend">
    <span class="input-group-text" id="">First and last name</span>
  </div>
	 */
}

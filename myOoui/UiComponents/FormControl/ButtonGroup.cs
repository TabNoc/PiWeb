using TabNoc.MyOoui.Interfaces.AbstractObjects;

namespace TabNoc.MyOoui.UiComponents.FormControl
{
	internal class ButtonGroup : InputGroupControl
	{
		/*
  <div class="btn-group mr-2" role="group" aria-label="First group">
    <button type="button" class="btn btn-secondary">1</button>
    <button type="button" class="btn btn-secondary">2</button>
    <button type="button" class="btn btn-secondary">3</button>
    <button type="button" class="btn btn-secondary">4</button>
  </div>
		 */

		public ButtonGroup()
		{
			ClassName = "btn-group";
			SetAttribute("role", "group");
		}

		public void AddButton(string buttonText)
		{
			global::Ooui.Button button = new global::Ooui.Button(buttonText)
			{
				ClassName = "btn btn-secondary"
			};
			button.SetAttribute("type", "button");
			AppendChild(button);
		}
	}
}

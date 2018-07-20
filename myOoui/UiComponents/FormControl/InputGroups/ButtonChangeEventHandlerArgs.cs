namespace TabNoc.MyOoui.UiComponents.FormControl.InputGroups
{
	public class ButtonChangeEventHandlerArgs
	{
		public readonly bool OldButtonState;
		public readonly bool NewButtonState;

		public ButtonChangeEventHandlerArgs(bool oldButtonState, bool newButtonState)
		{
			OldButtonState = oldButtonState;
			NewButtonState = newButtonState;
		}
	}
}

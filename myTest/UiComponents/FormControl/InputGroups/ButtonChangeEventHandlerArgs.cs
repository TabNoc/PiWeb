namespace TabNoc.Ooui.UiComponents.FormControl.InputGroups
{
	internal class ButtonChangeEventHandlerArgs
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
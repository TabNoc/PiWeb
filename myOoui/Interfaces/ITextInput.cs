namespace TabNoc.MyOoui.Interfaces
{
	public interface ITextInput
	{
		event StringChangeEventHandler TypingText;
	}

	public delegate void StringChangeEventHandler(object sender, StringChangeEventHandlerArgs args);

	public class StringChangeEventHandlerArgs
	{
		public string _typingComputedTextValue;

		public StringChangeEventHandlerArgs(string typingComputedTextValue)
		{
			_typingComputedTextValue = typingComputedTextValue;
		}
	}
}

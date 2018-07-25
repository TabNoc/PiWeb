using TabNoc.MyOoui.Interfaces.@event;

namespace TabNoc.MyOoui.Interfaces
{
	public interface ITextInput
	{
		event StringChangeEventHandler TypingText;
	}
}

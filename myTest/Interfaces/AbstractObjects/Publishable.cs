using Ooui;

namespace TabNoc.Ooui.Interfaces.AbstractObjects
{
	public abstract class Publishable
	{
		private readonly string _publishPath;

		protected Publishable(string publishPath)
		{
			_publishPath = publishPath;
		}

		protected abstract Element PopulateAppElement();

		public void Publish()
		{
			UI.Publish(_publishPath, PopulateAppElement, true);
		}
	}
}

using Ooui;

namespace TabNoc.Ooui.PagePublisher.WateringWeb
{
	internal class ManualPagePublisher : WateringPublisher
	{
		public ManualPagePublisher (string publishPath) : base (publishPath)
		{
		}

		protected override void Initialize()
		{
			
		}

		protected override Element CreatePage()
		{
			throw new System.NotImplementedException();
		}
	}
}
namespace TabNoc.MyOoui.Interfaces.AbstractObjects
{
	public class PageData
	{
		public bool Valid;

		public static PageData CreateNew() => new PageData
		{
			Valid = true
		};
	}
}

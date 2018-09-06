using System;
using System.Collections.Generic;
using System.Text;

namespace TabNoc.PiWeb.DataTypes
{
	public class TableHeadingDefinitionAttribute : Attribute
	{
		public readonly string HeadingName;
		public readonly int Position;

		public TableHeadingDefinitionAttribute(int position, string headingName)
		{
			Position = position;
			HeadingName = headingName;
		}
	}
}

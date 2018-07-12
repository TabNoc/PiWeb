using Ooui;
using System;
using System.Collections.Generic;
using TabNoc.Ooui.Interfaces.AbstractObjects;

namespace TabNoc.Ooui.UiComponents
{
	internal class Table : StylableElement
	{
		private readonly List<string> _header;
		private readonly List<(string, List<string>)> _entries;

		/*
<table class="table table-hover">
  <thead>
    <tr>
      <th scope="col">#</th>
      <th scope="col">First</th>
      <th scope="col">Last</th>
      <th scope="col">Handle</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th scope="row">1</th>
      <td>Mark</td>
      <td>Otto</td>
      <td>@mdo</td>
    </tr>
    <tr>
      <th scope="row">2</th>
      <td>Jacob</td>
      <td>Thornton</td>
      <td>@fat</td>
    </tr>
    <tr>
      <th scope="row">3</th>
      <td colspan="2">Larry the Bird</td>
      <td>@twitter</td>
    </tr>
  </tbody>
</table>
		 */
		private readonly TableRow _headTableRow;
		private readonly TableBody _tableBody;

		public Table(List<string> header, List<(string, List<string>)> entries) : base("table")
		{
			_header = header;
			_entries = entries;
			ClassName = "table table-hover";

			TableHead head = new TableHead();
			AppendChild(head);

			_headTableRow = new TableRow();
			head.AppendChild(_headTableRow);

			foreach (string headerEnty in header)
			{
				_headTableRow.AppendChild(new TableHeadEntry("col", headerEnty));
			}

			_tableBody = new TableBody();
			AppendChild(_tableBody);
			foreach ((string tableIndex, List<string> entryList) in entries)
			{
				AppendTableRow(entryList, tableIndex);
			}
		}

		private void AppendTableRow(List<string> entryList, string tableIndex)
		{
			TableRow tableRow = new TableRow();
			_tableBody.AppendChild(tableRow);
			tableRow.AppendChild(new TableHeadEntry("row", tableIndex, false));
			foreach (string item in entryList)
			{
				tableRow.AppendChild(new TableDataEntry(item));
			}
		}

		#region SubClasses

		private class TableHead : Element
		{
			public TableHead() : base("thead")
			{
			}
		}

		private class TableBody : Element
		{
			public TableBody() : base("tbody")
			{
			}
		}

		private class TableRow : Element
		{
			public TableRow() : base("tr")
			{
			}
		}

		private class TableHeadEntry : Element
		{
			public TableHeadEntry(string scope, string value, bool addSortableIcon = true) : base("th")
			{
				if (scope != "")
				{
					SetAttribute("scope", scope);
				}
				if (value != "")
				{
					Text = value;
				}

				if (addSortableIcon)
				{
					AppendChild(new Image() {ClassName = "fas fa-sort"});
				}
			}
		}

		private class TableDataEntry : Element
		{
			public TableDataEntry(string value, int colspan = 1) : base("td")
			{
				if (value != "")
				{
					Text = value;
				}
				if (colspan != 1)
				{
					SetAttribute("colspan", colspan);
				}
			}
		}

		#endregion SubClasses

		internal class TableHeadingDefinitionAttribute : Attribute
		{
			public readonly int Position;
			public readonly string HeadingName;

			public TableHeadingDefinitionAttribute(int position, string headingName)
			{
				Position = position;
				HeadingName = headingName;
			}
		}
	}
}

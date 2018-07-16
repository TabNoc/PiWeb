using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.Ooui.HtmlElements;
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

			_tableBody = new TableBody();
			AppendChild(_tableBody);

			CreateTable(header, entries);
		}

		private void CreateTable(List<string> header, List<(string, List<string>)> entries)
		{
			foreach (string headerEnty in header)
			{
				TableHeadEntry tableHeadEntry = new TableHeadEntry("col", headerEnty, TableHeadEntry.CaretStyle.UpDown);
				tableHeadEntry.Click += TableHeadEntryOnClick;
				_headTableRow.AppendChild(tableHeadEntry);
			}

			foreach ((string tableIndex, List<string> entryList) in _entries)
			{
				AppendTableRow(entryList, tableIndex);
			}
		}

		private void TableHeadEntryOnClick(object sender, TargetEventArgs e)
		{
			TableHeadEntry tableHeadEntry = sender as TableHeadEntry;
			List<(string, List<string>)> entries = _entries.ToArray().ToList();

			if (tableHeadEntry == null)
			{
				throw new NullReferenceException();
			}
			foreach (TableHeadEntry head in _headTableRow.Children.Where(node => node is TableHeadEntry && node != tableHeadEntry).Cast<TableHeadEntry>())
			{
				head.ChangeCaret(TableHeadEntry.CaretStyle.UpDown);
			}

			int indexOf = _header.IndexOf(tableHeadEntry.Text);
			tableHeadEntry.ChangeCaret((TableHeadEntry.CaretStyle)(((int)tableHeadEntry.CurrentCaretStyle + 1) % 3));
			switch (tableHeadEntry.CurrentCaretStyle)
			{
				case TableHeadEntry.CaretStyle.Up:
				case TableHeadEntry.CaretStyle.Down:
					if (indexOf == 0)
					{
						entries.Sort((firstSet, secondSet) => String.Compare(firstSet.Item1, secondSet.Item1, StringComparison.Ordinal));
					}
					else
					{
						entries.Sort((firstSet, secondSet) =>
						{
							if (TimeSpan.TryParse(firstSet.Item2[indexOf - 1], out TimeSpan firstTimeSpan) && TimeSpan.TryParse(secondSet.Item2[indexOf - 1], out TimeSpan secondTimeSpan))
							{
								return TimeSpan.Compare(firstTimeSpan, secondTimeSpan);
							}
							else
							{
								return string.Compare(firstSet.Item2[indexOf - 1], secondSet.Item2[indexOf - 1], StringComparison.Ordinal);
							}
						});
					}

					if (tableHeadEntry.CurrentCaretStyle == TableHeadEntry.CaretStyle.Down)
					{
						entries.Reverse();
					}
					break;

				case TableHeadEntry.CaretStyle.UpDown:

					break;

				case TableHeadEntry.CaretStyle.None:
					throw new InvalidOperationException();
				default:
					throw new ArgumentOutOfRangeException();
			}

			while (_tableBody.Children.Count > 0)
			{
				_tableBody.RemoveChild(_tableBody.FirstChild);
			}
			foreach ((string tableIndex, List<string> entryList) in entries)
			{
				AppendTableRow(entryList, tableIndex);
			}
		}

		private void AppendTableRow(List<string> entryList, string tableIndex)
		{
			TableRow tableRow = new TableRow();
			_tableBody.AppendChild(tableRow);
			tableRow.AppendChild(new TableHeadEntry("row", tableIndex));
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
			public CaretStyle CurrentCaretStyle;

			public TableHeadEntry(string scope, string value, CaretStyle caretStyle = CaretStyle.None) : base("th")
			{
				CurrentCaretStyle = caretStyle;
				if (scope != "")
				{
					SetAttribute("scope", scope);
				}
				if (value != "")
				{
					Text = value;
				}

				if (caretStyle != CaretStyle.None)
				{
					switch (caretStyle)
					{
						case CaretStyle.Up:
							AppendChild(new MyIElement() { ClassName = "fas fa-sort-up float-right", Style = { LineHeight = 1.5 } });
							break;

						case CaretStyle.Down:
							AppendChild(new MyIElement() { ClassName = "fas fa-sort-down float-right", Style = { LineHeight = 1.5 } });
							break;

						case CaretStyle.UpDown:
							AppendChild(new MyIElement() { ClassName = "fas fa-sort float-right", Style = { LineHeight = 1.5 } });
							break;

						default:
							throw new ArgumentOutOfRangeException(nameof(caretStyle), caretStyle, null);
					}
				}
			}

			public void ChangeCaret(CaretStyle caretStyle)
			{
				CurrentCaretStyle = caretStyle;
				RemoveChild(Children.First(node => node is MyIElement));
				if (caretStyle != CaretStyle.None)
				{
					switch (caretStyle)
					{
						case CaretStyle.Up:
							AppendChild(new MyIElement() { ClassName = "fas fa-sort-up float-right", Style = { LineHeight = 1.5 } });
							break;

						case CaretStyle.Down:
							AppendChild(new MyIElement() { ClassName = "fas fa-sort-down float-right", Style = { LineHeight = 1.5 } });
							break;

						case CaretStyle.UpDown:
							AppendChild(new MyIElement() { ClassName = "fas fa-sort float-right", Style = { LineHeight = 1.5 } });
							break;

						default:
							throw new ArgumentOutOfRangeException(nameof(caretStyle), caretStyle, null);
					}
				}
			}

			internal enum CaretStyle
			{
				Up = 2,
				Down = 1,
				UpDown = 0,
				None = 3
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

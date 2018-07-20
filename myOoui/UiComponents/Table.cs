using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.Ooui.HtmlElements;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;

namespace TabNoc.Ooui.UiComponents
{
	public class Table : StylableElement
	{
		private readonly List<(string, List<string>)> _entries;
		private readonly List<string> _header;
		private readonly TableRow _headTableRow;
		private readonly TableRow _searchTableRow;
		private readonly TableBody _tableBody;

		#region FilterFields

		private bool _caseSensitive;
		private TableHeadEntry _filteredByTableHeadEntry;
		private List<(string, List<string>)> _filteredEntries;
		private string _filterValue;

		#endregion FilterFields

		#region sorted Fields

		private TableHeadEntry _sortedByTableHeadEntry;
		private List<(string, List<string>)> _sortedEntries;

		#endregion sorted Fields

		#region SearchField

		private List<(string, List<string>)> _searchedEntries;

		#endregion SearchField

		#region Cell Color Fields

		private readonly List<CellColoring> _cellColorings = new List<CellColoring>();
		private bool _updateMode;

		private class CellColoring
		{
			public readonly StylingColor Color;
			public readonly TableHeadEntry TableHeadEntry;
			public readonly int TableHeadIndex;
			public readonly string TextValue;

			public CellColoring(TableHeadEntry tableHeadEntry, string textValue, StylingColor color, int tableHeadIndex)
			{
				TableHeadEntry = tableHeadEntry;
				TextValue = textValue;
				Color = color;
				TableHeadIndex = tableHeadIndex;
			}
		}

		#endregion Cell Color Fields

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

		public Table(List<string> header, List<(string, List<string>)> entries, bool inUpdateMode = false) : base("table")
		{
			_header = header;
			_entries = entries;
			ClassName = "table table-hover";

			TableHead head = new TableHead();
			AppendChild(head);

			#region Table Head Row

			_headTableRow = new TableRow();
			head.AppendChild(_headTableRow);

			foreach (string headerEnty in header)
			{
				TableHeadEntry tableHeadEntry = new TableHeadEntry("col", headerEnty, TableHeadEntry.CaretStyle.UpDown);
				tableHeadEntry.Click += TableHeadEntryOnClick;
				_headTableRow.AppendChild(tableHeadEntry);
			}

			#endregion Table Head Row

			#region Table Search Row

			_searchTableRow = new TableRow();
			head.AppendChild(_searchTableRow);

			foreach (string headerEnty in header)
			{
				const bool centeredText = false;
				const string textInputGhostMessage = "Search";

				StylableTextInput textInput = new StylableTextInput();
				textInput.SetAttribute("type", "text");
				textInput.ClassName = "form-control" + (centeredText ? " text-center" : "");
				textInput.SetAttribute("placeholder", textInputGhostMessage);
				textInput.SetAttribute("aria-label", textInputGhostMessage);

				textInput.Change += (sender, args) =>
				{
					SearchTableContent();
					RedrawTableContent();
				};

				_searchTableRow.AppendChild(new TableDataEntry(textInput));
			}

			#endregion Table Search Row

			_tableBody = new TableBody();
			AppendChild(_tableBody);

			if (inUpdateMode)
			{
				StartUpdate();
			}

			ReCalculateTableContent();
		}

		public void EndUpdate()
		{
			if (_updateMode)
			{
				_updateMode = false;

				ReCalculateTableContent();
			}
		}

		public void RemoveCellValueColor(string rowHeader, string value, StylingColor color)
		{
			_cellColorings.RemoveAll(coloring => coloring.TextValue == value && coloring.Color == color && coloring.TableHeadEntry == _headTableRow.Children.Cast<TableHeadEntry>().First(node => node.Text == rowHeader));
		}

		public void SetCellValueColor(string rowHeader, string value, StylingColor color)
		{
			string coloredValue = value;
			StylingColor coloredColor = color;
			TableHeadEntry coloredByTableHeadEntry;
			int coloredByTableHeadIndex;

			if (rowHeader != "")
			{
				coloredByTableHeadEntry = _headTableRow.Children.Cast<TableHeadEntry>().First(node => node.Text == rowHeader);
				coloredByTableHeadIndex = _header.IndexOf(coloredByTableHeadEntry.Text);
			}
			else
			{
				return;
			}

			if (_cellColorings.Any(coloring => coloring.Color == color && coloring.TableHeadEntry == coloredByTableHeadEntry && coloring.TableHeadIndex == coloredByTableHeadIndex && coloring.TextValue == coloredValue))
			{
				throw new InvalidOperationException("Es CellValueColor Eintrag mit diesen Bedingungen existiert bereits!");
			}

			_cellColorings.Add(new CellColoring(coloredByTableHeadEntry, coloredValue, coloredColor, coloredByTableHeadIndex));
			ReCalculateTableContent();
		}

		public void SetFilter(string rowHeader, string value, bool caseSensitive)
		{
			_caseSensitive = caseSensitive;
			_filterValue = caseSensitive ? value : value.ToLower();
			if (rowHeader != "")
			{
				_filteredByTableHeadEntry = _headTableRow.Children.Cast<TableHeadEntry>().First(node => node.Text == rowHeader);
			}
			else
			{
				_filteredByTableHeadEntry = null;
			}

			ReCalculateTableContent();
		}

		public void StartUpdate()
		{
			_updateMode = true;
		}

		private void AppendTableRow(List<string> entryList, string tableIndex)
		{
			TableRow tableRow = new TableRow();
			_tableBody.AppendChild(tableRow);

			CellColoring headCellColoring = _cellColorings.LastOrDefault(coloring => coloring.TableHeadIndex == 0 && coloring.TextValue == tableIndex);
			if (headCellColoring != null)
			{
				tableRow.AppendChild(new TableHeadEntry("row", tableIndex, color: headCellColoring.Color));
			}
			else
			{
				tableRow.AppendChild(new TableHeadEntry("row", tableIndex));
			}

			for (int index = 0; index < entryList.Count; index++)
			{
				string item = entryList[index];
				CellColoring entryCellColoring = _cellColorings.LastOrDefault(coloring => coloring.TableHeadIndex - 1 == index && coloring.TextValue == item);
				if (entryCellColoring != null)
				{
					tableRow.AppendChild(new TableDataEntry(item, color: entryCellColoring.Color));
				}
				else
				{
					tableRow.AppendChild(new TableDataEntry(item));
				}
			}
		}

		private void FilterTableContent()
		{
			if (_updateMode)
			{
				return;
			}

			_filteredEntries = _entries.ToArray().ToList();

			if (_filteredByTableHeadEntry == null)
			{
				return;
			}
			int indexOf = _header.IndexOf(_filteredByTableHeadEntry.Text);

			if (string.IsNullOrWhiteSpace(_filterValue) == false)
			{
				if (indexOf == 0)
				{
					_filteredEntries = _filteredEntries.Where(tuple => (_caseSensitive ? tuple.Item1 : tuple.Item1.ToLower()) == _filterValue).ToList();
				}
				else
				{
					_filteredEntries = _filteredEntries.Where(tuple => (_caseSensitive ? tuple.Item2[indexOf - 1] : tuple.Item2[indexOf - 1].ToLower()) == _filterValue).ToList();
				}
			}
		}

		private void ReCalculateTableContent()
		{
			FilterTableContent();
			SortTableContent();
			SearchTableContent();
			RedrawTableContent();
		}

		private void RedrawTableContent()
		{
			if (_updateMode)
			{
				return;
			}

			while (_tableBody.Children.Count > 0)
			{
				_tableBody.RemoveChild(_tableBody.FirstChild);
			}
			foreach ((string tableIndex, List<string> entryList) in _searchedEntries)
			{
				AppendTableRow(entryList, tableIndex);
			}
		}

		private void SearchTableContent()
		{
			if (_updateMode)
			{
				return;
			}

			_searchedEntries = _sortedEntries.ToArray().ToList();

			for (int index = 0; index < _searchTableRow.Children.Count; index++)
			{
				StylableTextInput textInput = ((TableDataEntry)_searchTableRow.Children[index]).SearchTextInput;
				if (textInput.Value != "")
				{
					string searchString = textInput.Value.ToLower();
					if (index == 0)
					{
						_searchedEntries = _searchedEntries.Where(tuple => tuple.Item1.ToLower().Contains(searchString)).ToList();
					}
					else
					{
						_searchedEntries = _searchedEntries.Where(tuple => tuple.Item2[index - 1].ToLower().Contains(searchString)).ToList();
					}
				}
			}
		}

		private void SortTableContent()
		{
			if (_updateMode)
			{
				return;
			}

			_sortedEntries = _filteredEntries.ToArray().ToList();

			if (_sortedByTableHeadEntry == null)
			{
				return;
			}
			int indexOf = _header.IndexOf(_sortedByTableHeadEntry.Text);

			switch (_sortedByTableHeadEntry.CurrentCaretStyle)
			{
				case TableHeadEntry.CaretStyle.Up:
				case TableHeadEntry.CaretStyle.Down:
					if (indexOf == 0)
					{
						_sortedEntries.Sort((firstSet, secondSet) => String.Compare(firstSet.Item1, secondSet.Item1, StringComparison.Ordinal));
					}
					else
					{
						_sortedEntries.Sort((firstSet, secondSet) =>
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

					if (_sortedByTableHeadEntry.CurrentCaretStyle == TableHeadEntry.CaretStyle.Down)
					{
						_sortedEntries.Reverse();
					}
					break;

				case TableHeadEntry.CaretStyle.UpDown:

					break;

				case TableHeadEntry.CaretStyle.None:
					throw new InvalidOperationException();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void TableHeadEntryOnClick(object sender, TargetEventArgs e)
		{
			if (!(sender is TableHeadEntry tableHeadEntry))
			{
				throw new NullReferenceException();
			}
			foreach (TableHeadEntry head in _headTableRow.Children.Where(node => node is TableHeadEntry && node != tableHeadEntry).Cast<TableHeadEntry>())
			{
				head.ChangeCaret(TableHeadEntry.CaretStyle.UpDown);
			}

			_sortedByTableHeadEntry = tableHeadEntry;

			_sortedByTableHeadEntry.ChangeCaret((TableHeadEntry.CaretStyle)(((int)_sortedByTableHeadEntry.CurrentCaretStyle + 1) % 3));

			ReCalculateTableContent();
		}

		#region SubClasses

		private class TableBody : Element
		{
			public TableBody() : base("tbody")
			{
			}
		}

		private class TableDataEntry : StylableElement
		{
			public StylableTextInput SearchTextInput;

			public TableDataEntry(string value, int colspan = 1, StylingColor color = StylingColor.Light) : base("td")
			{
				if (value != "")
				{
					Text = value;
				}
				if (colspan != 1)
				{
					SetAttribute("colspan", colspan);
				}

				if (color != StylingColor.Light)
				{
					ClassName = "table-" + Enum.GetName(typeof(StylingColor), color).ToLower();
				}
			}

			public TableDataEntry(StylableTextInput content, int colspan = 1, StylingColor color = StylingColor.Light) : base("td")
			{
				if (content != null)
				{
					SearchTextInput = content;
					AppendChild(content);
				}
				if (colspan != 1)
				{
					SetAttribute("colspan", colspan);
				}

				if (color != StylingColor.Light)
				{
					ClassName = "table-" + Enum.GetName(typeof(StylingColor), color).ToLower();
				}

				AddStyling(StylingOption.PaddingTop, 1);
				AddStyling(StylingOption.PaddingBottom, 1);
				AddStyling(StylingOption.PaddingLeft, 1);
				AddStyling(StylingOption.PaddingRight, 1);
			}
		}

		private class TableHead : Element
		{
			public TableHead() : base("thead")
			{
			}
		}

		private class TableHeadEntry : Element
		{
			public CaretStyle CurrentCaretStyle;

			public TableHeadEntry(string scope, string value, CaretStyle caretStyle = CaretStyle.None, StylingColor color = StylingColor.Light) : base("th")
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
				if (color != StylingColor.Light)
				{
					ClassName = "table-" + Enum.GetName(typeof(StylingColor), color).ToLower();
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

		private class TableRow : Element
		{
			public TableRow() : base("tr")
			{
			}
		}

		#endregion SubClasses

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
}

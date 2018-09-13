using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TabNoc.MyOoui.HtmlElements;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.delegates;
using TabNoc.MyOoui.Interfaces.Enums;
using Button = TabNoc.MyOoui.HtmlElements.Button;

namespace TabNoc.MyOoui.UiComponents
{
	public class Table<T> : StylableElement, IDisposable
	{
		#region Fields

		private const int PaginationPageAmount = 10;
		private readonly List<(T, List<string>)> _cachedSearchEntries = new List<(T, List<string>)>();
		private readonly HashSet<T> _cachedSearchEntriesHashSet = new HashSet<T>();
		private readonly List<(T, List<string>)> _entries = new List<(T, List<string>)>();
		private readonly HashSet<T> _entriesHashSet = new HashSet<T>();
		private readonly FetchEntriesDelegate<T> _fetchEntries;

		/// <summary>
		/// Die Liste an HeaderNamen    verwendung in:<see cref="CreateHeaders"/>
		/// </summary>
		private readonly List<string> _header;

		private readonly TableRow _headTableRow;
		private readonly Dictionary<T, string> _primaryCellToStringDictionary = new Dictionary<T, string>();
		private readonly PrimaryKeyConverterDelegate<T> _primaryKeyConverter;
		private readonly SearchFetchEntriesDelegate<T> _searchFetchEntries;
		private readonly TableRow _searchTableRow;
		private readonly TableBody _tableBody;
		private readonly int _visibleTablePageItems = int.MaxValue;
		private int _activeTablePage = 1;
		private T _lastNormalFetchedPrimaryKey = default(T);

		/// <summary>
		/// Gibt die Menge an Entries an, die mit der normalen Sortierung linear abgerufen wurden
		/// </summary>
		private int _normalFetchedEntries = 0;

		private Pagination _pagination;
		private int _totalTableAmount;
		private bool _updateMode;
		private readonly DateTime _tableOpenDateTime;

		#endregion Fields

		#region DataFields

		#region FilterFields

		private bool _caseSensitive;
		private TableHeadEntry _filteredByTableHeadEntry;
		private List<(T, List<string>)> _filteredEntries;
		private string _filterValue;

		#endregion FilterFields

		#region sorted Fields

		private TableHeadEntry _sortedByTableHeadEntry;
		private List<(T, List<string>)> _sortedEntries;

		#endregion sorted Fields

		#region SearchField

		private List<(T, List<string>)> _searchedEntries;

		#endregion SearchField

		#region Cell Color Fields

		private readonly List<CellColoring> _cellColorings = new List<CellColoring>();

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

		#endregion DataFields

		#region Constructors

		public Table(List<string> header, List<(T, List<string>)> entries, PrimaryKeyConverterDelegate<T> primaryKeyConverter, int visibleTablePageItems = 10, bool inUpdateMode = false) : base("table")
		{
			_tableOpenDateTime = DateTime.Now;
			_header = header;
			_entries = entries;
			_primaryKeyConverter = primaryKeyConverter;
			_visibleTablePageItems = visibleTablePageItems;
			ClassName = "table table-hover";

			TableHead head = new TableHead();
			AppendChild(head);

			#region Table Head Row

			_headTableRow = new TableRow();
			head.AppendChild(_headTableRow);
			CreateHeaders();

			#endregion Table Head Row

			#region Table Search Row

			_searchTableRow = new TableRow();
			head.AppendChild(_searchTableRow);

			foreach (string _ in _header)
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
			TableFoot tableFoot = new TableFoot();
			AppendChild(tableFoot);

			_normalFetchedEntries = _totalTableAmount = entries.Count;
			int pages = (int)Math.Ceiling((decimal)_totalTableAmount / _visibleTablePageItems);
			if (pages > 1)
			{
				_pagination = new Pagination(SwitchPageCallback, pages, PaginationPageAmount);
				tableFoot.AppendChild(_pagination);
			}

			ReCalculateTableContent();
		}

		public Table(List<string> header, FetchEntriesDelegate<T> fetchEntries, SearchFetchEntriesDelegate<T> searchFetchEntries, Func<Task<int>> fetchAmount, PrimaryKeyConverterDelegate<T> primaryKeyConverter, bool inUpdateMode = false, int visibleTablePageItems = 10) : base("table")
		{
			_tableOpenDateTime = DateTime.Now;
			_header = header;
			_fetchEntries = fetchEntries;
			_searchFetchEntries = searchFetchEntries;
			_primaryKeyConverter = primaryKeyConverter;
			_visibleTablePageItems = visibleTablePageItems;
			ClassName = "table table-hover";

			TableHead head = new TableHead();
			AppendChild(head);

			#region Table Head Row

			_headTableRow = new TableRow();
			head.AppendChild(_headTableRow);
			CreateHeaders();

			#endregion Table Head Row

			#region Table Search Row

			_searchTableRow = new TableRow();
			head.AppendChild(_searchTableRow);

			for (int headerIndex = 0; headerIndex < _header.Count; headerIndex++)
			{
				const bool centeredText = false;
				const string textInputGhostMessage = "Search";

				StylableTextInput textInput = new StylableTextInput();
				textInput.SetAttribute("type", "text");
				textInput.ClassName = "form-control" + (centeredText ? " text-center" : "");
				textInput.SetAttribute("placeholder", textInputGhostMessage);
				textInput.SetAttribute("aria-label", textInputGhostMessage);

				textInput.KeyUp += (sender, args) =>
				{
					try
					{
						SearchTableContent();
						RedrawTableContent();
					}
					catch (Exception e)
					{
						Logging.Error("Beim ausführen von textInput.KeyUp ist ein Fehler aufgetreten!", e);
						throw;
					}
				};

				int index = headerIndex;
				textInput.Change += (sender, args) =>
				{
					try
					{
						SearchTableContent();
						RedrawTableContent();
						QuerySearch(((StylableTextInput)sender).Value, index);
					}
					catch (Exception e)
					{
						Logging.Error("Beim ausführen von textInput.Change ist ein Fehler aufgetreten!", e);
						throw;
					}
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
			TableFoot tableFoot = new TableFoot();
			AppendChild(tableFoot);

			Task<List<(T, List<string>)>> entriesTask = _fetchEntries(_lastNormalFetchedPrimaryKey, PaginationPageAmount * _visibleTablePageItems);
			fetchAmount().ContinueWith(task =>
			{
				try
				{
					_totalTableAmount = task.Result;
					int pages = (int)Math.Ceiling((decimal)task.Result / _visibleTablePageItems);
					if (pages > 1)
					{
						_pagination = new Pagination(SwitchPageCallback, pages, PaginationPageAmount);
						tableFoot.AppendChild(_pagination);
					}

					if (AddEntriesToCache(entriesTask, true))
					{
						ReCalculateTableContent();
					}
				}
				catch (Exception e)
				{
					Logging.Error("Beim ausführen von Table._fetchAmount ist ein Fehler aufgetreten!", e);
					throw;
				}
			});

			Task.Run(() =>
			{
				while (_disposed == false)
				{
					RefreshBeginningTableContentAsync();
					System.Threading.Thread.Sleep(10000);
					if (_tableOpenDateTime.AddMinutes(-30) > DateTime.Now)
					{
						Console.WriteLine("Die Tabelle ist länger als 30 Minuten offen -> der Refresh wird gestoppt");
						return;
					}
				}
				Console.WriteLine("Table was Disposed -> finish refresh");
			});
		}

		#endregion Constructors

		#region Public Methods

		public void EndUpdate()
		{
			if (_updateMode)
			{
				_updateMode = false;

				ReCalculateTableContent();
			}
		}

		public void RefreshBeginningTableContent()
		{
			_fetchEntries(default(T), 10).ContinueWith(task =>
			{
				lock (_entries)
				{
					try
					{
						if (AddEntriesToCache(task, false, true))
						{
							ReCalculateTableContent();
						}
					}
					catch (Exception e)
					{
						Logging.Error("Beim ausführen von Table.RefreshBeginningTableContent ist ein Fehler aufgetreten!", e);
						throw;
					}
				}
			}).Wait();
		}

		public void RefreshBeginningTableContentAsync()
		{
			_fetchEntries(default(T), 10).ContinueWith(task =>
			{
				lock (_entries)
				{
					try
					{
						if (AddEntriesToCache(task, false, true))
						{
							ReCalculateTableContent();
						}
					}
					catch (Exception e)
					{
						Logging.Error("Beim ausführen von Table.RefreshBeginningTableContentAsync ist ein Fehler aufgetreten!", e);
						throw;
					}
				}
			});
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

			QuerySearch(_filterValue, _header.IndexOf(rowHeader));
		}

		public void StartUpdate()
		{
			_updateMode = true;
		}

		#endregion Public Methods

		#region Private Methods

		private bool AddEntriesToCache(Task<List<(T, List<string>)>> task, bool wasNormalFetch = false, bool wasRefreshFetch = false)
		{
			bool changed = false;
			lock (_entries)
			{
				List<(T, List<string>)> additionalEntries = task.Result.Where(tuple =>
						_entriesHashSet.Contains(tuple.Item1) == false && wasNormalFetch == true ||
						(_entriesHashSet.Contains(tuple.Item1) == false && _cachedSearchEntriesHashSet.Contains(tuple.Item1) == false) && wasNormalFetch == false)
					.ToList();
				foreach ((T, List<string>) entry in additionalEntries)
				{
					if (entry.Item1.Equals(default(T)) || entry.Item2 == null)
					{
						throw new NullReferenceException("Mindestends ein abgerufener Datensatz hat nur standardWerte!");
					}

					if (wasNormalFetch || wasRefreshFetch)
					{
						if (wasNormalFetch)
						{
							_entries.Add(entry);
						}
						_entriesHashSet.Add(entry.Item1);

						if (_cachedSearchEntriesHashSet.Contains(entry.Item1))
						{
							_cachedSearchEntriesHashSet.Remove(entry.Item1);
							_cachedSearchEntries.Remove(entry);
						}
					}
					else
					{
						_cachedSearchEntries.Add(entry);
						_cachedSearchEntriesHashSet.Add(entry.Item1);
					}

					changed = true;
				}

				if (wasRefreshFetch)
				{
					_entries.InsertRange(0, additionalEntries);
				}
			}

			if (wasNormalFetch && task.Result.Count > 0)
			{
				_lastNormalFetchedPrimaryKey = task.Result.Last().Item1;
				_normalFetchedEntries += task.Result.Count;
			}

			return changed;
		}

		private void CreateHeaders()
		{
			while (_headTableRow.FirstChild != null)
			{
				_headTableRow.RemoveChild(_headTableRow.FirstChild);
			}
			foreach (string headerEnty in _header)
			{
				TableHeadEntry tableHeadEntry = new TableHeadEntry("col", headerEnty, TableHeadEntry.CaretStyle.UpDown);
				tableHeadEntry.Click += TableHeadEntryOnClick;
				_headTableRow.AppendChild(tableHeadEntry);
			}

			if (HasButtonColumn)
			{
				TableHeadEntry tableHeadEntry = new TableHeadEntry("col", _buttonColumnHeaderName);
				_headTableRow.AppendChild(tableHeadEntry);
			}
		}

		private void FilterTableContent()
		{
			if (_updateMode)
			{
				return;
			}

			_filteredEntries = new List<(T, List<string>)>();
			_filteredEntries.AddRange(_entries);
			_filteredEntries.AddRange(_cachedSearchEntries);

			if (_filteredByTableHeadEntry == null)
			{
				return;
			}
			int indexOf = _header.IndexOf(_filteredByTableHeadEntry.Text);

			if (string.IsNullOrWhiteSpace(_filterValue) == false)
			{
				if (indexOf == 0)
				{
					_filteredEntries = _filteredEntries.Where(tuple =>
					{
						string primaryCellValue = GetValueFromPrimaryCell(tuple.Item1);
						return (_caseSensitive ? primaryCellValue : primaryCellValue.ToLower()) == _filterValue;
					}).ToList();
				}
				else
				{
					_filteredEntries = _filteredEntries.Where(tuple => (_caseSensitive ? tuple.Item2[indexOf - 1] : tuple.Item2[indexOf - 1].ToLower()) == _filterValue).ToList();
				}
			}
		}

		private string GetValueFromPrimaryCell(T cellValue)
		{
			if (!_primaryCellToStringDictionary.ContainsKey(cellValue))
			{
				_primaryCellToStringDictionary.Add(cellValue, _primaryKeyConverter(cellValue));
			}

			return _primaryCellToStringDictionary[cellValue];
		}

		private void QuerySearch(string searchString, int headerIndex)
		{
			if (headerIndex < 0)
			{
				return;
			}
			int neededTotalTablePageItems = PaginationPageAmount * _visibleTablePageItems;
			if (neededTotalTablePageItems > _searchedEntries.Count && _normalFetchedEntries < _totalTableAmount)
			{
				_searchFetchEntries(searchString, headerIndex, neededTotalTablePageItems - _searchedEntries.Count).ContinueWith(task =>
				{
					try
					{
						if (AddEntriesToCache(task, false))
						{
							ReCalculateTableContent();
						}
					}
					catch (Exception e)
					{
						Logging.Error("Beim ausführen von Table.QuerySearch ist ein Fehler aufgetreten!", e);
						throw;
					}
				});
			}
		}

		private void ReCalculateTableContent()
		{
			lock (_entries)
			{
				if (_entries == null)
				{
					return;
				}
				FilterTableContent();
				SortTableContent();
				SearchTableContent();
				RedrawTableContent();
			}
		}

		private void RedrawTableContent()
		{
			if (_updateMode)
			{
				return;
			}

			_pagination?.ChangeMaxPages((int)Math.Ceiling((decimal)_searchedEntries.Count / _visibleTablePageItems));

			while (_tableBody.Children.Count > 0)
			{
				_tableBody.RemoveChild(_tableBody.FirstChild);
			}
			foreach ((T tableIndex, List<string> entryList) in _searchedEntries.Skip((_activeTablePage - 1) * _visibleTablePageItems).Take(_visibleTablePageItems))
			{
				WriteTableRow(entryList, tableIndex);
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
						_searchedEntries = _searchedEntries.Where(tuple => GetValueFromPrimaryCell(tuple.Item1).ToLower().Contains(searchString)).ToList();
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
						_sortedEntries.Sort((firstSet, secondSet) => String.Compare(GetValueFromPrimaryCell(firstSet.Item1), GetValueFromPrimaryCell(secondSet.Item1), StringComparison.Ordinal));
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

		private void SwitchPageCallback(int newPage)
		{
			_activeTablePage = newPage;
			RedrawTableContent();
			int neededTotalTablePageItems = (newPage + (int)Math.Floor((decimal)(PaginationPageAmount - 0.5) / 2)) * _visibleTablePageItems;
			if (neededTotalTablePageItems > _normalFetchedEntries && _normalFetchedEntries < _totalTableAmount)
			{
				_fetchEntries(_lastNormalFetchedPrimaryKey, neededTotalTablePageItems - _normalFetchedEntries).ContinueWith(task =>
				{
					try
					{
						if (AddEntriesToCache(task, true))
						{
							ReCalculateTableContent();
						}
					}
					catch (Exception e)
					{
						Logging.Error("Beim ausführen von Table.SwitchPageCallback ist ein Fehler aufgetreten!", e);
						throw;
					}
				});
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

		private void WriteTableRow(List<string> entryList, T tableIndex)
		{
			TableRow tableRow = new TableRow();
			_tableBody.AppendChild(tableRow);

			CellColoring headCellColoring = _cellColorings.LastOrDefault(coloring => coloring.TableHeadIndex == 0 && coloring.TextValue == GetValueFromPrimaryCell(tableIndex));
			if (headCellColoring != null)
			{
				tableRow.AppendChild(new TableHeadEntry("row", GetValueFromPrimaryCell(tableIndex), color: headCellColoring.Color));
			}
			else
			{
				tableRow.AppendChild(new TableHeadEntry("row", GetValueFromPrimaryCell(tableIndex)));
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

			if (HasButtonColumn)
			{
				tableRow.AppendChild(new TableDataEntry(GetButtonColumnButton(tableIndex)));
			}
		}

		#endregion Private Methods

		#region ButtonColumn

		private readonly Dictionary<T, Button> _buttonColumnEntries = new Dictionary<T, Button>();
		private string _buttonColumnHeaderName;
		private Func<T, Button> _generateButtonColumnFunc;
		private bool HasButtonColumn => _generateButtonColumnFunc != null;

		public void SetButtonColumn(string headerName, Func<T, Button> generateButtonFunc)
		{
			if (_generateButtonColumnFunc != null)
			{
				throw new InvalidOperationException("Es wurde bereits SetButtonColumn für diese Tabelle aufgerufen!");
			}

			_generateButtonColumnFunc = generateButtonFunc;
			_buttonColumnHeaderName = headerName;
			CreateHeaders();
			ReCalculateTableContent();
		}

		private Button GetButtonColumnButton(T row)
		{
			if (!_buttonColumnEntries.ContainsKey(row))
			{
				_buttonColumnEntries.Add(row, _generateButtonColumnFunc(row));
			}

			return _buttonColumnEntries[row];
		}

		#endregion ButtonColumn

		#region Dispose Pattern

		private bool _disposed;

		~Table()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		public new void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
				base.Dispose();
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (_disposed)
					return;

				_disposed = true;
				base.Dispose(disposing);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		#endregion Dispose Pattern

		#region SubClasses

		private class TableBody : Element
		{
			public TableBody() : base("tbody")
			{
			}
		}

		private sealed class TableDataEntry : StylableElement
		{
			public readonly StylableTextInput SearchTextInput;

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

			public TableDataEntry(Button content, int colspan = 1, StylingColor color = StylingColor.Light) : base("td")
			{
				if (content != null)
				{
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

		private class TableFoot : Element
		{
			public TableFoot() : base("tfoot")
			{
			}
		}

		private class TableHead : Element
		{
			public TableHead() : base("thead")
			{
			}
		}

		private sealed class TableHeadEntry : Element
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
	}
}

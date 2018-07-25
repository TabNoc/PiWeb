using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.MyOoui.UiComponents;
using TabNoc.MyOoui.UiComponents.FormControl;
using TabNoc.PiWeb.Storage.WateringWeb.History;
using TabNoc.PiWeb.Storage.WateringWeb.Manual;
using Button = TabNoc.MyOoui.HtmlElements.Button;

namespace TabNoc.PiWeb.Pages.WateringWeb.History
{
	internal class HistoryPage : StylableElement
	{
		private const bool UseServerApiQuery = false;

		public HistoryPage() : base("div")
		{
			Container wrappingContainer = new Container();
			Grid grid = new Grid(wrappingContainer);

			Dropdown messageKindDropdown = new Dropdown(new Button(StylingColor.Light, false, text: "Quelle auswählen!"));
			grid.AddRow().AppendCollum(messageKindDropdown, autoSize: true);
			messageKindDropdown.AddStyling(StylingOption.MarginBottom, 4);

			// Table historyTable = new Table(new List<string>() { "Zeitpunkt", "Status", "Quelle", "Meldung" }, CreateHistoryTableContent(), true);
			Table<DateTime> historyTable = new Table<DateTime>(new List<string>() { "Zeitpunkt", "Status", "Quelle", "Meldung" }, FetchEntries, FetchSearchEntries, FetchAmount, PrimaryCellConverter, true, 14);
			grid.AddRow().AppendCollum(historyTable);

			messageKindDropdown.AddEntry("Alles").Click += (sender, args) =>
			{
				historyTable.SetFilter("", "", true);
				messageKindDropdown.Button.Text = "Alles anzeigen";
			};
			messageKindDropdown.AddDivider();
			messageKindDropdown.AddEntry("System").Click += (sender, args) =>
			{
				historyTable.SetFilter("Quelle", "System", true);
				messageKindDropdown.Button.Text = "System-Meldungen anzeigen";
			};
			messageKindDropdown.AddEntry("Automatikbetrieb").Click += (sender, args) =>
			{
				historyTable.SetFilter("Quelle", "Automatic", true);
				messageKindDropdown.Button.Text = "Automatikbetrieb-Meldungen anzeigen";
			};
			messageKindDropdown.AddEntry("Handbetrieb").Click += (sender, args) =>
			{
				historyTable.SetFilter("Quelle", "Manual", true);
				messageKindDropdown.Button.Text = "Handbetrieb-Meldungen anzeigen";
			};

			historyTable.SetCellValueColor("Status", "OK", StylingColor.Success);
			historyTable.SetCellValueColor("Status", "Warnung", StylingColor.Warning);
			historyTable.SetCellValueColor("Status", "Fehler", StylingColor.Danger);

			historyTable.EndUpdate();
			AppendChild(wrappingContainer);
		}

		private string PrimaryCellConverter(DateTime primaryKey)
		{
			return primaryKey.ToShortDateString() + " " + primaryKey.ToLongTimeString();
		}

		private Task<List<(DateTime, List<string>)>> FetchSearchEntries(string searchstring, int collumn, int amount)
		{
			if (UseServerApiQuery)
			{
				throw new NotImplementedException();
			}
			else
			{
				return Task.Run(() => PageStorage<HistoryData>.Instance.StorageData.HistoryElements
					.Where(element => GetElementCollumn(element, collumn).Contains(searchstring)).Take(amount).Select(historyElement =>
						(historyElement.TimeStamp, new List<string>() { historyElement.Status, historyElement.Source, historyElement.Message }))
					.ToList());
				;
			}
		}

		private string GetElementCollumn(HistoryElement element, int collumn)
		{
			switch (collumn)
			{
				case 0:
					return PrimaryCellConverter(element.TimeStamp);

				case 1:
					return element.Status;

				case 2:
					return element.Source;

				case 3:
					return element.Message;

				default:
					throw new IndexOutOfRangeException();
			}
		}

		private Task<int> FetchAmount()
		{
			if (UseServerApiQuery)
			{
				throw new NotImplementedException();
			}
			else
			{
				return Task.Run(() => PageStorage<HistoryData>.Instance.StorageData.HistoryElements.Count);
			}
		}

		private Task<List<(DateTime, List<string>)>> FetchEntries(DateTime primaryKey, int takeAmount)
		{
			if (UseServerApiQuery)
			{
				throw new NotImplementedException();
			}
			else
			{
				if (primaryKey == default(DateTime))
				{
					return Task.Run(() =>
						PageStorage<HistoryData>.Instance.StorageData.HistoryElements.Take(takeAmount)
							.Select(historyElement => (historyElement.TimeStamp,
								new List<string>()
								{
									historyElement.Status,
									historyElement.Source,
									historyElement.Message
								})).ToList());
				}
				else
				{
					return Task.Run(() =>
						PageStorage<HistoryData>.Instance.StorageData.HistoryElements
							.SkipWhile(element => element.TimeStamp != primaryKey).Take(takeAmount)
							.Select(historyElement => (historyElement.TimeStamp,
								new List<string>()
								{
									historyElement.Status,
									historyElement.Source,
									historyElement.Message
								})).ToList());
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			PageStorage<ManualData>.Instance.Save();
			base.Dispose(disposing);
		}

		private List<(string, List<string>)> CreateHistoryTableContent()
		{
			throw new NotSupportedException();
			List<(string, List<string>)> returnval = new List<(string, List<string>)>();
			foreach (HistoryElement historyElement in PageStorage<HistoryData>.Instance.StorageData.HistoryElements)
			{
				returnval.Add((
					historyElement.TimeStamp.ToShortDateString() + " " + historyElement.TimeStamp.ToLongTimeString(),
					new List<string>() { historyElement.Status, historyElement.Source, historyElement.Message }));
			}

			return returnval;
		}
	}
}

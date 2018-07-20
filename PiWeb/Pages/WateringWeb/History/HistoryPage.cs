using System.Collections.Generic;
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
		public HistoryPage() : base("div")
		{
			Container wrappingContainer = new Container();
			Grid grid = new Grid(wrappingContainer);

			Dropdown messageKindDropdown = new Dropdown(new Button(StylingColor.Light, false, text: "Quelle auswählen!"));
			grid.AddRow().AppendCollum(messageKindDropdown, autoSize: true);
			messageKindDropdown.AddStyling(StylingOption.MarginBottom, 4);

			Table historyTable = new Table(new List<string>() { "Zeitpunkt", "Status", "Quelle", "Meldung" }, CreateHistoryTableContent(), true);
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

		private List<(string, List<string>)> CreateHistoryTableContent()
		{
			List<(string, List<string>)> returnval = new List<(string, List<string>)>();
			foreach (AutomaticHistoryElement automaticHistoryElement in PageStorage<HistoryData>.Instance.StorageData.AutomaticHistoryElements)
			{
				returnval.Add((
					automaticHistoryElement.TimeStamp.ToShortTimeString() + " " +
					automaticHistoryElement.TimeStamp.ToShortDateString(),
					new List<string>() { automaticHistoryElement.Status, "Automatic", automaticHistoryElement.Message }));
			}
			foreach (ManualHistoryElement manualHistoryElement in PageStorage<HistoryData>.Instance.StorageData.ManualHistoryElements)
			{
				returnval.Add((
					manualHistoryElement.TimeStamp.ToShortTimeString() + " " +
					manualHistoryElement.TimeStamp.ToShortDateString(),
					new List<string>() { manualHistoryElement.Status, "Manual", manualHistoryElement.Message }));
			}
			foreach (SystemHistoryElement systemHistoryElement in PageStorage<HistoryData>.Instance.StorageData.SystemHistoryElements)
			{
				returnval.Add((
					systemHistoryElement.TimeStamp.ToShortTimeString() + " " +
					systemHistoryElement.TimeStamp.ToShortDateString(),
					new List<string>() { systemHistoryElement.Status, "System", systemHistoryElement.Message }));
			}

			return returnval;
		}

		protected override void Dispose(bool disposing)
		{
			PageStorage<ManualData>.Instance.Save();
			base.Dispose(disposing);
		}
	}
}

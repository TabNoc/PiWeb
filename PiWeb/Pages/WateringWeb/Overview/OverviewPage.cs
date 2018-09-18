using Ooui;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.MyOoui.UiComponents;
using TabNoc.PiWeb.DataTypes;
using TabNoc.PiWeb.DataTypes.WateringWeb.Overview;
using Button = TabNoc.MyOoui.HtmlElements.Button;

namespace TabNoc.PiWeb.Pages.WateringWeb.Overview
{
	internal class OverviewPage : StylableElement
	{
		public OverviewPage() : base("div")
		{
			Container wrappingContainer = new Container();
			Grid grid = new Grid(wrappingContainer);

			grid.AddRow().AppendCollum(new Heading(2, "Automatische Aufträge") { ClassName = "text-center" });

			Table<string> automaticTable = new Table<string>(CreateAutomaticTableHeading(), CreateAutomaticTableBody(), value => value, 4);
			grid.AddRow().AppendCollum(automaticTable);

			grid.AddRow().AppendCollum(new Heading(2, "Manuelle Aufträge") { ClassName = "text-center" });

			Table<string> manualTable = new Table<string>(CreateManualTableHeading(), CreateManualTableBody(), value => value, 5);
			manualTable.SetButtonColumn("Löschen", s =>
			{
				Button button = new Button(StylingColor.Danger, true, Button.ButtonSize.Small, fontAwesomeIcon: "trash");
				button.Click += (sender, args) => ServerConnection.DeleteAsync($"overview", $"deleteManualEntry?number={s}");
				return button;
			});
			grid.AddRow().AppendCollum(manualTable);
			AppendChild(wrappingContainer);
		}

		private List<(string, List<string>)> CreateAutomaticTableBody()
		{
			List<(string, List<string>)> returnval = new List<(string, List<string>)>();
			OverviewData data = PageStorage<OverviewData>.Instance.StorageData;
			foreach (AutomaticOverviewEntry automaticOverviewEntry in data.AutomaticOverviewEntries)
			{
				Dictionary<int, string> entryDictionary = new Dictionary<int, string>();
				foreach (FieldInfo fieldInfo in automaticOverviewEntry.GetType().GetFields().ToList())
				{
					TableHeadingDefinitionAttribute attribute = fieldInfo.GetCustomAttributes(typeof(TableHeadingDefinitionAttribute), false).Cast<TableHeadingDefinitionAttribute>().First();
					entryDictionary.Add(attribute.Position, fieldInfo.GetValue(automaticOverviewEntry).ToString());
				}

				List<string> entryList = new List<string>();
				for (int i = 1; i < entryDictionary.Keys.Max() + 1; i++)
				{
					entryList.Add(entryDictionary[i]);
				}
				returnval.Add((entryDictionary[0], entryList));
			}

			return returnval;
		}

		private List<string> CreateAutomaticTableHeading()
		{
			List<string> list = new List<string>();
			List<TableHeadingDefinitionAttribute> attributes = new List<TableHeadingDefinitionAttribute>();

			foreach (FieldInfo fieldInfo in typeof(AutomaticOverviewEntry).GetFields().ToList())
			{
				attributes.Add(fieldInfo.GetCustomAttributes(typeof(TableHeadingDefinitionAttribute), false).Cast<TableHeadingDefinitionAttribute>().First());
			}

			attributes.Sort((TableHeadingDefinitionAttribute o, TableHeadingDefinitionAttribute o1) =>
				o.Position.CompareTo(o1.Position));

			foreach (TableHeadingDefinitionAttribute attribute in attributes)
			{
				list.Add(attribute.HeadingName);
			}

			return list;
		}

		private List<(string, List<string>)> CreateManualTableBody()
		{
			List<(string, List<string>)> returnval = new List<(string, List<string>)>();
			OverviewData data = PageStorage<OverviewData>.Instance.StorageData;
			foreach (ManualOverviewEntry manualOverviewEntry in data.ManualOverviewEntries)
			{
				Dictionary<int, string> entryDictionary = new Dictionary<int, string>();
				foreach (FieldInfo fieldInfo in manualOverviewEntry.GetType().GetFields().ToList())
				{
					TableHeadingDefinitionAttribute attribute = fieldInfo.GetCustomAttributes(typeof(TableHeadingDefinitionAttribute), false).Cast<TableHeadingDefinitionAttribute>().First();
					entryDictionary.Add(attribute.Position, fieldInfo.GetValue(manualOverviewEntry).ToString());
				}

				List<string> entryList = new List<string>();
				for (int i = 1; i < entryDictionary.Keys.Max() + 1; i++)
				{
					entryList.Add(entryDictionary[i]);
				}
				returnval.Add((entryDictionary[0], entryList));
			}

			return returnval;
		}

		private List<string> CreateManualTableHeading()
		{
			List<string> list = new List<string>();
			List<TableHeadingDefinitionAttribute> attributes = new List<TableHeadingDefinitionAttribute>();

			foreach (FieldInfo fieldInfo in typeof(ManualOverviewEntry).GetFields().ToList())
			{
				attributes.Add(fieldInfo.GetCustomAttributes(typeof(TableHeadingDefinitionAttribute), false).Cast<TableHeadingDefinitionAttribute>().First());
			}

			attributes.Sort((TableHeadingDefinitionAttribute o, TableHeadingDefinitionAttribute o1) =>
				o.Position.CompareTo(o1.Position));

			foreach (TableHeadingDefinitionAttribute attribute in attributes)
			{
				list.Add(attribute.HeadingName);
			}

			return list;
		}
	}
}

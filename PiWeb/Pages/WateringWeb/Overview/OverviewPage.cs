using Ooui;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.UiComponents;
using TabNoc.PiWeb.Storage.WateringWeb.Overview;

namespace TabNoc.PiWeb.Pages.WateringWeb.Overview
{
	internal class OverviewPage : StylableElement
	{
		public OverviewPage() : base("div")
		{
			Container wrappingContainer = new Container();
			Grid grid = new Grid(wrappingContainer);

			Table table = new Table(CreateAutomaticTableHeading(), CreateAutomaticTableBody());
			grid.AddRow().AppendCollum(table);

			grid.AddRow().AppendCollum(new Heading(2, "Manuelle Aufträge") { ClassName = "text-center" });

			Table table2 = new Table(CreateManualTableHeading(), CreateManualTableBody());
			grid.AddRow().AppendCollum(table2);
			AppendChild(wrappingContainer);
		}

		private List<string> CreateManualTableHeading()
		{
			List<string> list = new List<string>();
			List<Table.TableHeadingDefinitionAttribute> attributes = new List<Table.TableHeadingDefinitionAttribute>();

			foreach (FieldInfo fieldInfo in typeof(ManualOverviewEntry).GetFields().ToList())
			{
				attributes.Add(fieldInfo.GetCustomAttributes(typeof(Table.TableHeadingDefinitionAttribute), false).Cast<Table.TableHeadingDefinitionAttribute>().First());
			}

			attributes.Sort((Table.TableHeadingDefinitionAttribute o, Table.TableHeadingDefinitionAttribute o1) =>
				o.Position.CompareTo(o1.Position));

			foreach (Table.TableHeadingDefinitionAttribute attribute in attributes)
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
					Table.TableHeadingDefinitionAttribute attribute = fieldInfo.GetCustomAttributes(typeof(Table.TableHeadingDefinitionAttribute), false).Cast<Table.TableHeadingDefinitionAttribute>().First();
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

		private List<(string, List<string>)> CreateAutomaticTableBody()
		{
			List<(string, List<string>)> returnval = new List<(string, List<string>)>();
			OverviewData data = PageStorage<OverviewData>.Instance.StorageData;
			foreach (AutomaticOverviewEntry automaticOverviewEntry in data.AutomaticOverviewEntries)
			{
				Dictionary<int, string> entryDictionary = new Dictionary<int, string>();
				foreach (FieldInfo fieldInfo in automaticOverviewEntry.GetType().GetFields().ToList())
				{
					Table.TableHeadingDefinitionAttribute attribute = fieldInfo.GetCustomAttributes(typeof(Table.TableHeadingDefinitionAttribute), false).Cast<Table.TableHeadingDefinitionAttribute>().First();
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
			List<Table.TableHeadingDefinitionAttribute> attributes = new List<Table.TableHeadingDefinitionAttribute>();

			foreach (FieldInfo fieldInfo in typeof(AutomaticOverviewEntry).GetFields().ToList())
			{
				attributes.Add(fieldInfo.GetCustomAttributes(typeof(Table.TableHeadingDefinitionAttribute), false).Cast<Table.TableHeadingDefinitionAttribute>().First());
			}

			attributes.Sort((Table.TableHeadingDefinitionAttribute o, Table.TableHeadingDefinitionAttribute o1) =>
				o.Position.CompareTo(o1.Position));

			foreach (Table.TableHeadingDefinitionAttribute attribute in attributes)
			{
				list.Add(attribute.HeadingName);
			}

			return list;
		}
	}
}

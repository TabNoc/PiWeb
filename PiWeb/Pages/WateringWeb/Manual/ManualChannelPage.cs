using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.MyOoui.UiComponents;
using TabNoc.MyOoui.UiComponents.FormControl.InputGroups;
using TabNoc.PiWeb.Storage.WateringWeb.Channels;
using TabNoc.PiWeb.Storage.WateringWeb.Manual;
using Button = TabNoc.MyOoui.HtmlElements.Button;

namespace TabNoc.PiWeb.Pages.WateringWeb.Manual
{
	internal class ManualChannelPage : StylableElement
	{
		private ChannelData _channel;

		public ManualChannelPage(ChannelData channel, ManualPage parent) : base("div")
		{
			const int labelSize = 229;
			_channel = channel;
			SetBorder(BorderKind.Rounded, StylingColor.Secondary);

			#region ExecuteAction

			#region Initialize Grid

			Grid grid = new Grid(this);
			grid.AddStyling(StylingOption.MarginRight, 2);
			grid.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddStyling(StylingOption.MarginTop, 4);
			grid.AddStyling(StylingOption.MarginBottom, 2);

			#endregion Initialize Grid

			#region Init Container

			Container firstContainer = new Container();
			firstContainer.SetBorder(BorderKind.Rounded, StylingColor.Info);
			firstContainer.AddStyling(StylingOption.MarginTop, 3);
			firstContainer.AddStyling(StylingOption.MarginBottom, 1);
			firstContainer.AddStyling(StylingOption.PaddingTop, 3);
			firstContainer.AddStyling(StylingOption.PaddingBottom, 2);
			grid.AddRow().AppendCollum(firstContainer, autoSize: true);

			#endregion Init Container

			#region create Heading

			Heading firstHeading = new Heading(5, "Kanal aktivieren ...");
			firstContainer.AppendChild(firstHeading);

			#endregion create Heading

			#region duration Input

			MultiInputGroup durationMultiInputGroup = new MultiInputGroup();
			durationMultiInputGroup.AppendLabel("Dauer", labelSize);
			StylableTextInput durationTextInput = durationMultiInputGroup.AppendTextInput("hh:mm:ss");
			durationTextInput.Value = "00:00:00";
			firstContainer.AppendChild(durationMultiInputGroup);
			durationMultiInputGroup.AddStyling(StylingOption.MarginBottom, 2);
			durationMultiInputGroup.AppendValidation("", "Das Format der Dauer muss hh:mm:ss sein!", true);

			#endregion duration Input

			#region Master ButtonGroup

			MultiInputGroup masterEnabledMultiInputGroup = new MultiInputGroup();
			masterEnabledMultiInputGroup.AppendLabel("Master Aktivieren", labelSize);
			TwoStateButtonGroup masterEnabledTwoStateButtonGroup = masterEnabledMultiInputGroup.AppendCustomElement(new TwoStateButtonGroup("Ja", "Nein", true, false), false);
			firstContainer.AppendChild(masterEnabledMultiInputGroup);
			masterEnabledMultiInputGroup.AddStyling(StylingOption.MarginBottom, 2);

			#endregion Master ButtonGroup

			#region StartButton

			Button startButton = new Button(StylingColor.Success, true, text: "Starten!", fontAwesomeIcon: "play", asBlock: true);
			firstContainer.AppendChild(startButton);
			startButton.Click += (o, args) =>
			{
				if (durationTextInput.Value.Split(':').Length != 3 || durationTextInput.Value.Split(':').Any(s => int.TryParse(s, out _) == false))
				{
					durationTextInput.SetValidation(false, true);
				}
				else
				{
					durationTextInput.SetValidation(true, false);
					startButton.IsDisabled = true;
					try
					{
						CreateChannelAction(channel, TimeSpan.Parse(durationTextInput.Value), masterEnabledTwoStateButtonGroup.FirstButtonActive, 100);

						startButton.Text = "Gestartet";
					}
					catch (Exception)
					{
						startButton.Text = "Start fehlgeschlagen";
						throw;
					}
				}
			};
			firstContainer.AppendChild(startButton);
			startButton.AddStyling(StylingOption.MarginBottom, 2);

			#endregion StartButton

			#endregion ExecuteAction

			#region AddToBatch

			#region Init Container

			Container secondContainer = new Container();
			secondContainer.SetBorder(BorderKind.Rounded, StylingColor.Info);
			secondContainer.AddStyling(StylingOption.MarginTop, 3);
			secondContainer.AddStyling(StylingOption.MarginBottom, 1);
			secondContainer.AddStyling(StylingOption.PaddingTop, 3);
			secondContainer.AddStyling(StylingOption.PaddingBottom, 2);
			grid.AddRow().AppendCollum(secondContainer, autoSize: true);

			#endregion Init Container

			#region create Heading

			Heading heading = new Heading(5, "... als Batch-Auftrag speichern");
			secondContainer.AppendChild(heading);

			#endregion create Heading

			#region batchName Input

			MultiInputGroup batchNameMultiInputGroup = new MultiInputGroup();
			batchNameMultiInputGroup.AddStyling(StylingOption.MarginTop, 4);
			batchNameMultiInputGroup.AppendLabel("Name für den Batch-Auftrag:");
			StylableTextInput batchNameTextInput = batchNameMultiInputGroup.AppendTextInput("Name?");
			batchNameMultiInputGroup.AppendValidation("", "Der Name ist bereits vergeben", true);
			secondContainer.AppendChild(batchNameMultiInputGroup);

			#endregion batchName Input

			Button appendToBatchButton = new Button(StylingColor.Success, true, text: "Als Batch-Auftrag speichern", fontAwesomeIcon: "save", asBlock: true);
			appendToBatchButton.AddStyling(StylingOption.MarginTop, 2);
			appendToBatchButton.Click += (sender, args) =>
			{
				if (PageStorage<ManualData>.Instance.StorageData.BatchEntries.Any(entry => entry.Name == batchNameTextInput.Value))
				{
					batchNameTextInput.SetValidation(false, true);
				}
				else
				{
					batchNameTextInput.SetValidation(true, false);
					PageStorage<ManualData>.Instance.StorageData.BatchEntries.Add(new BatchEntry(batchNameTextInput.Value, channel.ChannelId, TimeSpan.Parse(durationTextInput.Value), masterEnabledTwoStateButtonGroup.FirstButtonActive, 100, PageStorage<ManualData>.Instance.StorageData.GetUniqueID()));
					parent.UpdateBatch();
				}
			};
			secondContainer.AppendChild(appendToBatchButton);

			#endregion AddToBatch
		}

		private static void CreateChannelAction(ChannelData channel, TimeSpan duration, bool activateMasterChannel, int durationOverride = 100)
		{
			PageStorage<ManualActionExecutionData>.Instance.StorageData.ExecutionList = new List<ManualActionExecutionData.ManualActionExecution>()
			{
				new ManualActionExecutionData.ManualActionExecution(channel.ChannelId, duration, activateMasterChannel, durationOverride)
			};
			PageStorage<ManualActionExecutionData>.Instance.Save();
			PageStorage<ManualActionExecutionData>.Instance.StorageData.ExecutionList = null;
		}
	}
}

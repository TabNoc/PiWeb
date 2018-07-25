using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using TabNoc.MyOoui.Interfaces.AbstractObjects;
using TabNoc.MyOoui.Interfaces.Enums;
using TabNoc.MyOoui.UiComponents;
using TabNoc.MyOoui.UiComponents.FormControl.InputGroups;
using TabNoc.MyOoui.UiComponents.FormControl.InputGroups.Components;
using TabNoc.PiWeb.Storage.WateringWeb.Manual;
using Button = TabNoc.MyOoui.HtmlElements.Button;

namespace TabNoc.PiWeb.Pages.WateringWeb.Manual
{
	internal class JobPage : StylableElement
	{
		private JobEntry _job;

		public JobPage(JobEntry job, ManualPage parent) : base("div")
		{
			this._job = job;
			SetBorder(BorderKind.Rounded, StylingColor.Secondary);

			#region Initialize Grid

			Grid grid = new Grid(this);
			grid.AddStyling(StylingOption.MarginRight, 2);
			grid.AddStyling(StylingOption.MarginLeft, 2);
			grid.AddStyling(StylingOption.MarginTop, 4);
			grid.AddStyling(StylingOption.MarginBottom, 2);

			#endregion Initialize Grid

			#region JobName

			MultiInputGroup jobNameMultiInputGroup = new MultiInputGroup();
			jobNameMultiInputGroup.AppendLabel("JobName");
			StylableTextInput jobNameTextInput = jobNameMultiInputGroup.AppendTextInput("Name?", startText: job.Name);
			jobNameMultiInputGroup.AppendValidation("", "Ein Job mit diesem Namen existiert bereits", false);
			Button saveJobNameButton = jobNameMultiInputGroup.AppendCustomElement(new Button(StylingColor.Success, asOutline: true, text: "Namen übernehmen", fontAwesomeIcon: "save"), false);
			Button deleteJobButton = jobNameMultiInputGroup.AppendCustomElement(new Button(StylingColor.Danger, asOutline: true, text: "Job Löschen", fontAwesomeIcon: "trash"), false);
			deleteJobButton.Click += (sender, args) =>
			{
				const string confirmMessage = "Wirklich Löschen";
				if (deleteJobButton.Text != confirmMessage)
				{
					deleteJobButton.Text = confirmMessage;
					return;
				}
				else
				{
					PageStorage<ManualData>.Instance.StorageData.JobEntries.Remove(job);
					parent.UpdateJobs();
				}
			};

			saveJobNameButton.Click += (sender, args) =>
			{
				if (PageStorage<ManualData>.Instance.StorageData.JobEntries.Any(entry => entry.Name == jobNameTextInput.Value))
				{
					if (job.Name == jobNameTextInput.Value)
					{
						return;
					}
					else
					{
						jobNameTextInput.SetValidation(false, true);
					}
				}
				else
				{
					jobNameTextInput.SetValidation(true, false);
					job.Name = jobNameTextInput.Value;
					parent.UpdateJobs();
				}
			};
			grid.AddRow().AppendCollum(jobNameMultiInputGroup, autoSize: true);

			#endregion JobName

			#region ExecuteJob

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

			Heading firstHeading = new Heading(5, "Job Ausführen ...");
			firstContainer.AppendChild(firstHeading);

			#endregion create Heading

			#region Override

			OverrideInputGroup overrideInputGroup = new OverrideInputGroup(100);
			firstContainer.AppendChild(overrideInputGroup);

			#endregion Override

			#region StartButton

			Button startButton = new Button(StylingColor.Success, true, text: "Starten!", fontAwesomeIcon: "play", asBlock: true);
			firstContainer.AppendChild(startButton);
			startButton.Click += (o, args) =>
			{
				startButton.IsDisabled = true;
				try
				{
					CreateJobAction(job, overrideInputGroup.Value);

					startButton.Text = "Gestartet";
				}
				catch (Exception)
				{
					startButton.Text = "Start fehlgeschlagen";
					throw;
				}
			};
			firstContainer.AppendChild(startButton);
			startButton.AddStyling(StylingOption.MarginBottom, 2);

			#endregion StartButton

			#endregion ExecuteJob

			grid.AddRow().AppendCollum(new Heading(4, "Batch Einträge"), autoSize: true);
			List batchEntries = grid.AddRow().AppendCollum(new List(false), autoSize: true);
			foreach (BatchEntry jobBatchEntry in job.BatchEntries)
			{
				batchEntries.AppendChild(new ListItem() { Text = jobBatchEntry.Name });
			}
		}

		private static void CreateJobAction(JobEntry job, int durationOverride)
		{
			PageStorage<ManualActionExecutionData>.Instance.StorageData.ExecutionList = new List<ManualActionExecutionData.ManualActionExecution>();
			foreach (BatchEntry jobBatchEntry in job.BatchEntries)
			{
				PageStorage<ManualActionExecutionData>.Instance.StorageData.ExecutionList.Add(
					new ManualActionExecutionData.ManualActionExecution(jobBatchEntry.ChannelId, jobBatchEntry.Duration,
						jobBatchEntry.ActivateMasterChannel, durationOverride));
			}
			PageStorage<ManualActionExecutionData>.Instance.Save();
			PageStorage<ManualActionExecutionData>.Instance.StorageData.ExecutionList = null;
		}
	}
}

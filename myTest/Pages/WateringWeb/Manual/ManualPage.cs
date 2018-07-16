using Ooui;
using System.Collections.Generic;
using System.Linq;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages.WateringWeb.Manual;
using TabNoc.Ooui.Storage.WateringWeb.Channels;
using TabNoc.Ooui.Storage.WateringWeb.Manual;
using TabNoc.Ooui.UiComponents;

namespace TabNoc.Ooui.Pages.WateringWeb.Overview
{
	internal class ManualPage : StylableElement
	{
		private readonly List<(Anchor, BatchPage)> _batchPills = new List<(Anchor, BatchPage)>();
		private readonly List<(Anchor, ManualChannelPage)> _channelPills = new List<(Anchor, ManualChannelPage)>();
		private readonly List<(Anchor, JobPage)> _jobPills = new List<(Anchor, JobPage)>();
		private VerticalPillNavigation _batchTabPage;
		private VerticalPillNavigation _channelTabPage;
		private VerticalPillNavigation _jobTabPage;

		public ManualPage() : base("div")
		{
			Container wrappingContainer = new Container();
			Grid grid = new Grid(wrappingContainer);

			TabNavigation tabNavigation = grid.AddRow().AppendCollum(new TabNavigation(true, false));

			tabNavigation.AddTab("Kanäle", CreateChannelTabPage(), false);
			tabNavigation.AddTab("Batch", CreateBatchTabPage(), false);
			tabNavigation.AddTab("Jobs", CreateJobTabPage(), true);

			AppendChild(wrappingContainer);
		}

		public void UpdateBatch()
		{
			_batchTabPage.Clear();
			_batchPills.Clear();
			FillBatchTabPage();
		}

		public void UpdateJobs()
		{
			_jobTabPage.Clear();
			_jobPills.Clear();
			FillJobTabPage();
			foreach ((Anchor pill, BatchPage batchPage) in _batchPills)
			{
				batchPage.FillJobDropDown();
			}
		}

		protected override void Dispose(bool disposing)
		{
			PageStorage<ManualData>.Instance.Save();
			base.Dispose(disposing);
		}

		private Element CreateBatchTabPage()
		{
			_batchTabPage = new VerticalPillNavigation(3, 9, true);
			_batchTabPage.AddStyling(StylingOption.MarginTop, 2);
			_batchTabPage.AddStyling(StylingOption.MarginBottom, 2);
			_batchTabPage.AddStyling(StylingOption.MarginLeft, 2);
			_batchTabPage.AddStyling(StylingOption.MarginRight, 0);

			FillBatchTabPage();

			return _batchTabPage;
		}

		private Element CreateChannelTabPage()
		{
			_channelTabPage = new VerticalPillNavigation("col-md-3 col-sm-1", "col-md-9 col-sm-11", true);
			_channelTabPage.AddStyling(StylingOption.MarginTop, 2);
			_channelTabPage.AddStyling(StylingOption.MarginBottom, 2);
			_channelTabPage.AddStyling(StylingOption.MarginLeft, 2);
			_channelTabPage.AddStyling(StylingOption.MarginRight, 0);

			FillChannelTabPage();

			return _channelTabPage;
		}

		private Element CreateJobTabPage()
		{
			_jobTabPage = new VerticalPillNavigation(3, 9, true);
			_jobTabPage.AddStyling(StylingOption.MarginTop, 2);
			_jobTabPage.AddStyling(StylingOption.MarginBottom, 2);
			_jobTabPage.AddStyling(StylingOption.MarginLeft, 2);
			_jobTabPage.AddStyling(StylingOption.MarginRight, 0);

			FillJobTabPage();

			return _jobTabPage;
		}

		private void FillBatchTabPage()
		{
			foreach (BatchEntry batch in PageStorage<ManualData>.Instance.StorageData.BatchEntries)
			{
				BatchPage batchPage = new BatchPage(batch, this);
				Anchor pill = _batchTabPage.AddPill(batch.Name, batchPage, batch == PageStorage<ManualData>.Instance.StorageData.BatchEntries.First());
				_batchPills.Add((pill, batchPage));
			}
		}

		private void FillChannelTabPage()
		{
			foreach (ChannelData channel in PageStorage<ChannelsData>.Instance.StorageData.Channels)
			{
				ManualChannelPage manualChannelPage = new ManualChannelPage(channel, this);
				Anchor pill = _channelTabPage.AddPill(channel.Name, manualChannelPage, channel == PageStorage<ChannelsData>.Instance.StorageData.Channels.First());
				_channelPills.Add((pill, manualChannelPage));
			}
		}

		private void FillJobTabPage()
		{
			foreach (JobEntry job in PageStorage<ManualData>.Instance.StorageData.JobEntries)
			{
				JobPage jobPage = new JobPage(job, this);
				Anchor pill = _jobTabPage.AddPill(job.Name, jobPage, job == PageStorage<ManualData>.Instance.StorageData.JobEntries.First());
				_jobPills.Add((pill, jobPage));
			}
		}
	}
}

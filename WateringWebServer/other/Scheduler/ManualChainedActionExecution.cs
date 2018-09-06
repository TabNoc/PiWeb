﻿using System;
using System.ComponentModel;
using TabNoc.PiWeb.DataTypes.WateringWeb.Manual;
using TabNoc.PiWeb.WateringWebServer.other.Hardware;

namespace TabNoc.PiWeb.WateringWebServer.other.Scheduler
{
	public class ManualChainedActionExecution : ChainScheduleManager<ManualChainedActionExecution>.ChainedActionExecution
	{
		// ReSharper disable once FieldCanBeMadeReadOnly.Global
		public ManualActionExecutionData.ManualActionExecution ManualActionExecution;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public ManualChainedActionExecution()
		{
		}

		public ManualChainedActionExecution(ManualActionExecutionData.ManualActionExecution manualActionExecution)
			: base(manualActionExecution.Guid, manualActionExecution.Duration, manualActionExecution.DurationOverride)
		{
			ManualActionExecution = manualActionExecution;
		}

		public override void ActivateAction(TimeSpan duration) => RelaisControl.Activate(ManualActionExecution.ChannelId, ManualActionExecution.ActivateMasterChannel, "Manual", duration);

		public override void DeactivateAction() => RelaisControl.Deactivate(ManualActionExecution.ChannelId, "Manual");
	}
}
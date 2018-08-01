using System;
using System.ComponentModel;
using TabNoc.PiWeb.DataTypes.WateringWeb.Manual;

namespace TabNoc.PiWeb.WateringWebServer.other.Scheduler
{
	public class ManualChainedActionExecution : ChainScheduleManager<ManualChainedActionExecution>.ChainedActionExecution
	{
		public readonly ManualActionExecutionData.ManualActionExecution ManualActionExecution;

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

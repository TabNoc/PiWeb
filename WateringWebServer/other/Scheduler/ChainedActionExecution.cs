using System;
using System.ComponentModel;

namespace TabNoc.PiWeb.WateringWebServer.other.Scheduler
{
	public partial class ChainScheduleManager<T> where T : ChainScheduleManager<T>.ChainedActionExecution
	{
		public abstract class ChainedActionExecution
		{
			public readonly TimeSpan Duration;
			public readonly int DurationOverride;
			public readonly string Guid;

			protected ChainedActionExecution(string guid, TimeSpan duration, int durationOverride)
			{
				Guid = guid;
				Duration = duration;
				DurationOverride = durationOverride;
			}

			[EditorBrowsable(EditorBrowsableState.Never)]
			protected ChainedActionExecution()
			{
			}

			public abstract void ActivateAction(TimeSpan duration);

			public abstract void DeactivateAction();
		}
	}
}

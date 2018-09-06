using System;
using System.ComponentModel;

namespace TabNoc.PiWeb.WateringWebServer.other.Scheduler
{
	public partial class ChainScheduleManager<T> where T : ChainScheduleManager<T>.ChainedActionExecution
	{
		public abstract class ChainedActionExecution
		{
			// ReSharper disable FieldCanBeMadeReadOnly.Global
			public TimeSpan Duration;

			public int DurationOverride;
			public string Guid;
			// ReSharper restore FieldCanBeMadeReadOnly.Global

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

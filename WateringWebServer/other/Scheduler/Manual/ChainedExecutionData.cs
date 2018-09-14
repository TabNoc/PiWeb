using System;

namespace TabNoc.PiWeb.WateringWebServer.other.Scheduler.Manual
{
	public partial class ChainScheduleManager<T> where T : ChainScheduleManager<T>.ChainedActionExecution
	{
		public class ChainedExecutionData
		{
			public T ChainedActionExecutionData;
			public string ElementEventSource;
			public string Guid;
			public string NextGuid = "";
			public string PreviousGuid;

			/// <summary>
			/// Represents the Job to deactivate an enqued Job. Only set, when the Job ist already enqueed
			/// </summary>
			public string DeactivationJob;

			/// <summary>
			/// Time when the Job has started
			/// </summary>
			public TimeSpan StartTime;

			/// <summary>
			/// Calculated Duration with all Overrides included, if the Job has already Started
			/// </summary>
			public TimeSpan Duration;

			public ChainedExecutionData(T chainedActionExecutionData, string elementEventSource, string previousGuid)
			{
				Guid = chainedActionExecutionData.Guid;
				ChainedActionExecutionData = chainedActionExecutionData;
				ElementEventSource = elementEventSource;
				PreviousGuid = previousGuid;
			}
		}
	}
}

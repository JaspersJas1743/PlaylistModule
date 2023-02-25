using System;

namespace PlaylistModule.Utilities
{
	public sealed class TimeTrackingEventArgs: EventArgs
	{
		public TimeTrackingEventArgs(TimeSpan offset, TimeSpan duration)
		{
			Offset = offset;
			Duration = duration;
		}

		public TimeSpan Offset { get; init; }
		public TimeSpan Duration { get; init;  }
	}
}
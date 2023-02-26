using System;

namespace PlaylistModule.Utilities
{
	public class ForwardSwitchingEventArgs: EventArgs
	{
		public ForwardSwitchingEventArgs(bool isLast)
		{
			IsLast = isLast;
		}

		public bool IsLast { get; init; }
	}
}
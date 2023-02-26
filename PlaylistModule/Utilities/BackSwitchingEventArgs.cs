using System;

namespace PlaylistModule.Utilities
{
	public class BackSwitchingEventArgs: EventArgs
	{
		public BackSwitchingEventArgs(bool isFirst)
		{
			IsFirst = isFirst;
		}

		public bool IsFirst { get; init; }
	}
}
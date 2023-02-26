using System;

namespace PlaylistModule.Utilities
{
	public sealed class SoundStartingEventArgs: EventArgs
	{
		public SoundStartingEventArgs(Audio audio)
		{
			PlaylistAudio = audio;
		}

		public Audio PlaylistAudio { get; init; }
	}
}
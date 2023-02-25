using System;

namespace PlaylistModule.Utilities
{
	public sealed class SoundStartingEventArgs: EventArgs
	{
		public SoundStartingEventArgs(Audio audio, int audioIndex)
		{
			PlaylistAudio = audio;
			AudioIndex = audioIndex;
		}

		public Audio PlaylistAudio { get; init; }
		public int AudioIndex { get; init; }
	}
}
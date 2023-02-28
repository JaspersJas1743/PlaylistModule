using System;

namespace PlaylistModule.Utilities
{
    public class SoundPausingEventArgs: EventArgs
    {
        public SoundPausingEventArgs(Audio playlistAudio)
        {
            PlaylistAudio = playlistAudio;
        }

        public Audio PlaylistAudio { get; init; }
    }
}
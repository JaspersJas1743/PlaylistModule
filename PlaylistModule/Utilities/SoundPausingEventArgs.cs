namespace PlaylistModule.Utilities
{
    public class SoundPausingEventArgs
    {
        public SoundPausingEventArgs(Audio playlistAudio)
        {
            PlaylistAudio = playlistAudio;
        }

        public Audio PlaylistAudio { get; init; }
    }
}
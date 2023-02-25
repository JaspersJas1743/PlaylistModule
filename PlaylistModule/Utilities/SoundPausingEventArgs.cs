namespace PlaylistModule.Utilities
{
    public class SoundPausingEventArgs
    {
        public SoundPausingEventArgs(Audio playlistAudio, int audioIndex)
        {
            PlaylistAudio = playlistAudio;
            AudioIndex = audioIndex;
        }

        public Audio PlaylistAudio { get; init; }
        public int AudioIndex { get; init; }
    }
}
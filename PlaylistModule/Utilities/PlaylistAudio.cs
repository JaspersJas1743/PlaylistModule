namespace PlaylistModule.Utilities
{
	public class PlaylistAudio
	{
		public PlaylistAudio(Audio audio)
			=> Value = audio;

		public Audio Value { get; set; }
		public PlaylistAudio Prev { get; set; }
		public PlaylistAudio Next { get; set; }
	}
}

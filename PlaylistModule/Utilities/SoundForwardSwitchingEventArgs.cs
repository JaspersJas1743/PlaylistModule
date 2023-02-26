namespace PlaylistModule.Utilities
{
	public class SoundForwardSwitchingEventArgs
	{
		public SoundForwardSwitchingEventArgs(Audio substituteAudio, Audio changeableAudio)
		{
			SubstituteAudio = substituteAudio;
			ChangeableAudio = changeableAudio;
		}

		public Audio SubstituteAudio { get; init; }
		public Audio ChangeableAudio { get; init; }
	}
}
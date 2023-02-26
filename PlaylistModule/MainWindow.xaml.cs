using PlaylistModule.Utilities;
using System;
using System.Windows;

namespace PlaylistModule
{
	public partial class MainWindow: Window
	{
		private Playlist _playlist = new Playlist();
		private IProgress<double> _progress;

		public MainWindow()
		{
			InitializeComponent();

			_progress = new Progress<double>(handler: progress => ProgressIndicator.Value = progress);

			_playlist.AddSound(GetAudio(title: "N/A - Две тысячи баксов за сигарету", duration: TimeSpan.FromSeconds(10)));
			_playlist.AddSound(GetAudio(title: "White Punk - Дюраг", duration: TimeSpan.FromSeconds(10)));
			_playlist.AddSound(GetAudio(title: "Платина - Биг бой абубаби", duration: TimeSpan.FromSeconds(10)));
			_playlist.AddSound(GetAudio(title: "Кишлак - Маленький бандит", duration: TimeSpan.FromSeconds(10)));
			_playlist.AddSound(GetAudio(title: "Кишлак - Дрочу на твои фото", duration: TimeSpan.FromSeconds(10)));

			_playlist.SoundStarting += OnPlaylistPlayStarting;
			_playlist.SoundPausing += OnPlaylistSoundPausing;
			_playlist.ForwardSwitching += OnPlaylistForwardSwitching;
			_playlist.BackSwitching += OnPlaylistBackSwitching;
			_playlist.PlaylistStarted += OnPlaylistPlaylistStarted;
			_playlist.PlaylistEnded += OnPlaylistPlaylistEnded;
		}

		private void OnPlaylistPlaylistEnded(object sender, PlaylistEndedEventArgs e)
		{
			PrevSoundButton.Visibility = Visibility.Collapsed;
			NextSoundButton.Visibility = Visibility.Collapsed;
			PlayButton.Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Collapsed;
			SoundTitle.Text = String.Empty;
			Offset.Text = String.Empty;
			Duration.Text = String.Empty;
			RestartButton.Visibility = Visibility.Visible;
			_progress.Report(value: 0);
		}

		private void OnPlaylistPlaylistStarted(object sender, PlaylistStartedEventArgs e)
		{
			NextSoundButton.Visibility = Visibility.Visible;
			RestartButton.Visibility = Visibility.Collapsed;
		}

		private void OnPlaylistBackSwitching(object sender, BackSwitchingEventArgs e)
		{
			if (e.IsFirst)
				PrevSoundButton.Visibility = Visibility.Collapsed;
			NextSoundButton.Visibility = Visibility.Visible;
		}

		private void OnPlaylistForwardSwitching(object sender, ForwardSwitchingEventArgs e)
		{
			if (e.IsLast)
				NextSoundButton.Visibility = Visibility.Collapsed;
			PrevSoundButton.Visibility = Visibility.Visible;
		}

		private Audio GetAudio(string title, TimeSpan duration)
		{
			Audio audio = new Audio(title: title, duration: duration);
			audio.TimeTracking += OnAudioTimeTracking;
			return audio;
		}

		private void OnPlaylistSoundPausing(object sender, SoundPausingEventArgs e)
			=> (PlayButton.Visibility, PauseButton.Visibility) = (Visibility.Visible, Visibility.Collapsed);

		private void OnPlaylistPlayStarting(object sender, SoundStartingEventArgs e)
		{
			(PlayButton.Visibility, PauseButton.Visibility) = (Visibility.Collapsed, Visibility.Visible);

			SoundTitle.Text = e.PlaylistAudio.Title;
			Duration.Text = e.PlaylistAudio.Duration.ToString(format: "mm\\:ss");
			ProgressIndicator.Maximum = e.PlaylistAudio.Duration.TotalSeconds;
		}

		private void OnAudioTimeTracking(object sender, TimeTrackingEventArgs e)
		{
			Offset.Text = e.Offset.ToString(format: "mm\\:ss");
			_progress.Report(value: e.Offset.TotalSeconds);
		}

		private async void OnPlayButtonClick(object sender, RoutedEventArgs e)
			=> await _playlist.Play();

		private void OnPauseButtonClick(object sender, RoutedEventArgs e)
			=> _playlist.Pause();

		private async void OnNextSoundButtonClick(object sender, RoutedEventArgs e)
			=> await _playlist.MoveToNextSound();

		private async void OnPrevSoundButtonClick(object sender, RoutedEventArgs e)
			=> await _playlist.MoveToPrevSound();

		private async void OnRestartButtonClick(object sender, RoutedEventArgs e)
			=> await _playlist.StartAgain();
	}
}

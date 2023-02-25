using PlaylistModule.Utilities;
using System;
using System.Windows;

namespace PlaylistModule
{
	public partial class MainWindow : Window
	{
		private Playlist _playlist = new Playlist();
		private IProgress<double> _p;

		public MainWindow()
		{
			InitializeComponent();

			ProgressIndicator.Minimum = 0;
			_p = new Progress<double>(progress => ProgressIndicator.Value = progress);

			_playlist.AddSound(GetAudio(title: "Две тысячи баксов за сигарету", duration: TimeSpan.FromSeconds(100)));
			_playlist.AddSound(GetAudio(title: "Дюраг", duration: TimeSpan.FromSeconds(120)));
			_playlist.AddSound(GetAudio(title: "Биг бой абубаби", duration: TimeSpan.FromSeconds(10)));

			_playlist.SoundStarting += OnPlaylistPlayStarting;
		}

		private Audio GetAudio(string title, TimeSpan duration)
		{
			Audio audio = new Audio(title: title, duration: duration);
			audio.TimeTracking += OnAudioTimeTracking;
			return audio;
		}

		private void OnPlaylistPlayStarting(object sender, SoundStartingEventArgs e)
		{
			SongTitle.Text = e.PlaylistAudio.Title;
			Duration.Text = e.PlaylistAudio.Duration.ToString("mm\\:ss");
			ProgressIndicator.Maximum = e.PlaylistAudio.Duration.TotalSeconds;
		}

		private void OnAudioTimeTracking(object sender, TimeTrackingEventArgs e)
		{
			Offset.Text = e.Offset.ToString("mm\\:ss");
			_p.Report(e.Offset.TotalSeconds);
		}

		private async void OnPlayButtonClick(object sender, RoutedEventArgs e)
		{
			SwapButton();
			await _playlist.Play();
		}

		private async void OnPauseButtonClick(object sender, RoutedEventArgs e)
		{
			SwapButton();
			await _playlist.Pause();
		}

		private void SwapButton()
			=> (PauseButton.Visibility, PlayButton.Visibility) = (PlayButton.Visibility, PauseButton.Visibility);
	}
}

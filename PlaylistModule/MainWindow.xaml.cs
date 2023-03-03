using PlaylistModule.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PlaylistModule
{
	public partial class MainWindow: Window
	{
		private Playlist _playlist = new Playlist();

		public MainWindow()
		{
			InitializeComponent();

			_playlist.AddSound(GetAudio(title: "Две тысячи баксов за сигарету", author: "Неизвестен", duration: TimeSpan.FromSeconds(10)));
			_playlist.AddSound(GetAudio(title: "Дюраг", author: "White Punk", duration: TimeSpan.FromSeconds(10)));
			_playlist.AddSound(GetAudio(title: "Биг бой абубаби", author: "Платина", duration: TimeSpan.FromSeconds(10)));
			_playlist.AddSound(GetAudio(title: "Маленький бандит", author: "Кишлак", duration: TimeSpan.FromSeconds(10)));
			_playlist.AddSound(GetAudio(title: "Утонуть", author: "тринадцать карат", duration: TimeSpan.FromSeconds(10)));

			_playlist.SoundStarting += OnPlaylistPlayStarting;
			_playlist.SoundPausing += OnPlaylistSoundPausing;
			_playlist.ForwardSwitching += OnPlaylistForwardSwitching;
			_playlist.BackSwitching += OnPlaylistBackSwitching;
			_playlist.PlaylistStarted += OnPlaylistPlaylistStarted;
			_playlist.PlaylistEnded += OnPlaylistPlaylistEnded;
		}

		private Audio GetAudio(string title, string author, TimeSpan duration)
		{
			Uri uri = new Uri($"{Environment.CurrentDirectory}/../../../Resources/SoundCovers/{author} - {title}.jpg");
			Audio audio = new Audio(title: title, author: author, duration: duration, cover: new BitmapImage(uriSource: uri));
			audio.TimeTracking += OnAudioTimeTracking;
			return audio;
		}

		private void OnPlaylistPlaylistEnded(object sender, PlaylistEndedEventArgs e)
		{
			PrevSoundButton.Visibility = NextSoundButton.Visibility = PlayButton.Visibility = PauseButton.Visibility = SoundProgress.Visibility = SoundImage.Visibility = Visibility.Collapsed;
			SoundTitle.Text = SoundAuthor.Text = Offset.Text = Duration.Text = String.Empty;
			RestartButton.Visibility = Visibility.Visible;
		}

		private void OnPlaylistPlaylistStarted(object sender, PlaylistStartedEventArgs e)
		{
			(NextSoundButton.Visibility, RestartButton.Visibility) = (Visibility.Visible, Visibility.Collapsed);
			SoundProgress.Visibility = Visibility.Visible;
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

		private void OnPlaylistSoundPausing(object sender, SoundPausingEventArgs e)
			=> (PlayButton.Visibility, PauseButton.Visibility) = (Visibility.Visible, Visibility.Collapsed);

		private void OnPlaylistPlayStarting(object sender, SoundStartingEventArgs e)
		{
			(PlayButton.Visibility, PauseButton.Visibility) = (Visibility.Collapsed, Visibility.Visible);

			SoundTitle.Text = e.PlaylistAudio.Title;
			SoundAuthor.Text = e.PlaylistAudio.Author;
			Duration.Text = e.PlaylistAudio.Duration.ToString(format: "mm\\:ss");
			SoundImage.Source = e.PlaylistAudio.Cover;
			SoundProgress.MaxWidth = e.PlaylistAudio.Duration.TotalMilliseconds;
		}

		private void OnAudioTimeTracking(object sender, TimeTrackingEventArgs e)
		{
			Offset.Text = e.Offset.ToString(format: "mm\\:ss");
			SoundProgress.Value = e.Offset.TotalSeconds;
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

		private async void OnSoundProgressDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			Slider slider = (Slider)sender;
			await _playlist.SetSoundOffset(Math.Round(slider.Value));
		}

		private void OnSoundProgressDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
			=> _playlist.Pause();

		private void OnExitButtonClick(object sender, RoutedEventArgs e)
			=> Application.Current.Shutdown();

		private void OnDeactivateButtonClick(object sender, RoutedEventArgs e)
			=> WindowState = WindowState.Minimized;
	}
}

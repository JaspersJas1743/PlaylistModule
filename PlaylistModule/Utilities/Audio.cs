using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PlaylistModule.Utilities
{
	public sealed class Audio
	{
		private CancellationTokenSource _tokenSource;
		private CancellationToken _token;

		public Audio(string title, string author, int durationInSeconds, BitmapImage cover)
			: this(title: title, author: author, duration: TimeSpan.FromSeconds(value: durationInSeconds), cover: cover)
		{ }

		public Audio(string title, string author, TimeSpan duration, BitmapImage cover)
		{
			Author = String.IsNullOrEmpty(author) ? "Неизвестен" : author;
			Title = title;
			Duration = duration;
			Cover = cover;
			_tokenSource = new CancellationTokenSource();
			_token = _tokenSource.Token;
		}

		public delegate void TimeTrackingEventHandler(object sender, TimeTrackingEventArgs e);
		public event TimeTrackingEventHandler TimeTracking;

		public string Author { get; init; }
		public string Title { get; init; }
		public TimeSpan Duration { get; init; }
		public BitmapImage Cover { get; init; }
		public TimeSpan Offset { get; set; }

		public async Task PlaySound()
		{
			while (Offset <= Duration)
			{
				try
				{
					_token.ThrowIfCancellationRequested();
					TimeTracking?.Invoke(sender: this, e: new TimeTrackingEventArgs(offset: Offset, duration: Duration));
					await Task.Delay(millisecondsDelay: 1_000, cancellationToken: _token);
					Offset = Offset.Add(ts: TimeSpan.FromSeconds(value: 1));
				}
				catch
				{
					RefreshToken();
					throw;
				}
			}
			RefreshOffset();
		}

		private void RefreshToken()
		{
			_tokenSource.Dispose();
			_tokenSource = new CancellationTokenSource();
			_token = _tokenSource.Token;
		}

		public void RefreshOffset()
			=> Offset = TimeSpan.Zero;

		public void PauseSound()
			=> _tokenSource.Cancel();
	}
}

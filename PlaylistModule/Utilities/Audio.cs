using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PlaylistModule.Utilities
{
	public sealed class Audio
	{
		private TimeSpan _audioOffset;
		private CancellationTokenSource _tokenSource;
		private CancellationToken _token;

		public Audio(string title, int durationInSeconds)
			: this(title: title, duration: TimeSpan.FromSeconds(value: durationInSeconds))
		{ }

		public Audio(string title, TimeSpan duration)
		{
			Title = title;
			Duration = duration;
			_tokenSource = new CancellationTokenSource();
			_token = _tokenSource.Token;
		}

		public delegate void TimeTrackingEventHandler(object sender, TimeTrackingEventArgs e);
		public event TimeTrackingEventHandler TimeTracking;

		public string Title { get; init; }
		public TimeSpan Duration { get; init; }

		public async Task PlaySound()
		{
			while (_audioOffset <= Duration)
			{
				try
				{
					_token.ThrowIfCancellationRequested();
					TimeTracking?.Invoke(sender: this, e: new TimeTrackingEventArgs(offset: _audioOffset, duration: Duration));
					await Task.Delay(millisecondsDelay: 1_000, cancellationToken: _token);
					_audioOffset = _audioOffset.Add(ts: TimeSpan.FromSeconds(value: 1));
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
			=> _audioOffset = TimeSpan.Zero;

		public void PauseSound()
			=> _tokenSource.Cancel();
	}
}

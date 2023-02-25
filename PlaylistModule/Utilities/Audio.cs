using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PlaylistModule.Utilities
{
	public sealed class Audio
	{
		private CancellationTokenSource _tokenSource;
		private CancellationToken _token;
		private TimeSpan _audioOffset;

		public Audio(string title, int durationInSeconds)
			: this(title: title, duration: TimeSpan.FromSeconds(durationInSeconds))
		{ }

		public Audio(string title, TimeSpan duration)
		{
			Title = title;
			Duration = duration;
			RefreshToken();
		}

		public delegate void TimeTrackingEventHandler(object sender, TimeTrackingEventArgs e);
		public event TimeTrackingEventHandler TimeTracking;

		public string Title { get; init; }
		public TimeSpan Duration { get; init; }

		public async Task PlaySound()
		{
			int audioDurationInSeconds = (int)Duration.TotalSeconds;
			int lostDurationInSeconds = (int)_audioOffset.TotalSeconds;
			for (int currentSecond = lostDurationInSeconds; currentSecond < audioDurationInSeconds; ++currentSecond)
			{
				if (_token.IsCancellationRequested)
				{
					_tokenSource.Dispose();
					RefreshToken();
					return;
				}

				TimeTracking?.Invoke(sender: this, e: new TimeTrackingEventArgs(offset: _audioOffset, duration: Duration));
				await Task.Delay(millisecondsDelay: 1_000);
				_audioOffset = _audioOffset.Add(ts: TimeSpan.FromSeconds(value: 1));
			}
			_audioOffset = TimeSpan.Zero;
		}

		public async Task PauseSound()
			=> _tokenSource.Cancel();

		public override string ToString()
			=> $"Audio {{Title={Title};Offset={_audioOffset};Duration={Duration}}}";

		private void RefreshToken()
		{
			_tokenSource = new CancellationTokenSource();
			_token = _tokenSource.Token;
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PlaylistModule.Utilities
{
	public class Playlist: IEnumerable<Audio>
	{
		private CancellationTokenSource _tokenSource;
		private CancellationToken _token;
		private PlaylistAudio _first;
		private PlaylistAudio _last;
		private int _indexOfCurrentSound = 0;

		public Playlist()
		{
			_tokenSource = new CancellationTokenSource();
			_token = _tokenSource.Token;
		}

		public delegate void PlaybackStartingEventHandler(object sender, SoundStartingEventArgs e);
		public delegate void PlaybackPausingEventHandler(object sender, SoundPausingEventArgs e);
		public delegate void ForwardSwitchingEventHandler(object sender, ForwardSwitchingEventArgs e);
		public delegate void BackSwitchingEventHandler(object sender, BackSwitchingEventArgs e);
		public delegate void PlaylistStartedEventHandler(object sender, PlaylistStartedEventArgs e);
		public delegate void PlaylistEndedEventHandler(object sender, PlaylistEndedEventArgs e);

		public event PlaybackStartingEventHandler SoundStarting;
		public event PlaybackPausingEventHandler SoundPausing;
		public event ForwardSwitchingEventHandler ForwardSwitching;
		public event BackSwitchingEventHandler BackSwitching;
		public event PlaylistStartedEventHandler PlaylistStarted;
		public event PlaylistEndedEventHandler PlaylistEnded;

		public static bool IsPlayed { get; private set; } = false;

		public int Length { get; private set; } = 0;

		private Audio this[int index]
		{
			get
			{
				if (!Enumerable.Range(start: 0, count: Length).Contains(value: index))
					throw new ArgumentOutOfRangeException();

				return this.ElementAt(index);
			}
		}

		public void AddSound(Audio audio)
		{
			PlaylistAudio node = new PlaylistAudio(audio: audio);

			if (_first is null)
				_first = node;
			else
			{
				_last.Next = node;
				node.Prev = _last;
			}

			_last = node;
			++Length;
		}

		public async Task Play()
		{
			if (IsPlayed)
				return;

			if (_indexOfCurrentSound == 0)
				PlaylistStarted?.Invoke(sender: this, e: new PlaylistStartedEventArgs());

			IsPlayed = true;
			for (int index = _indexOfCurrentSound; index < Length; ++index)
			{
				try
				{
					Audio audio = this[index: index];
					SoundStarting?.Invoke(sender: this, e: new SoundStartingEventArgs(audio: audio));
					await audio.PlaySound();
					_indexOfCurrentSound = index;
					_token.ThrowIfCancellationRequested();
					ForwardSwitching?.Invoke(sender: this, e: new ForwardSwitchingEventArgs(isLast: index + 1 == Length - 1));
				}
				catch
				{
					RefreshToken();
					return;
				}
			}
			IsPlayed = false;
			PlaylistEnded?.Invoke(sender: this, e: new PlaylistEndedEventArgs());
		}

		private void RefreshToken()
		{
			_tokenSource.Dispose();
			_tokenSource = new CancellationTokenSource();
			_token = _tokenSource.Token;
		}

		public void Pause()
		{
			if (!IsPlayed)
				return;

			IsPlayed = false;
			Audio audio = this[_indexOfCurrentSound];
			_tokenSource.Cancel();
			audio.PauseSound();
			SoundPausing?.Invoke(sender: this, e: new SoundPausingEventArgs(playlistAudio: audio));
		}

		public async Task MoveToNextSound()
		{
			ForwardSwitching?.Invoke(sender: this, e: new ForwardSwitchingEventArgs(isLast: _indexOfCurrentSound + 1 == Length - 1));
			await PlaylistMovement(action: () => ++_indexOfCurrentSound);
		}

		public async Task MoveToPrevSound()
		{
			BackSwitching?.Invoke(sender: this, e: new BackSwitchingEventArgs(isFirst: _indexOfCurrentSound - 1 == 0));
			await PlaylistMovement(action: () => --_indexOfCurrentSound);
		}

		private async Task PlaylistMovement(Action action)
		{
			Pause();
			Audio audio = this[_indexOfCurrentSound];
			audio.RefreshOffset();
			action();
			await Play();
		}

		public async Task StartAgain()
		{
			_indexOfCurrentSound = 0;
			await Play();
		}

		public async Task SetSoundOffset(double offsetInSeconds)
			=> await SetSoundOffset(newOffset: TimeSpan.FromSeconds(value: offsetInSeconds));

		public async Task SetSoundOffset(TimeSpan newOffset)
		{
			Pause();
			this[index: _indexOfCurrentSound].Offset = newOffset;
			await Play();
		}

		public IEnumerator<Audio> GetEnumerator()
		{
			PlaylistAudio node = _first;
			while (node is not null)
			{
				yield return node.Value;
				node = node.Next;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();
	}
}

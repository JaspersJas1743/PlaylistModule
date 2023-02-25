using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PlaylistModule.Utilities
{
    public class Playlist : IEnumerable<Audio>
	{
		private CancellationTokenSource _tokenSource;
		private CancellationToken _token;
		private PlaylistAudio _first;
		private PlaylistAudio _last;
		private int _indexOfCurrentSound = 0;

		public Playlist()
			=> RefreshToken();

		public delegate void PlaybackStartingEventHandler(object sender, SoundStartingEventArgs e);
		public delegate void PlaybackPausingEventHandler(object sender, SoundPausingEventArgs e);

		public event PlaybackStartingEventHandler SoundStarting;
		public event PlaybackPausingEventHandler SoundPausing;

		public int Length { get; private set; }

		public Audio this[int index]
		{
			get
			{
				if (!Enumerable.Range(0, Length).Contains(index))
				{
					throw new ArgumentOutOfRangeException(
						paramName: nameof(index),
						message: $"Индекс ({index}) вне допустимых значений: [0; {Length})"
					);
				}

				return this.ElementAt(index);
			}
		}

		private void RefreshToken()
		{
			_tokenSource = new CancellationTokenSource();
			_token = _tokenSource.Token;
		}

		public async Task AddSound(Audio audio)
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
			for (int index = _indexOfCurrentSound; index < Length; ++index)
			{
				if (_token.IsCancellationRequested)
				{
					_tokenSource.Dispose();
					RefreshToken();
					return;
				}

				_indexOfCurrentSound = index;
				Audio audio = this[index];
				SoundStarting?.Invoke(sender: this, e: new SoundStartingEventArgs(audio: audio, audioIndex: index));
				await audio.PlaySound();
			}
		}

		public async Task Pause()
		{
			Audio audio = this[_indexOfCurrentSound];
			await audio.PauseSound();
			_tokenSource.Cancel();
			SoundPausing?.Invoke(sender: this, e: new SoundPausingEventArgs(playlistAudio: audio, audioIndex: _indexOfCurrentSound));
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

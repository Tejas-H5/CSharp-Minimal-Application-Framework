using OpenTK.Audio.OpenAL;
using System;

namespace MinimalAF.Audio {
    /// <summary>
    /// Use this one to load and play small sound effects. 
    /// Despite the name, the audio may be looped
    /// 
    /// Advantages:
    ///     - relatively easy to use, no audio updating subroutines/threads need to be implemented to use this
    ///     
    /// Disadvantages:
    ///     - Cannot play procedurally generated audio or anything that must be streamed from somewhere
    ///         - No support for custom signal processing effect chains as a consequence of this
    ///         - this use case requires AudioSourceStreamed
    /// </summary>
    public class AudioClipInput : IAudioSourceInput, IDisposable {
        public float PlayStartOffset = 0;

        private int _alBuffer;
        AudioRawData _audioData;

        public bool CanHaveMultipleConsumers => true;

        public AudioClipInput(AudioRawData audioData) {
            AudioCTX.ALCall(out _alBuffer, () => { return AL.GenBuffer(); });

            if (audioData == null) {
                throw new System.Exception("audioData can't be null here");
            }

            _audioData = audioData;
            AudioCTX.ALCall(() => AL.BufferData(
                _alBuffer,
                _audioData.Channels == 1 ? ALFormat.Mono16 : ALFormat.Stereo16,
                _audioData.Samples,
                _audioData.SampleRate
            ));
        }

        public void Play(OpenALSource alSource) {
            alSource.SetBuffer(_alBuffer);
            alSource.SetSecOffset(PlayStartOffset);

            alSource.Play();
        }

        public void Pause(OpenALSource alSource) {
            alSource.StopAndUnqueueAllBuffers();
            PlayStartOffset = alSource.GetSecOffset();
        }

        public void Stop(OpenALSource alSource) {
            alSource.StopAndUnqueueAllBuffers();
            PlayStartOffset = 0;
        }

        public double GetRealtimePlaybackPosition(OpenALSource alSource) {
            return alSource.GetSecOffset();
        }

        public void SetPlaybackPosition(OpenALSource alSource, double pos) {
            alSource.SetSecOffset((float)pos);
        }

        /// <summary>
        /// it's a no-op
        /// </summary>
        public void Update(OpenALSource alSource) {}

        #region IDisposable Support
        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;

            _audioData = null;

            // TODO: also detatch from source if possible
            AudioCTX.ALCall(() => AL.DeleteBuffer(_alBuffer));

            _disposed = true;
        }

        public PlaybackState GetPlaybackState(OpenALSource alSource) {
            var state = alSource.GetSourceState();
            if (state == ALSourceState.Playing) {
                return PlaybackState.Playing;
            }

            if (state == ALSourceState.Paused) {
                return PlaybackState.Paused;
            }

            return PlaybackState.Stopped;
        }

        ~AudioClipInput() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

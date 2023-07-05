using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;

namespace MinimalAF.Audio {
    // https://indiegamedev.net/2020/02/25/the-complete-guide-to-openal-with-c-part-2-streaming-audio/ as a reference

    /// <summary>
    /// Use this to play larger audio files, or for playing generated/streamed audio
    /// 
    /// TODO: rename to streamedAudioInput
    /// 
    /// Advantages:
    ///     - Can play streamed audio, which can be provided from any custom implementation of the IAudioStreamProvider interface
    ///     - doesn't create large OpenAL buffers
    ///     - can be used to implement audio playback with custom effects
    ///     
    /// Disadvantages:
    ///     - OpenAL Buffers need to be constantly spooled with the UpdateStream() function every 'frame', 
    ///     either on the main thread or in a seperate one. Using several of these in a single 
    ///     program might be compute intensive. It could also be absolutely fine, I have yet to do any benchmarking and such
    /// </summary>
    public class AudioStreamInput : IAudioSourceInput, IDisposable {
        const int NUM_BUFFERS = 4;
        const int BUFFER_SIZE = 65536; // 32kb of data in each buffer

        internal static short[] s_NonThreadSafeTempBuffer = new short[BUFFER_SIZE];
        int[] _buffers = new int[NUM_BUFFERS];

        IAudioStreamProvider _streamProvider;

        double _lastKnownCursorPosition;
        Queue<double> _bufferCursorPositions = new Queue<double>();

        public bool CanHaveMultipleConsumers => false;

        public AudioStreamInput(IAudioStreamProvider stream = null) {
            // initialize buffers
            for (int i = 0; i < _buffers.Length; i++) {
                AudioCTX.ALCall(() => {
                    _buffers[i] = AL.GenBuffer();
                });
            }

            // put zeroes in all the new buffers
            {
                for (int i = 0; i < s_NonThreadSafeTempBuffer.Length; i++) {
                    s_NonThreadSafeTempBuffer[i] = 0;
                }

                for (int i = 0; i < _buffers.Length; i++) {
                    SendTempDataToBuffer(_buffers[i]);
                }
            }

            SetStream(stream);
        }

        public void SetStream(IAudioStreamProvider stream) {
            _streamProvider = stream;
        }

        /// <summary>
        /// Can be quite inefficient
        /// </summary>
        public void RespoolAllBuffers(OpenALSource alSource) {
            // requeue all buffers
            for (int i = 0; i < _buffers.Length; i++) {
                if (!AdvanceStreamToTempBufferAndEnqueueIt(alSource, _buffers[i])) {
                    break;
                }
            }
        }

        public void Play(OpenALSource alSource) {
            if (_streamProvider == null) {
                return;
            }

            if (alSource.GetBuffersQueued() == 0) {
                _streamProvider.PlaybackPosition = _lastKnownCursorPosition;
                RespoolAllBuffers(alSource);
            }

            alSource.Play();
        }


        public void Pause(OpenALSource alSource) {
            if (_streamProvider == null) {
                return;
            }

            alSource.Pause();
        }

        public void Stop(OpenALSource alSource) {
            if (_streamProvider == null)
                return;

            _lastKnownCursorPosition = GetRealtimePlaybackPosition(alSource);

            alSource.StopAndUnqueueAllBuffers();
            _bufferCursorPositions.Clear();
        }

        public void Update(OpenALSource alSource) {
            // rotate openAL buffers
            for(
                int buffersProcessed = alSource.GetBuffersProcessed(); 
                buffersProcessed > 0; 
                buffersProcessed --
            ) {
                int nextBuffer;
                {
                    nextBuffer = alSource.UnqueueBuffer();
                    _lastKnownCursorPosition = _bufferCursorPositions.Dequeue();
                }

                if (!AdvanceStreamToTempBufferAndEnqueueIt(alSource, nextBuffer)) {
                    if (alSource.GetBuffersQueued() == 0) {
                        Stop(alSource);
                    }
                }
            }
        }

        // return if we were able to advance the stream at all
        private bool AdvanceStreamToTempBufferAndEnqueueIt(OpenALSource alSource, int bufferNum) {
            if (_streamProvider == null)
                return false;

            var res = _streamProvider.AdvanceStream(s_NonThreadSafeTempBuffer, BUFFER_SIZE);
            if (res.WriteCount == 0) {
                return false;
            }

            if (res.WriteCount < BUFFER_SIZE) {
                // zero out remaining data that wasn't written to
                for(int i = res.WriteCount; i < BUFFER_SIZE; i++) {
                    s_NonThreadSafeTempBuffer[i] = 0;
                }
            }

            SendTempDataToBuffer(bufferNum);
            {
                alSource.QueueBuffer(bufferNum);
                _bufferCursorPositions.Enqueue(res.CursorPosSeconds);
            }

            return true;
        }


        private void SendTempDataToBuffer(int alBuffer) {
            if (_streamProvider == null)
                return;

            ALFormat format = ALFormat.Mono16;
            int sampleRate = 44100;
            if (_streamProvider != null) {
                format = _streamProvider.Format;
                sampleRate = _streamProvider.SampleRate;
            }

            AudioCTX.ALCall(() => {
                AL.BufferData<short>(alBuffer, format, new Span<short>(s_NonThreadSafeTempBuffer, 0, BUFFER_SIZE), sampleRate);
            });
        }

        public double GetRealtimePlaybackPosition(OpenALSource? alSource) {
            if (alSource == null) {
                return _lastKnownCursorPosition;
            }

            int offsetIntoCurrentBuffer = alSource.GetSampleOffset();
            return _lastKnownCursorPosition + offsetIntoCurrentBuffer / (double)_streamProvider.SampleRate;
        }

        public void SetPlaybackPosition(OpenALSource? alSource, double pos) {
            bool wasPlaying = alSource != null &&
                alSource.GetSourceState() == ALSourceState.Playing;

            if (wasPlaying)
                Stop(alSource);

            _streamProvider.PlaybackPosition = pos;
            _lastKnownCursorPosition = _streamProvider.PlaybackPosition;

            if (wasPlaying)
                Play(alSource);
        }



        #region IDisposable pattern


        bool isDisposed = false;
        void Dispose(bool isDisposing) {
            if (isDisposed) return;
            isDisposed = true;

            for (int i = 0; i < _buffers.Length; i++) {
                AudioCTX.ALCall(() => {
                    AL.DeleteBuffer(_buffers[i]);
                });
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AudioStreamInput() {
            Dispose(false);
        }

        #endregion
    }


    /// <summary>
    /// Not sure if they are redundant or not yet, need to experiment with this 
    /// </summary>
    public struct StreamAdvanceResult {
        public int WriteCount;
        public double CursorPosSeconds;
    }


    /// <summary>
    /// Should be used to playback of audio by calling low level OpenAL Apis, through a single Play() call,
    /// streaming, or something else. 
    /// 
    /// Most of the time, custom audio generators can be created by implementing the IStreamDataProvider interface instead
    /// of this one
    /// 
    /// TODO: rename to IAudioStream
    /// </summary>
    public interface IAudioStreamProvider {
        /// <summary>
        /// advances the stream by dataUnitsToWrite (short)s, then 
        /// returns the new cursor position. This is to keep track of the current playback position.
        /// </summary>
        StreamAdvanceResult AdvanceStream(short[] outputBuffer, int dataUnitsToWrite);

        double PlaybackPosition {
            get; set;
        }
        double Duration {
            get;
        }

        ALFormat Format {
            get;
        }
        int SampleRate {
            get;
        }
        int Channels {
            get;
        }
    }
}

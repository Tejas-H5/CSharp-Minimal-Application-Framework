using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio
{
    //https://indiegamedev.net/2020/02/25/the-complete-guide-to-openal-with-c-part-2-streaming-audio/ as a reference

    /// <summary>
    /// Use this to play larger audio files, or for playing generated/streamed audio
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
    public class AudioSourceStreamed : AudioSource
    {
        const int NUM_BUFFERS = 4;
        const int BUFFER_SIZE = 65536; // 32kb of data in each buffer

        short[] _intermediateBuffer = new short[BUFFER_SIZE];

        int[] _buffers = new int[NUM_BUFFERS];

        IAudioStreamProvider _streamProvider;

        bool _isPlaying = false;

        public AudioSourceStreamed(bool relative, IAudioStreamProvider stream)
            : base(relative, false)
        {
            for (int i = 0; i < _buffers.Length; i++)
            {
                _buffers[i] = AL.GenBuffer();
                AL.BufferData(_buffers[i], stream.Format, _intermediateBuffer, stream.SampleRate);
                AL.SourceQueueBuffer(_alSource, _buffers[i]);
            }

            SetStream(stream);
        }

        public void SetStream(IAudioStreamProvider stream)
        {
            _streamProvider = stream;
        }

        public override double GetPlaybackPosition()
        {
            return _streamProvider.PlaybackPosition;
        }

        public override void SetPlaybackPosition(double pos)
        {
            _streamProvider.PlaybackPosition = pos;
        }

        public override void Play()
        {
            base.Play();
            _isPlaying = true;
        }

        public override void Pause()
        {
            base.Pause();
            _isPlaying = false;
        }

        public override void Stop()
        {
            base.Stop();
            _isPlaying = false;
        }

        public void UpdateStream()
        {
            if (!_isPlaying)
                return;

            int buffersProcessed = 0;
            AL.GetSource(_alSource, ALGetSourcei.BuffersProcessed, out buffersProcessed);

            while (buffersProcessed > 0)
            {
                int unqueuedBuffer = AL.SourceUnqueueBuffer(_alSource);

                CopyDataToIntermediateBuffer();

                AL.BufferData(unqueuedBuffer, _streamProvider.Format, _intermediateBuffer, _streamProvider.SampleRate);
                AL.SourceQueueBuffer(_alSource, unqueuedBuffer);

                buffersProcessed--;
            }
        }

        private void CopyDataToIntermediateBuffer()
        {
            int amountCopied = _streamProvider.AdvanceStream(_intermediateBuffer, BUFFER_SIZE);

            bool endOfStreamReached = amountCopied < BUFFER_SIZE;
            if (endOfStreamReached)
            {
                _streamProvider.PlaybackPosition = 0;
                _isPlaying = false;

                //Clear the end of the buffer
                for (int i = amountCopied; i < BUFFER_SIZE; i++)
                {
                    _intermediateBuffer[i] = 0;
                }
            }
        }


        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AudioClipStreamed()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }
        #endregion
    }
}

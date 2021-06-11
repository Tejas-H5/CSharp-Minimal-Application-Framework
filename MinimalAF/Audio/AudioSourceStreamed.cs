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

        short[] _tempBuffer = new short[BUFFER_SIZE];

        int[] _buffers = new int[NUM_BUFFERS];

        IAudioStreamProvider _streamProvider;
        int _lastStreamPosition = 0;

        bool _isPlaying = false;

        public AudioSourceStreamed(bool relative, IAudioStreamProvider stream)
            : base(relative, false)
        {
            InitializeBuffers();

            SetStream(stream);
        }

        private void InitializeBuffers()
        {
            CreateALBuffers();

            ClearBuffers();
        }

        public override void Play()
        {
            OpenALSource alSource = ALAudioSourcePool.AcquireSource(this);
            if (alSource == null)
                return;

            QueueAllBuffersForFirstTime(alSource);
            StartPlaying(alSource);
        }


        private void QueueAllBuffersForFirstTime(OpenALSource alSource)
        {
            ClearBuffers();

            for (int i = 0; i < _buffers.Length; i++)
            {
                alSource.QueueBuffer(_buffers[i]);
            }
        }


        private void CreateALBuffers()
        {
            for (int i = 0; i < _buffers.Length; i++)
            {
                CreateALBuffer(i);
            }
        }

        private void ClearBuffers()
        {
            ClearTempData();
            for (int i = 0; i < _buffers.Length; i++)
            {
                SendTempDataToBuffer(_buffers[i]);
            }
        }

        public override void Pause()
        {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            alSource.Pause();

            _isPlaying = false;
        }

        public override void Stop()
        {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            alSource.Stop();

            _isPlaying = false;
            _streamProvider.PlaybackPosition = 0;
            _lastStreamPosition = 0;
        }

        public void UpdateStream()
        {
            if (!_isPlaying)
                return;

            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            RotateProcessedBuffers(alSource);
        }

        private void RotateProcessedBuffers(OpenALSource alSource)
        {
            int buffersProcessed = alSource.GetBuffersProcessed();

            while (buffersProcessed > 0)
            {
                int nextBuffer = alSource.UnqueueBuffer();
                ReadStreamToTempDataBuffer();
                SendTempDataToBuffer(nextBuffer);
                alSource.QueueBuffer(nextBuffer);
                buffersProcessed--;
            }
        }

        private void ReadStreamToTempDataBuffer()
        {
            int amountCopied = _streamProvider.AdvanceStream(_tempBuffer, BUFFER_SIZE);

            bool endOfStreamReached = amountCopied < BUFFER_SIZE;
            if (endOfStreamReached)
            {
                _streamProvider.PlaybackPosition = 0;
                _isPlaying = false;

                //Clear the end of the buffer
                for (int i = amountCopied; i < BUFFER_SIZE; i++)
                {
                    _tempBuffer[i] = 0;
                }
            }
        }

        private void CreateALBuffer(int bufferIndex)
        {
            _buffers[bufferIndex] = AL.GenBuffer();
        }

        private void SendTempDataToBuffer(int alBuffer)
        {
            AL.BufferData(alBuffer, _streamProvider.Format, _tempBuffer, _streamProvider.SampleRate);
        }

        private void ClearTempData()
        {
            for (int i = 0; i < _tempBuffer.Length; i++)
            {
                _tempBuffer[i] = 0;
            }
        }

        private void StartPlaying(OpenALSource alSource)
        {
            alSource.Play();
            _isPlaying = true;
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
    }
}

using OpenTK.Audio.OpenAL;
using System;

namespace MinimalAF.Audio {
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
    public class AudioSourceStreamed : AudioSource {
        const int NUM_BUFFERS = 4;
        const int BUFFER_SIZE = 65536; // 32kb of data in each buffer

        short[] _tempBuffer = new short[BUFFER_SIZE];

        int[] _buffers = new int[NUM_BUFFERS];

        IAudioStreamProvider _streamProvider;
        int _lastStreamPosition = 0;

        bool _endOfStreamFound = false;
        int _finalBufferSize = 0;

        public AudioSourceStreamed(bool relative, IAudioStreamProvider stream = null)
            : base(relative, false) {
            InitializeBuffers();

            SetStream(stream);
        }

        private void InitializeBuffers() {
            CreateALBuffers();

            ClearBuffers();
        }

        public override void Play() {
            OpenALSource alSource = ALAudioSourcePool.AcquireSource(this);
            if (alSource == null)
                return;

            QueueAllBuffersForFirstTime(alSource);
            StartPlaying(alSource);
        }


        private void QueueAllBuffersForFirstTime(OpenALSource alSource) {
            if (_streamProvider == null)
                return;

            _streamProvider.PlaybackPosition = SamplesToSeconds(_lastStreamPosition);

            for (int i = 0; i < _buffers.Length; i++) {
                if (_endOfStreamFound)
                    return;

                int numCopied = ReadStreamToTempDataBuffer();
                SendTempDataToBuffer(_buffers[i], numCopied);
                alSource.QueueBuffer(_buffers[i]);
            }
        }


        private void CreateALBuffers() {
            for (int i = 0; i < _buffers.Length; i++) {
                CreateALBuffer(i);
            }
        }

        private void ClearBuffers() {
            ClearTempData();
            for (int i = 0; i < _buffers.Length; i++) {
                SendTempDataToBuffer(_buffers[i], BUFFER_SIZE);
            }
        }

        public override void Pause() {
            if (_streamProvider == null)
                return;

            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            int sampleOffset = alSource.GetSampleOffset();
            alSource.StopAndUnqueueAllBuffers();

            _lastStreamPosition += sampleOffset * _streamProvider.Channels;

            //important as it will prevent the call to Update() from advancing the stream.
            _endOfStreamFound = false;
        }

        private double SamplesToSeconds(int streamPos) {
            return streamPos / (double)(_streamProvider.SampleRate * _streamProvider.Channels);
        }

        public override void Stop() {
            if (_streamProvider == null)
                return;

            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            alSource.StopAndUnqueueAllBuffers();

            _streamProvider.PlaybackPosition = 0;
            _lastStreamPosition = 0;
            _endOfStreamFound = false;
        }

        public void UpdateStream() {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null) {
                if (_endOfStreamFound) {
                    _lastStreamPosition += _finalBufferSize;
                    _endOfStreamFound = false;
                }
                return;
            }

            RotateProcessedBuffers(alSource);
        }

        private void RotateProcessedBuffers(OpenALSource alSource) {
            int buffersProcessed = alSource.GetBuffersProcessed();

            while (buffersProcessed > 0) {
                int nextBuffer = alSource.UnqueueBuffer();

                if (!_endOfStreamFound) {
                    int numCopied = ReadStreamToTempDataBuffer();
                    SendTempDataToBuffer(nextBuffer, numCopied);
                    alSource.QueueBuffer(nextBuffer);
                }

                buffersProcessed--;

                int bufferSize = BUFFER_SIZE;
                _lastStreamPosition += bufferSize;

                Console.WriteLine(GetCurrentTime());
            }
        }

        private int ReadStreamToTempDataBuffer() {
            if (_streamProvider == null)
                return 0;

            int amountCopied = _streamProvider.AdvanceStream(_tempBuffer, BUFFER_SIZE);

            _endOfStreamFound = amountCopied < BUFFER_SIZE;

            if (_endOfStreamFound) {
                _finalBufferSize = amountCopied;
            }

            return amountCopied;
        }

        private void CreateALBuffer(int bufferIndex) {
            _buffers[bufferIndex] = AL.GenBuffer();
        }

        private void SendTempDataToBuffer(int alBuffer, int count) {
            if (_streamProvider == null)
                return;

            ALFormat format = ALFormat.Mono16;
            int sampleRate = 44100;
            if (_streamProvider != null) {
                format = _streamProvider.Format;
                sampleRate = _streamProvider.SampleRate;
            }

            AL.BufferData(alBuffer, format, new Span<short>(_tempBuffer, 0, count), sampleRate);
        }

        private void ClearTempData() {
            for (int i = 0; i < _tempBuffer.Length; i++) {
                _tempBuffer[i] = 0;
            }
        }

        private void StartPlaying(OpenALSource alSource) {
            alSource.Play();
        }

        public void SetStream(IAudioStreamProvider stream) {
            _streamProvider = stream;
        }

        public override double GetPlaybackPosition() {
            return GetCurrentTime();
        }


        double GetCurrentTime() {
            if (_streamProvider == null)
                return 0;

            int streamPos = _lastStreamPosition;

            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource != null)
                streamPos += alSource.GetSampleOffset() * _streamProvider.Channels;

            double lastPositionSeconds = SamplesToSeconds(streamPos);
            return lastPositionSeconds;
        }


        public override void SetPlaybackPosition(double pos) {
            if (_streamProvider == null)
                return;

            bool wasPlaying = GetSourceState() == AudioSourceState.Playing;

            if (wasPlaying)
                Stop();

            _streamProvider.PlaybackPosition = pos;
            _lastStreamPosition = (int)(Math.Floor(_streamProvider.PlaybackPosition * _streamProvider.SampleRate) * _streamProvider.Channels);

            if (wasPlaying)
                Play();
        }
    }
}

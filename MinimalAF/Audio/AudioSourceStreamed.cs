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

        short[] tempBuffer = new short[BUFFER_SIZE];

        int[] buffers = new int[NUM_BUFFERS];

        IAudioStreamProvider streamProvider;
        int lastStreamPosition = 0;

        bool endOfStreamFound = false;
        int finalBufferSize = 0;

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
            if (streamProvider == null)
                return;

            streamProvider.PlaybackPosition = SamplesToSeconds(lastStreamPosition);

            for (int i = 0; i < buffers.Length; i++) {
                if (endOfStreamFound)
                    return;

                int numCopied = ReadStreamToTempDataBuffer();
                SendTempDataToBuffer(buffers[i], numCopied);
                alSource.QueueBuffer(buffers[i]);
            }
        }


        private void CreateALBuffers() {
            for (int i = 0; i < buffers.Length; i++) {
                CreateALBuffer(i);
            }
        }

        private void ClearBuffers() {
            ClearTempData();
            for (int i = 0; i < buffers.Length; i++) {
                SendTempDataToBuffer(buffers[i], BUFFER_SIZE);
            }
        }

        public override void Pause() {
            if (streamProvider == null)
                return;

            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            int sampleOffset = alSource.GetSampleOffset();
            alSource.StopAndUnqueueAllBuffers();

            lastStreamPosition += sampleOffset * streamProvider.Channels;

            //important as it will prevent the call to Update() from advancing the stream.
            endOfStreamFound = false;
        }

        private double SamplesToSeconds(int streamPos) {
            return streamPos / (double)(streamProvider.SampleRate * streamProvider.Channels);
        }

        public override void Stop() {
            if (streamProvider == null)
                return;

            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            alSource.StopAndUnqueueAllBuffers();

            streamProvider.PlaybackPosition = 0;
            lastStreamPosition = 0;
            endOfStreamFound = false;
        }

        public void UpdateStream() {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null) {
                if (endOfStreamFound) {
                    lastStreamPosition += finalBufferSize;
                    endOfStreamFound = false;
                }
                return;
            }

            RotateProcessedBuffers(alSource);
        }

        private void RotateProcessedBuffers(OpenALSource alSource) {
            int buffersProcessed = alSource.GetBuffersProcessed();

            while (buffersProcessed > 0) {
                int nextBuffer = alSource.UnqueueBuffer();

                if (!endOfStreamFound) {
                    int numCopied = ReadStreamToTempDataBuffer();
                    SendTempDataToBuffer(nextBuffer, numCopied);
                    alSource.QueueBuffer(nextBuffer);
                }

                buffersProcessed--;

                int bufferSize = BUFFER_SIZE;
                lastStreamPosition += bufferSize;

                Console.WriteLine(GetCurrentTime());
            }
        }

        private int ReadStreamToTempDataBuffer() {
            if (streamProvider == null)
                return 0;

            int amountCopied = streamProvider.AdvanceStream(tempBuffer, BUFFER_SIZE);

            endOfStreamFound = amountCopied < BUFFER_SIZE;

            if (endOfStreamFound) {
                finalBufferSize = amountCopied;
            }

            return amountCopied;
        }

        private void CreateALBuffer(int bufferIndex) {
            buffers[bufferIndex] = AL.GenBuffer();
        }

        private void SendTempDataToBuffer(int alBuffer, int count) {
            if (streamProvider == null)
                return;

            ALFormat format = ALFormat.Mono16;
            int sampleRate = 44100;
            if (streamProvider != null) {
                format = streamProvider.Format;
                sampleRate = streamProvider.SampleRate;
            }

            AL.BufferData(alBuffer, format, new Span<short>(tempBuffer, 0, count), sampleRate);
        }

        private void ClearTempData() {
            for (int i = 0; i < tempBuffer.Length; i++) {
                tempBuffer[i] = 0;
            }
        }

        private void StartPlaying(OpenALSource alSource) {
            alSource.Play();
        }

        public void SetStream(IAudioStreamProvider stream) {
            streamProvider = stream;
        }

        public override double GetPlaybackPosition() {
            return GetCurrentTime();
        }


        double GetCurrentTime() {
            if (streamProvider == null)
                return 0;

            int streamPos = lastStreamPosition;

            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource != null)
                streamPos += alSource.GetSampleOffset() * streamProvider.Channels;

            double lastPositionSeconds = SamplesToSeconds(streamPos);
            return lastPositionSeconds;
        }


        public override void SetPlaybackPosition(double pos) {
            if (streamProvider == null)
                return;

            bool wasPlaying = GetSourceState() == AudioSourceState.Playing;

            if (wasPlaying)
                Stop();

            streamProvider.PlaybackPosition = pos;
            lastStreamPosition = (int)(Math.Floor(streamProvider.PlaybackPosition * streamProvider.SampleRate) * streamProvider.Channels);

            if (wasPlaying)
                Play();
        }
    }
}

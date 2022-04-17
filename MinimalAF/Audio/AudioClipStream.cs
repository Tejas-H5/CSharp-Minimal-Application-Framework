using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio {
    /// <summary>
    /// Should only be used by one audio source at a time, otherwise multiple sources
    /// will be advancing the stream forward so the audio will not play correctly.
    /// 
    /// Multiple AudioClipStreamed instances can however point to the same AudioData.
    /// </summary>
    public class AudioClipStream : IAudioStreamProvider {
        AudioData data;
        int cursor = 0;

        /// <summary>
        /// Can the playback position go negative?
        /// </summary>
        public bool UseSlackAtBegining {
            get; set;
        }

        /// <summary>
        /// Can the playback position go past the end?
        /// (However, the stream will still stop playing)
        /// </summary>
        public bool UseSlackAtEnd {
            get; set;
        }

        public AudioClipStream(AudioData data) {
            this.data = data;
        }

        public double PlaybackPosition {
            get {
                return cursor / (double)data.SampleRate / data.Channels;
            }

            set {
                cursor = data.Channels * (int)(value * data.SampleRate);
                ConstrainSlack();
            }
        }

        private void ConstrainSlack() {
            if (!UseSlackAtBegining && cursor < 0)
                cursor = 0;

            if (!UseSlackAtEnd && (cursor >= data.RawData.Length - data.Channels))
                cursor = data.RawData.Length - data.Channels;
        }

        public double Duration => data.Duration;

        public ALFormat Format => data.Format;

        public int SampleRate => data.SampleRate;

        public int Channels => data.Channels;

        public int AdvanceStream(short[] outputBuffer, int dataToWrite) {
            if (cursor < 0) {
                int zeroesToWrite = -cursor;
                if (zeroesToWrite > dataToWrite)
                    zeroesToWrite = dataToWrite;

                for (int i = 0; i < zeroesToWrite; i++) {
                    outputBuffer[i] = 0;
                }

                cursor += zeroesToWrite;

                if (zeroesToWrite == dataToWrite)
                    return dataToWrite;

                dataToWrite -= zeroesToWrite;
            }

            int dataLeft = data.RawData.Length - cursor;

            if (dataLeft <= 0)
                return 0;

            if (dataToWrite > dataLeft) {
                dataToWrite = dataLeft;
            }

            for (int i = 0; i < dataToWrite; i++) {
                outputBuffer[i] = data.RawData[cursor + i];
            }

            cursor += dataToWrite;

            return dataToWrite;
        }
    }
}

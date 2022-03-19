using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio {
    /// <summary>
    /// Should only be used by one audio source at a time, otherwise multiple sources
    /// will be advancing the stream forward so the audio will not play correctly.
    /// 
    /// Multiple AudioClipStreamed instances can however point to the same AudioData.
    /// </summary>
    public class AudioClipStream : IAudioStreamProvider {
        AudioData _data;
        int _cursor = 0;

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
            _data = data;
        }

        public double PlaybackPosition {
            get {
                return _cursor / (double)_data.SampleRate / _data.Channels;
            }

            set {
                _cursor = _data.Channels * (int)(value * _data.SampleRate);
                ConstrainSlack();
            }
        }

        private void ConstrainSlack() {
            if (!UseSlackAtBegining && _cursor < 0)
                _cursor = 0;

            if (!UseSlackAtEnd && (_cursor >= _data.RawData.Length - _data.Channels))
                _cursor = _data.RawData.Length - _data.Channels;
        }

        public double Duration => _data.Duration;

        public ALFormat Format => _data.Format;

        public int SampleRate => _data.SampleRate;

        public int Channels => _data.Channels;

        public int AdvanceStream(short[] outputBuffer, int dataToWrite) {
            if (_cursor < 0) {
                int zeroesToWrite = -_cursor;
                if (zeroesToWrite > dataToWrite)
                    zeroesToWrite = dataToWrite;

                for (int i = 0; i < zeroesToWrite; i++) {
                    outputBuffer[i] = 0;
                }

                _cursor += zeroesToWrite;

                if (zeroesToWrite == dataToWrite)
                    return dataToWrite;

                dataToWrite -= zeroesToWrite;
            }

            int dataLeft = _data.RawData.Length - _cursor;

            if (dataLeft <= 0)
                return 0;

            if (dataToWrite > dataLeft) {
                dataToWrite = dataLeft;
            }

            for (int i = 0; i < dataToWrite; i++) {
                outputBuffer[i] = _data.RawData[_cursor + i];
            }

            _cursor += dataToWrite;

            return dataToWrite;
        }
    }
}

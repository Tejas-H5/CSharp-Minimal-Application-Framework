using OpenTK.Audio.OpenAL;
using System;

namespace MinimalAF.Audio {
    /// <summary>
    /// Should only be used by one audio source at a time, otherwise multiple sources
    /// will be advancing the stream forward so the audio will not play correctly.
    /// 
    /// Multiple AudioClipStreamed instances can however point to the same AudioData.
    /// </summary>
    public class AudioClipStream : IAudioStreamProvider {
        AudioData data;
        long cursor = 0;

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
                cursor = (long)(value * data.SampleRate * data.Channels);
                cursor = ConstrainCursor(cursor);
            }
        }

        private long ConstrainCursor(long cursor) {
            if (!UseSlackAtBegining && cursor < 0)
                return 0;

            if (!UseSlackAtEnd && (cursor >= data.RawData.LongLength))
                return data.RawData.LongLength;

            return cursor;
        }

        public double Duration => data.Duration;

        public ALFormat Format => data.Format;

        public int SampleRate => data.SampleRate;

        public int Channels => data.Channels;

        public StreamAdvanceResult AdvanceStream(short[] outputBuffer, int dataToWrite) {
            cursor = ConstrainCursor(cursor);

            // if cursor is before the start, fill the buffer with zeroes till it isn't
            int i = 0;
            for (;i < dataToWrite && cursor < 0; cursor++, i++) {
                outputBuffer[i] = 0;
            }

            for (; i < dataToWrite && cursor < data.RawData.LongLength; cursor++, i++) {
                outputBuffer[i] = data.RawData[cursor];
            }

            if (UseSlackAtEnd) {
                for (; i < dataToWrite; i++) {
                    outputBuffer[i] = 0;
                }
            }

            return new StreamAdvanceResult {
                WriteCount = i,
                CursorPosSeconds = PlaybackPosition
            };
        }
    }
}

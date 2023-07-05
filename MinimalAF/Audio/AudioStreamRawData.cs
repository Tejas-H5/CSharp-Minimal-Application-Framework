using OpenTK.Audio.OpenAL;
using System;

namespace MinimalAF.Audio {
    public enum PlaybackEndBehaviourType {
        Loop,
        ContinueIntoTheVoid,
        Stop
    }

    /// <summary>
    /// Should only be used by one audio source at a time, otherwise multiple sources
    /// will be advancing the stream forward so the audio will not play correctly.
    /// 
    /// Multiple AudioClipStreamed instances can however point to the same AudioData.
    /// 
    /// TODO: rename to MemoryAudioStream for a naming convention that consistent with System.IO's MemoryStream
    /// </summary>
    public class AudioStreamRawData : IAudioStreamProvider {
        AudioRawData _data;
        long cursor = 0;

        public bool AllowPlaybackToGoNegative;
        public bool IsLooping;
        public PlaybackEndBehaviourType PlaybackEndBehaviour;


        public AudioStreamRawData(AudioRawData data) {
            this._data = data;
        }

        public double PlaybackPosition {
            get {
                return cursor / (double)_data.SampleRate / _data.Channels;
            }
            set {
                cursor = (long)(value * _data.SampleRate * _data.Channels);
                cursor = ConstrainCursor(cursor);
            }
        }

        private long ConstrainCursor(long cursor) {
            if (!AllowPlaybackToGoNegative && cursor < 0)
                return 0;

            if (cursor >= _data.Samples.LongLength) {
                if (PlaybackEndBehaviour == PlaybackEndBehaviourType.Stop) {
                    return _data.Samples.LongLength - 1;
                }

                if (PlaybackEndBehaviour == PlaybackEndBehaviourType.Loop) {
                    return 0;
                }

                return cursor;
            }

            return cursor;
        }

        public double Duration => _data.Duration;

        public ALFormat Format => _data.Format;

        public int SampleRate => _data.SampleRate;

        public int Channels => _data.Channels;

        public StreamAdvanceResult AdvanceStream(short[] outputBuffer, int dataToWrite) {
            cursor = ConstrainCursor(cursor);

            // if cursor is before the start, fill the buffer with zeroes till it isn't
            int i = 0;
            for (;i < dataToWrite && cursor < 0; cursor++, i++) {
                outputBuffer[i] = 0;
            }

            for (; i < dataToWrite && cursor < _data.Samples.LongLength; cursor++, i++) {
                outputBuffer[i] = _data.Samples[cursor];
            }

            if (PlaybackEndBehaviour == PlaybackEndBehaviourType.ContinueIntoTheVoid) {
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

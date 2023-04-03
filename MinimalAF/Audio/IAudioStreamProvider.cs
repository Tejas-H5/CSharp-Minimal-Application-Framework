using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio {

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

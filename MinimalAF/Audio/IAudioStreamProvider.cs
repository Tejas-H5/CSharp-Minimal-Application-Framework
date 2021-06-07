using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio
{
    /// <summary>
    /// Should be used to playback of audio by calling low level OpenAL Apis, through a single Play() call,
    /// streaming, or something else. 
    /// 
    /// Most of the time, custom audio generators can be created by implementing the IStreamDataProvider interface instead
    /// of this one
    /// </summary>
    public interface IAudioStreamProvider
    {
        int AdvanceStream(short[] outputBuffer, int dataUnitsToWrite);

        double PlaybackPosition { get; set; }
        double Duration { get; }

        ALFormat Format { get; }
        int SampleRate { get; }
        int Channels { get; }
    }
}

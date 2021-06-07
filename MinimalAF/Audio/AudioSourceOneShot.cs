using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio
{
    /// <summary>
    /// Use this one to load and play small sound effects. 
    /// Despite the name, the audio may be looped
    /// 
    /// Advantages:
    ///     - relatively easy to use, no audio updating subroutines/threads need to be implemented to use this
    ///     
    /// Disadvantages:
    ///     - Cannot play procedurally generated audio or anything that must be streamed from somewhere
    ///         - No support for custom signal processing effect chains as a consequence of this
    ///         - this use case requires AudioSourceStreamed
    /// </summary>
    public class AudioSourceOneShot : AudioSource
    {
        AudioClipOneShot _clip;

        public AudioSourceOneShot(bool relative, bool looping, AudioClipOneShot sound)
            : base(relative, looping)
        {
            SetAudioClip(sound);
        }

        public void SetAudioClip(AudioClipOneShot sound)
        {
            _clip = sound;
            SetBuffer(_clip.ALBuffer);
        }

        public override double GetPlaybackPosition()
        {
            float pos;
            AL.GetSource(_alSource, ALSourcef.SecOffset, out pos);
            return pos;
        }

        public override void SetPlaybackPosition(double pos)
        {
            AL.Source(_alSource, ALSourcef.SecOffset, (float)pos);
        }
    }
}

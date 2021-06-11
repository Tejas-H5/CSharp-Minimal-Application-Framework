using OpenTK.Mathematics;
using System;

namespace MinimalAF.Audio
{
    public abstract class AudioSource
    {
        private static int _nextAudioSourceID = 1;


        public int SourceID = -1;
        public override int GetHashCode()
        {
            return SourceID;
        }

        // If all OpenAL audio outputs are being used, and this priority number is higher than 
        // any of the sources using them, then the lower priority AudioSource will release it's
        // resource and give it to this one
        public int Priority = 0;


        public float Gain { get; set; } = 1;
        public float Pitch { get; set; } = 1;
        public bool Relative { get; set; } = false;
        public bool Looping { get; set; } = false;

        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public Vector3 Direction { get; set; } = Vector3.Zero;

        protected AudioSource(bool relative = false, bool looping = false)
        {
            SourceID = _nextAudioSourceID;
            _nextAudioSourceID++;
        }

        public abstract void Play();

        public abstract void Pause();

        public abstract void Stop();

        public abstract double GetPlaybackPosition();
        public abstract void SetPlaybackPosition(double pos);

        public AudioSourceState GetSourceState()
        {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return AudioSourceState.Stopped;

            return AudioSourceState.Playing;
        }
    }
}

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


        public float Gain { get; set; }
        public float Pitch { get; set; }
        public bool Relative { get; set; }
        public bool Looping { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 UpVector { get; set; }
        public Vector3 LookAtVector { get; set; }

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
            throw new NotImplementedException();
        }

    }
}

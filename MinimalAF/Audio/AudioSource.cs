using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using System;

namespace MinimalAF.Audio
{
    public abstract class AudioSource : IDisposable
    {
        protected int _alSource;
        protected internal int SourceID {
            get {
                return _alSource;
            }
        }

        protected bool _looping;
        protected bool _relative;

        public AudioSource(bool relative = false, bool looping = false)
        {
            _looping = looping;
            _relative = relative;

            AudioCTX.ALCall(out _alSource, () => { return AL.GenSource(); });
            AudioCTX.ALCall(() => { SetPitch(1); });
            AudioCTX.ALCall(() => { SetGain(1); });
            AudioCTX.ALCall(() => { SetPosition(0, 0, 0); });
            AudioCTX.ALCall(() => { SetVelocity(0, 0, 0); });
            AudioCTX.ALCall(() => { SetSourceRelative(_relative); });
            AudioCTX.ALCall(() => { SetLooping(_looping); });
        }


        public virtual void Play()
        {
            AL.SourcePlay(_alSource);
        }

        public virtual void Pause()
        {
            AL.SourcePause(_alSource);
        }

        public virtual void Stop()
        {
            AL.SourceStop(_alSource);
        }

        public abstract double GetPlaybackPosition();
        public abstract void SetPlaybackPosition(double pos);

        public AudioSourceState GetSourceState()
        {
            ALSourceState state = AL.GetSourceState(_alSource);

            return (AudioSourceState)state;
        }


        #region Wrappers for OpenAL getters and setters

        #region ALSourcef

        public float GetReferenceDistance()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.ReferenceDistance, out value);
            return value;
        }

        public AudioSource SetReferenceDistance(float value)
        {
            AL.Source(_alSource, ALSourcef.ReferenceDistance, value);
            return this;
        }

        public float GetMaxDistance()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.MaxDistance, out value);
            return value;
        }

        public AudioSource SetMaxDistance(float value)
        {
            AL.Source(_alSource, ALSourcef.MaxDistance, value);
            return this;
        }

        public float GetRolloffFactor()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.RolloffFactor, out value);
            return value;
        }

        public AudioSource SetRolloffFactor(float value)
        {
            AL.Source(_alSource, ALSourcef.RolloffFactor, value);
            return this;
        }

        public float GetPitch()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.Pitch, out value);
            return value;
        }

        public AudioSource SetPitch(float value)
        {
            AL.Source(_alSource, ALSourcef.Pitch, value);
            return this;
        }

        public float GetGain()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.Gain, out value);
            return value;
        }

        public AudioSource SetGain(float value)
        {
            AL.Source(_alSource, ALSourcef.Gain, value);
            return this;
        }

        public float GetMinGain()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.MinGain, out value);
            return value;
        }

        public AudioSource SetMinGain(float value)
        {
            AL.Source(_alSource, ALSourcef.MinGain, value);
            return this;
        }

        public float GetMaxGain()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.MaxGain, out value);
            return value;
        }

        public AudioSource SetMaxGain(float value)
        {
            AL.Source(_alSource, ALSourcef.MaxGain, value);
            return this;
        }

        public float GetConeInnerAngle()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.ConeInnerAngle, out value);
            return value;
        }

        public AudioSource SetConeInnerAngle(float value)
        {
            AL.Source(_alSource, ALSourcef.ConeInnerAngle, value);
            return this;
        }

        public float GetConeOuterAngle()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.ConeOuterAngle, out value);
            return value;
        }

        public AudioSource SetConeOuterAngle(float value)
        {
            AL.Source(_alSource, ALSourcef.ConeOuterAngle, value);
            return this;
        }

        public float GetConeOuterGain()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.ConeOuterGain, out value);
            return value;
        }

        public AudioSource SetConeOuterGain(float value)
        {
            AL.Source(_alSource, ALSourcef.ConeOuterGain, value);
            return this;
        }

        public float GetEfxAirAbsorptionFactor()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.EfxAirAbsorptionFactor, out value);
            return value;
        }

        public AudioSource SetEfxAirAbsorptionFactor(float value)
        {
            AL.Source(_alSource, ALSourcef.EfxAirAbsorptionFactor, value);
            return this;
        }

        public float GetEfxRoomRolloffFactor()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.EfxRoomRolloffFactor, out value);
            return value;
        }

        public AudioSource SetEfxRoomRolloffFactor(float value)
        {
            AL.Source(_alSource, ALSourcef.EfxRoomRolloffFactor, value);
            return this;
        }

        public float GetEfxConeOuterGainHighFrequency()
        {
            float value;
            AL.GetSource(_alSource, ALSourcef.EfxConeOuterGainHighFrequency, out value);
            return value;
        }

        public AudioSource SetEfxConeOuterGainHighFrequency(float value)
        {
            AL.Source(_alSource, ALSourcef.EfxConeOuterGainHighFrequency, value);
            return this;
        }

        #endregion

        #region ALSourcei		
        public int GetByteOffset()
        {
            int value;
            AL.GetSource(_alSource, ALGetSourcei.ByteOffset, out value);
            return value;
        }

        public int GetSampleOffset()
        {
            int value;
            AL.GetSource(_alSource, ALGetSourcei.SampleOffset, out value);
            return value;
        }

        public int GetBuffersProcessed()
        {
            int value;
            AL.GetSource(_alSource, ALGetSourcei.BuffersProcessed, out value);
            return value;
        }

        public int GetBuffer()
        {
            int value;
            AL.GetSource(_alSource, ALGetSourcei.Buffer, out value);
            return value;
        }

        public int GetSourceType()
        {
            int value;
            AL.GetSource(_alSource, ALGetSourcei.SourceType, out value);
            return value;
        }

        public AudioSource SetByteOffset(int value)
        {
            AL.Source(_alSource, ALSourcei.ByteOffset, value);
            return this;
        }

        public AudioSource SetSampleOffset(int value)
        {
            AL.Source(_alSource, ALSourcei.SampleOffset, value);
            return this;
        }

        public AudioSource SetBuffer(int value)
        {
            AL.Source(_alSource, ALSourcei.Buffer, value);
            return this;
        }

        public AudioSource SetSourceType(int value)
        {
            AL.Source(_alSource, ALSourcei.SourceType, value);
            return this;
        }

        //Does not contain a corresponding getter
        public AudioSource SetEfxDirectFilter(int value)
        {
            AL.Source(_alSource, ALSourcei.EfxDirectFilter, value);
            return this;
        }

        #endregion

        #region ALSourceb		
        public bool GetSourceRelative()
        {
            bool value;
            AL.GetSource(_alSource, ALSourceb.SourceRelative, out value);
            return value;
        }

        public AudioSource SetSourceRelative(bool value)
        {
            AL.Source(_alSource, ALSourceb.SourceRelative, value);
            return this;
        }
        public bool GetLooping()
        {
            bool value;
            AL.GetSource(_alSource, ALSourceb.Looping, out value);
            return value;
        }

        public AudioSource SetLooping(bool value)
        {
            AL.Source(_alSource, ALSourceb.Looping, value);
            return this;
        }
        public bool GetEfxDirectFilterGainHighFrequencyAuto()
        {
            bool value;
            AL.GetSource(_alSource, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, out value);
            return value;
        }

        public AudioSource SetEfxDirectFilterGainHighFrequencyAuto(bool value)
        {
            AL.Source(_alSource, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, value);
            return this;
        }
        public bool GetEfxAuxiliarySendFilterGainAuto()
        {
            bool value;
            AL.GetSource(_alSource, ALSourceb.EfxAuxiliarySendFilterGainAuto, out value);
            return value;
        }

        public AudioSource SetEfxAuxiliarySendFilterGainAuto(bool value)
        {
            AL.Source(_alSource, ALSourceb.EfxAuxiliarySendFilterGainAuto, value);
            return this;
        }
        public bool GetEfxAuxiliarySendFilterGainHighFrequencyAuto()
        {
            bool value;
            AL.GetSource(_alSource, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, out value);
            return value;
        }

        public AudioSource SetEfxAuxiliarySendFilterGainHighFrequencyAuto(bool value)
        {
            AL.Source(_alSource, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, value);
            return this;
        }

        #endregion

        #region ALSource3f		
        public Vector3 GetPosition()
        {
            Vector3 value;
            AL.GetSource(_alSource, ALSource3f.Position, out value);
            return value;
        }

        public AudioSource SetPosition(float x, float y, float z)
        {
            AL.Source(_alSource, ALSource3f.Position, x, y, -z);
            return this;
        }

        public Vector3 GetVelocity()
        {
            Vector3 value;
            AL.GetSource(_alSource, ALSource3f.Velocity, out value);
            return value;
        }

        public AudioSource SetVelocity(float x, float y, float z)
        {
            AL.Source(_alSource, ALSource3f.Velocity, x, y, -z);
            return this;
        }

        public Vector3 GetDirection()
        {
            Vector3 value;
            AL.GetSource(_alSource, ALSource3f.Direction, out value);
            return value;
        }

        public AudioSource SetDirection(float x, float y, float z)
        {
            AL.Source(_alSource, ALSource3f.Direction, x, y, -z);
            return this;
        }

        #endregion

        //ALSource3i properties seem to be duplicates of ALSource3f, so they will not be exposed here


        #endregion

        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        ~AudioSource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

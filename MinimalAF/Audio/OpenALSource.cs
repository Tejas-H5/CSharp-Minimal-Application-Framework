using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using System;

namespace MinimalAF.Audio
{
    /// <summary>
    /// A wrapper for OpenAL's audio source.
    /// Only a finite number of these classes may be created as determined by the OpenAL
    /// that is operating on a particular machine, so the factory pattern has been used
    /// </summary>
    internal class OpenALSource : IDisposable
    {
        protected int _alSourceId;
        protected internal int ALSourceID {
            get {
                return _alSourceId;
            }
        }

        public static OpenALSource CreateOpenALSource()
        {
            int alSource = 0;
            AudioCTX.ALCall(out alSource, () => {
                return AL.GenSource();
            }
            );

            if (alSource == 0)
            {
                return null;
            }

            return new OpenALSource(alSource);
        }

        private OpenALSource(int sourceId)
        {
            _alSourceId = sourceId;

            AudioCTX.ALCall(() => { SetPitch(1); });
            AudioCTX.ALCall(() => { SetGain(1); });
            AudioCTX.ALCall(() => { SetPosition(0, 0, 0); });
            AudioCTX.ALCall(() => { SetVelocity(0, 0, 0); });
            AudioCTX.ALCall(() => { SetSourceRelative(false); });
            AudioCTX.ALCall(() => { SetLooping(false); });
        }

        public virtual void Play()
        {
            AL.SourcePlay(_alSourceId);
        }

        public virtual void Pause()
        {
            AL.SourcePause(_alSourceId);
        }

        public virtual void Stop()
        {
            AL.SourceStop(_alSourceId);
        }

        public AudioSourceState GetSourceState()
        {
            ALSourceState state = AL.GetSourceState(_alSourceId);

            return (AudioSourceState)state;
        }

        public void QueueBuffer(int alBufferID)
        {
            AL.SourceQueueBuffer(_alSourceId, alBufferID);
        }

        public int UnqueueBuffer()
        {
            return AL.SourceUnqueueBuffer(_alSourceId);
        }

        /// <summary>
        /// This method clears the buffer queue on this source
        /// If you actually need to use the values of the unqueued buffers, then
        /// use UnqueueBuffer in conjunction with GetBuffersProcessed.
        /// </summary>
        public void StopAndUnqueueAllBuffers()
        {
            Stop();
            AL.SourceUnqueueBuffers(_alSourceId, GetBuffersProcessed());
        }


        #region Wrappers for OpenAL getters and setters

        #region ALSourcef

        public float GetSecOffset()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.SecOffset, out value);
            return value;
        }

        public void SetSecOffset(float value)
        {
            AL.Source(_alSourceId, ALSourcef.SecOffset, value);
        }

        public float GetReferenceDistance()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.ReferenceDistance, out value);
            return value;
        }

        public OpenALSource SetReferenceDistance(float value)
        {
            AL.Source(_alSourceId, ALSourcef.ReferenceDistance, value);
            return this;
        }

        public float GetMaxDistance()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.MaxDistance, out value);
            return value;
        }

        public OpenALSource SetMaxDistance(float value)
        {
            AL.Source(_alSourceId, ALSourcef.MaxDistance, value);
            return this;
        }

        public float GetRolloffFactor()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.RolloffFactor, out value);
            return value;
        }

        public OpenALSource SetRolloffFactor(float value)
        {
            AL.Source(_alSourceId, ALSourcef.RolloffFactor, value);
            return this;
        }

        public float GetPitch()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.Pitch, out value);
            return value;
        }

        public OpenALSource SetPitch(float value)
        {
            AL.Source(_alSourceId, ALSourcef.Pitch, value);
            return this;
        }

        public float GetGain()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.Gain, out value);
            return value;
        }

        public OpenALSource SetGain(float value)
        {
            AL.Source(_alSourceId, ALSourcef.Gain, value);
            return this;
        }

        public float GetMinGain()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.MinGain, out value);
            return value;
        }

        public OpenALSource SetMinGain(float value)
        {
            AL.Source(_alSourceId, ALSourcef.MinGain, value);
            return this;
        }

        public float GetMaxGain()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.MaxGain, out value);
            return value;
        }

        public OpenALSource SetMaxGain(float value)
        {
            AL.Source(_alSourceId, ALSourcef.MaxGain, value);
            return this;
        }

        public float GetConeInnerAngle()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.ConeInnerAngle, out value);
            return value;
        }

        public OpenALSource SetConeInnerAngle(float value)
        {
            AL.Source(_alSourceId, ALSourcef.ConeInnerAngle, value);
            return this;
        }

        public float GetConeOuterAngle()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.ConeOuterAngle, out value);
            return value;
        }

        public OpenALSource SetConeOuterAngle(float value)
        {
            AL.Source(_alSourceId, ALSourcef.ConeOuterAngle, value);
            return this;
        }

        public float GetConeOuterGain()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.ConeOuterGain, out value);
            return value;
        }

        public OpenALSource SetConeOuterGain(float value)
        {
            AL.Source(_alSourceId, ALSourcef.ConeOuterGain, value);
            return this;
        }

        public float GetEfxAirAbsorptionFactor()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.EfxAirAbsorptionFactor, out value);
            return value;
        }

        public OpenALSource SetEfxAirAbsorptionFactor(float value)
        {
            AL.Source(_alSourceId, ALSourcef.EfxAirAbsorptionFactor, value);
            return this;
        }

        public float GetEfxRoomRolloffFactor()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.EfxRoomRolloffFactor, out value);
            return value;
        }

        public OpenALSource SetEfxRoomRolloffFactor(float value)
        {
            AL.Source(_alSourceId, ALSourcef.EfxRoomRolloffFactor, value);
            return this;
        }

        public float GetEfxConeOuterGainHighFrequency()
        {
            float value;
            AL.GetSource(_alSourceId, ALSourcef.EfxConeOuterGainHighFrequency, out value);
            return value;
        }

        public OpenALSource SetEfxConeOuterGainHighFrequency(float value)
        {
            AL.Source(_alSourceId, ALSourcef.EfxConeOuterGainHighFrequency, value);
            return this;
        }

        #endregion

        #region ALSourcei		
        public int GetByteOffset()
        {
            int value;
            AL.GetSource(_alSourceId, ALGetSourcei.ByteOffset, out value);
            return value;
        }

        public int GetSampleOffset()
        {
            int value;
            AL.GetSource(_alSourceId, ALGetSourcei.SampleOffset, out value);
            return value;
        }

        public int GetBuffersProcessed()
        {
            int value;
            AL.GetSource(_alSourceId, ALGetSourcei.BuffersProcessed, out value);
            return value;
        }

        public int GetBuffer()
        {
            int value;
            AL.GetSource(_alSourceId, ALGetSourcei.Buffer, out value);
            return value;
        }

        public int GetBuffersQueued()
        {
            int value;
            AL.GetSource(_alSourceId, ALGetSourcei.BuffersQueued, out value);
            return value;
        }

        public int GetSourceType()
        {
            int value;
            AL.GetSource(_alSourceId, ALGetSourcei.SourceType, out value);
            return value;
        }

        public OpenALSource SetByteOffset(int value)
        {
            AL.Source(_alSourceId, ALSourcei.ByteOffset, value);
            return this;
        }

        public OpenALSource SetSampleOffset(int value)
        {
            AL.Source(_alSourceId, ALSourcei.SampleOffset, value);
            return this;
        }

        public OpenALSource SetBuffer(int value)
        {
            AL.Source(_alSourceId, ALSourcei.Buffer, value);
            return this;
        }

        public OpenALSource SetSourceType(int value)
        {
            AL.Source(_alSourceId, ALSourcei.SourceType, value);
            return this;
        }

        //Does not contain a corresponding getter
        public OpenALSource SetEfxDirectFilter(int value)
        {
            AL.Source(_alSourceId, ALSourcei.EfxDirectFilter, value);
            return this;
        }

        #endregion

        #region ALSourceb		
        public bool GetSourceRelative()
        {
            bool value;
            AL.GetSource(_alSourceId, ALSourceb.SourceRelative, out value);
            return value;
        }

        public OpenALSource SetSourceRelative(bool value)
        {
            AL.Source(_alSourceId, ALSourceb.SourceRelative, value);
            return this;
        }
        public bool GetLooping()
        {
            bool value;
            AL.GetSource(_alSourceId, ALSourceb.Looping, out value);
            return value;
        }

        public OpenALSource SetLooping(bool value)
        {
            AL.Source(_alSourceId, ALSourceb.Looping, value);
            return this;
        }
        public bool GetEfxDirectFilterGainHighFrequencyAuto()
        {
            bool value;
            AL.GetSource(_alSourceId, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, out value);
            return value;
        }

        public OpenALSource SetEfxDirectFilterGainHighFrequencyAuto(bool value)
        {
            AL.Source(_alSourceId, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, value);
            return this;
        }
        public bool GetEfxAuxiliarySendFilterGainAuto()
        {
            bool value;
            AL.GetSource(_alSourceId, ALSourceb.EfxAuxiliarySendFilterGainAuto, out value);
            return value;
        }

        public OpenALSource SetEfxAuxiliarySendFilterGainAuto(bool value)
        {
            AL.Source(_alSourceId, ALSourceb.EfxAuxiliarySendFilterGainAuto, value);
            return this;
        }
        public bool GetEfxAuxiliarySendFilterGainHighFrequencyAuto()
        {
            bool value;
            AL.GetSource(_alSourceId, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, out value);
            return value;
        }

        public OpenALSource SetEfxAuxiliarySendFilterGainHighFrequencyAuto(bool value)
        {
            AL.Source(_alSourceId, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, value);
            return this;
        }

        #endregion

        #region ALSource3f		
        public Vector3 GetPosition()
        {
            Vector3 value;
            AL.GetSource(_alSourceId, ALSource3f.Position, out value);
            return value;
        }

        public OpenALSource SetPosition(float x, float y, float z)
        {
            AL.Source(_alSourceId, ALSource3f.Position, x, y, -z);
            return this;
        }

        public Vector3 GetVelocity()
        {
            Vector3 value;
            AL.GetSource(_alSourceId, ALSource3f.Velocity, out value);
            return value;
        }

        public OpenALSource SetVelocity(float x, float y, float z)
        {
            AL.Source(_alSourceId, ALSource3f.Velocity, x, y, -z);
            return this;
        }

        public Vector3 GetDirection()
        {
            Vector3 value;
            AL.GetSource(_alSourceId, ALSource3f.Direction, out value);
            return value;
        }

        public OpenALSource SetDirection(float x, float y, float z)
        {
            AL.Source(_alSourceId, ALSource3f.Direction, x, y, -z);
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
                }

                AL.DeleteSource(_alSourceId);
                disposedValue = true;
            }
        }

        ~OpenALSource()
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

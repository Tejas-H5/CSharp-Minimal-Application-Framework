using System;
using Silk.NET.OpenAL;

namespace MinimalAF.Audio
{
	/// <summary>
	/// A wrapper for OpenAL's audio source.
	/// Only a finite number of these classes may be created as determined by the OpenAL
	/// that is operating on a particular machine, so the factory pattern has been used
	/// </summary>
	internal class OpenALSource : IDisposable
    {
        private uint _alSourceId;
        internal uint ALSourceID {
            get {
                return _alSourceId;
            }
        }

        public static OpenALSource CreateOpenALSource()
        {
            uint alSource = 0;
            AudioCTX.ALCall(out alSource, () => {
                return AudioCTX.AL.GenSource();
            }
            );

            if (alSource == 0)
            {
                return null;
            }

            return new OpenALSource(alSource);
        }

        private OpenALSource(uint sourceId)
        {
            _alSourceId = sourceId;
        }

        public void Play()
        {
            if (GetSourcePropertyState() == AudioSourceState.Playing)
                return;

            AudioCTX.ALCall(() => { AudioCTX.AL.SourcePlay(_alSourceId); });
            AudioSourceState state = GetSourcePropertyState();
        }

        public void Pause()
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SourcePause(_alSourceId); });
        }

        private void Stop()
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SourceStop(_alSourceId); });
        }

        public AudioSourceState GetSourcePropertyState()
        {
			AudioCTX.AL.GetSourceProperty(_alSourceId, GetSourceInteger.SourceState, out int s);
			SourceState state = (SourceState)s;

            return (AudioSourceState)state;
        }

        public void QueueBuffer(int alBufferID)
        {
            int bufferCount = GetBuffersQueued();
            AudioCTX.ALCall(() => { AudioCTX.AL.SourceQueueBuffer(_alSourceId, alBufferID); });
        }

        public int UnqueueBuffer()
        {
            int value;
            AudioCTX.ALCall(out value, () => { return AudioCTX.AL.SourceUnqueueBuffer(_alSourceId); });
            return value;
        }

        /// <summary>
        /// This method clears the buffer queue on this source
        /// If you actually need to use the values of the unqueued buffers, then
        /// use UnqueueBuffer in conjunction with GetBuffersProcessed.
        /// </summary>
        public void StopAndUnqueueAllBuffers()
        {
            Stop();
            int numQueued = GetBuffersQueued();
            if(numQueued > 0)
            {
                AudioCTX.ALCall(() => { AudioCTX.AL.SourceUnqueueBuffers(_alSourceId, numQueued); });
            }
            GetBuffersQueued();
            SetBuffer(0);
            ALSourceType type = AudioCTX.AL.GetSourcePropertyType(_alSourceId);
        }


        public void PullDataFrom(AudioSource virtualSource)
        {
            SetPosition(virtualSource.Position.X, virtualSource.Position.Y, virtualSource.Position.Z);
            SetVelocity(virtualSource.Velocity.X, virtualSource.Velocity.Y, virtualSource.Velocity.Z);
            SetDirection(virtualSource.Direction.X, virtualSource.Direction.Y, virtualSource.Direction.Z);

            SetGain(virtualSource.Gain);
            SetPitch(virtualSource.Pitch);

            SetLooping(virtualSource.Looping);
            SetSourceRelative(virtualSource.Relative);
        }


        #region Wrappers for OpenAL getters and setters

        #region SourceFloat

        public float GetSecOffset()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.SecOffset, out value);
            return value;
        }

        public void SetSecOffset(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSou(_alSourceId, SourceFloat.SecOffset, value); });
        }

        public float GetReferenceDistance()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.ReferenceDistance, out value);
            return value;
        }

        public OpenALSource SetReferenceDistance(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.ReferenceDistance, value); });
            return this;
        }

        public float GetMaxDistance()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.MaxDistance, out value);
            return value;
        }

        public OpenALSource SetMaxDistance(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.MaxDistance, value); });
            return this;
        }

        public float GetRolloffFactor()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.RolloffFactor, out value);
            return value;
        }

        public OpenALSource SetRolloffFactor(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.RolloffFactor, value); });
            return this;
        }

        public float GetPitch()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.Pitch, out value);
            return value;
        }

        public OpenALSource SetPitch(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.Pitch, value); });
            return this;
        }

        public float GetGain()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.Gain, out value);
            return value;
        }

        public OpenALSource SetGain(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.Gain, value); });
            return this;
        }

        public float GetMinGain()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.MinGain, out value);
            return value;
        }

        public OpenALSource SetMinGain(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.MinGain, value); });
            return this;
        }

        public float GetMaxGain()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.MaxGain, out value);
            return value;
        }

        public OpenALSource SetMaxGain(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.MaxGain, value); });
            return this;
        }

        public float GetConeInnerAngle()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.ConeInnerAngle, out value);
            return value;
        }

        public OpenALSource SetConeInnerAngle(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.ConeInnerAngle, value); });
            return this;
        }

        public float GetConeOuterAngle()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.ConeOuterAngle, out value);
            return value;
        }

        public OpenALSource SetConeOuterAngle(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.ConeOuterAngle, value); });
            return this;
        }

        public float GetConeOuterGain()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.ConeOuterGain, out value);
            return value;
        }

        public OpenALSource SetConeOuterGain(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.ConeOuterGain, value); });
            return this;
        }

        public float GetEfxAirAbsorptionFactor()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.EfxAirAbsorptionFactor, out value);
            return value;
        }

        public OpenALSource SetEfxAirAbsorptionFactor(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.EfxAirAbsorptionFactor, value); });
            return this;
        }

        public float GetEfxRoomRolloffFactor()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.EfxRoomRolloffFactor, out value);
            return value;
        }

        public OpenALSource SetEfxRoomRolloffFactor(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.EfxRoomRolloffFactor, value); });
            return this;
        }

        public float GetEfxConeOuterGainHighFrequency()
        {
            float value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, SourceFloat.EfxConeOuterGainHighFrequency, out value);
            return value;
        }

        public OpenALSource SetEfxConeOuterGainHighFrequency(float value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, SourceFloat.EfxConeOuterGainHighFrequency, value); });
            return this;
        }

        #endregion

        #region ALSourcei		
        public int GetByteOffset()
        {
            int value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALGetSourcePropertyi.ByteOffset, out value);
            return value;
        }

        public int GetSampleOffset()
        {
            int value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALGetSourcePropertyi.SampleOffset, out value);
            return value;
        }

        public int GetBuffersProcessed()
        {
            int value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALGetSourcePropertyi.BuffersProcessed, out value);
            return value;
        }

        public int GetBuffer()
        {
            int value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALGetSourcePropertyi.Buffer, out value);
            return value;
        }

        public int GetBuffersQueued()
        {
            int value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALGetSourcePropertyi.BuffersQueued, out value);
            return value;
        }

        public int GetSourcePropertyType()
        {
            int value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALGetSourcePropertyi.SourceType, out value);
            return value;
        }

        public OpenALSource SetByteOffset(int value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourcei.ByteOffset, value); });
            return this;
        }

        public OpenALSource SetSampleOffset(int value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourcei.SampleOffset, value); });
            return this;
        }

        public OpenALSource SetBuffer(int value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourcei.Buffer, value); });
            return this;
        }

        public OpenALSource SetSourceType(int value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourcei.SourceType, value); });
            return this;
        }

        //Does not contain a corresponding getter
        public OpenALSource SetEfxDirectFilter(int value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourcei.EfxDirectFilter, value); });
            return this;
        }

        #endregion

        #region ALSourceb		
        public bool GetSourcePropertyRelative()
        {
            bool value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALSourceb.SourceRelative, out value);
            return value;
        }

        public OpenALSource SetSourceRelative(bool value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourceb.SourceRelative, value); });
            return this;
        }
        public bool GetLooping()
        {
            bool value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALSourceb.Looping, out value);
            return value;
        }

        public OpenALSource SetLooping(bool value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourceb.Looping, value); });
            return this;
        }
        public bool GetEfxDirectFilterGainHighFrequencyAuto()
        {
            bool value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, out value);
            return value;
        }

        public OpenALSource SetEfxDirectFilterGainHighFrequencyAuto(bool value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, value); });
            return this;
        }
        public bool GetEfxAuxiliarySendFilterGainAuto()
        {
            bool value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALSourceb.EfxAuxiliarySendFilterGainAuto, out value);
            return value;
        }

        public OpenALSource SetEfxAuxiliarySendFilterGainAuto(bool value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourceb.EfxAuxiliarySendFilterGainAuto, value); });
            return this;
        }
        public bool GetEfxAuxiliarySendFilterGainHighFrequencyAuto()
        {
            bool value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, out value);
            return value;
        }

        public OpenALSource SetEfxAuxiliarySendFilterGainHighFrequencyAuto(bool value)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, value); });
            return this;
        }

        #endregion

        #region ALSource3f		
        public Vector3 GetPosition()
        {
            Vector3 value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALSource3f.Position, out value);
            return value;
        }

        public OpenALSource SetPosition(float x, float y, float z)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSource3f.Position, x, y, -z); });
            return this;
        }

        public Vector3 GetVelocity()
        {
            Vector3 value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALSource3f.Velocity, out value);
            return value;
        }

        public OpenALSource SetVelocity(float x, float y, float z)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSource3f.Velocity, x, y, -z); });
            return this;
        }

        public Vector3 GetDirection()
        {
            Vector3 value;
            AudioCTX.AL.GetSourceProperty(_alSourceId, ALSource3f.Direction, out value);
            return value;
        }

        public OpenALSource SetDirection(float x, float y, float z)
        {
            AudioCTX.ALCall(() => { AudioCTX.AL.SetSourceProperty(_alSourceId, ALSource3f.Direction, x, y, -z); });
            return this;
        }

        #endregion

        //ALSource3i properties seem to be duplicates of ALSource3f, so they will not be exposed here


        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                AudioCTX.AL.DeleteSource(_alSourceId);
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

using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using System;

namespace MinimalAF.Audio {
    /// <summary>
    /// A wrapper for OpenAL's audio source.
    /// Only a finite number of these classes may be created as determined by the OpenAL
    /// that is operating on a particular machine
    /// </summary>
    internal class OpenALSource : IDisposable {
        private int alSourceId;
        internal int ALSourceID => alSourceId;

        public static OpenALSource CreateOpenALSource() {
            int alSource = 0;
            AudioCTX.ALCall(out alSource, () => {
                return AL.GenSource();
            });

            if (alSource == 0) {
                return null;
            }

            return new OpenALSource(alSource);
        }

        private OpenALSource(int sourceId) {
            alSourceId = sourceId;
        }

        public void Play() {
            if (GetSourceState() == AudioSourceState.Playing)
                return;

            AudioCTX.ALCall(() => { AL.SourcePlay(alSourceId); });
        }

        public void Pause() {
            AudioCTX.ALCall(() => { AL.SourcePause(alSourceId); });
        }

        private void Stop() {
            AudioCTX.ALCall(() => { AL.SourceStop(alSourceId); });
        }

        public AudioSourceState GetSourceState() {
            ALSourceState state = AL.GetSourceState(alSourceId);

            return (AudioSourceState)state;
        }

        public void QueueBuffer(int alBufferID) {
            int bufferCount = GetBuffersQueued();
            AudioCTX.ALCall(() => {
                AL.SourceQueueBuffer(alSourceId, alBufferID);
            });
        }

        public int UnqueueBuffer() {
            int value;
            AudioCTX.ALCall(out value, () => {
                return AL.SourceUnqueueBuffer(alSourceId);
            });

            return value;
        }

        /// <summary>
        /// This method clears the buffer queue on this source.
        /// If you actually need to use the values of the unqueued buffers, then
        /// use UnqueueBuffer with GetBuffersProcessed.
        /// </summary>
        public void StopAndUnqueueAllBuffers() {
            Stop();
            int numQueued = GetBuffersQueued();
            if (numQueued > 0) {
                AudioCTX.ALCall(() => { AL.SourceUnqueueBuffers(alSourceId, numQueued); });
            }
            GetBuffersQueued();
            SetBuffer(0);
            ALSourceType type = AL.GetSourceType(alSourceId);
        }


        public void PullDataFrom(AudioSource virtualSource) {
            SetPosition(virtualSource.Position.X, virtualSource.Position.Y, virtualSource.Position.Z);
            SetVelocity(virtualSource.Velocity.X, virtualSource.Velocity.Y, virtualSource.Velocity.Z);
            SetDirection(virtualSource.Direction.X, virtualSource.Direction.Y, virtualSource.Direction.Z);

            SetGain(virtualSource.Gain);
            SetPitch(virtualSource.Pitch);

            SetLooping(virtualSource.Looping);
            SetSourceRelative(virtualSource.Relative);
        }


        #region Wrappers for OpenAL getters and setters

        #region ALSourcef

        public float GetSecOffset() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.SecOffset, out value);
            return value;
        }

        public void SetSecOffset(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.SecOffset, value); });
        }

        public float GetReferenceDistance() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.ReferenceDistance, out value);
            return value;
        }

        public OpenALSource SetReferenceDistance(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.ReferenceDistance, value); });
            return this;
        }

        public float GetMaxDistance() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.MaxDistance, out value);
            return value;
        }

        public OpenALSource SetMaxDistance(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.MaxDistance, value); });
            return this;
        }

        public float GetRolloffFactor() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.RolloffFactor, out value);
            return value;
        }

        public OpenALSource SetRolloffFactor(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.RolloffFactor, value); });
            return this;
        }

        public float GetPitch() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.Pitch, out value);
            return value;
        }

        public OpenALSource SetPitch(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.Pitch, value); });
            return this;
        }

        public float GetGain() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.Gain, out value);
            return value;
        }

        public OpenALSource SetGain(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.Gain, value); });
            return this;
        }

        public float GetMinGain() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.MinGain, out value);
            return value;
        }

        public OpenALSource SetMinGain(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.MinGain, value); });
            return this;
        }

        public float GetMaxGain() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.MaxGain, out value);
            return value;
        }

        public OpenALSource SetMaxGain(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.MaxGain, value); });
            return this;
        }

        public float GetConeInnerAngle() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.ConeInnerAngle, out value);
            return value;
        }

        public OpenALSource SetConeInnerAngle(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.ConeInnerAngle, value); });
            return this;
        }

        public float GetConeOuterAngle() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.ConeOuterAngle, out value);
            return value;
        }

        public OpenALSource SetConeOuterAngle(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.ConeOuterAngle, value); });
            return this;
        }

        public float GetConeOuterGain() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.ConeOuterGain, out value);
            return value;
        }

        public OpenALSource SetConeOuterGain(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.ConeOuterGain, value); });
            return this;
        }

        public float GetEfxAirAbsorptionFactor() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.EfxAirAbsorptionFactor, out value);
            return value;
        }

        public OpenALSource SetEfxAirAbsorptionFactor(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.EfxAirAbsorptionFactor, value); });
            return this;
        }

        public float GetEfxRoomRolloffFactor() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.EfxRoomRolloffFactor, out value);
            return value;
        }

        public OpenALSource SetEfxRoomRolloffFactor(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.EfxRoomRolloffFactor, value); });
            return this;
        }

        public float GetEfxConeOuterGainHighFrequency() {
            float value;
            AL.GetSource(alSourceId, ALSourcef.EfxConeOuterGainHighFrequency, out value);
            return value;
        }

        public OpenALSource SetEfxConeOuterGainHighFrequency(float value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcef.EfxConeOuterGainHighFrequency, value); });
            return this;
        }

        #endregion

        #region ALSourcei		
        public int GetByteOffset() {
            int value;
            AL.GetSource(alSourceId, ALGetSourcei.ByteOffset, out value);
            return value;
        }

        public int GetSampleOffset() {
            int value;
            AL.GetSource(alSourceId, ALGetSourcei.SampleOffset, out value);
            return value;
        }

        public int GetBuffersProcessed() {
            int value;
            AL.GetSource(alSourceId, ALGetSourcei.BuffersProcessed, out value);
            return value;
        }

        public int GetBuffer() {
            int value;
            AL.GetSource(alSourceId, ALGetSourcei.Buffer, out value);
            return value;
        }

        public int GetBuffersQueued() {
            int value;
            AL.GetSource(alSourceId, ALGetSourcei.BuffersQueued, out value);
            return value;
        }

        public int GetSourceType() {
            int value;
            AL.GetSource(alSourceId, ALGetSourcei.SourceType, out value);
            return value;
        }

        public OpenALSource SetByteOffset(int value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcei.ByteOffset, value); });
            return this;
        }

        public OpenALSource SetSampleOffset(int value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcei.SampleOffset, value); });
            return this;
        }

        public OpenALSource SetBuffer(int value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcei.Buffer, value); });
            return this;
        }

        public OpenALSource SetSourceType(int value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcei.SourceType, value); });
            return this;
        }

        //Does not contain a corresponding getter
        public OpenALSource SetEfxDirectFilter(int value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourcei.EfxDirectFilter, value); });
            return this;
        }

        #endregion

        #region ALSourceb		
        public bool GetSourceRelative() {
            bool value;
            AL.GetSource(alSourceId, ALSourceb.SourceRelative, out value);
            return value;
        }

        public OpenALSource SetSourceRelative(bool value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourceb.SourceRelative, value); });
            return this;
        }
        public bool GetLooping() {
            bool value;
            AL.GetSource(alSourceId, ALSourceb.Looping, out value);
            return value;
        }

        public OpenALSource SetLooping(bool value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourceb.Looping, value); });
            return this;
        }
        public bool GetEfxDirectFilterGainHighFrequencyAuto() {
            bool value;
            AL.GetSource(alSourceId, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, out value);
            return value;
        }

        public OpenALSource SetEfxDirectFilterGainHighFrequencyAuto(bool value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourceb.EfxDirectFilterGainHighFrequencyAuto, value); });
            return this;
        }
        public bool GetEfxAuxiliarySendFilterGainAuto() {
            bool value;
            AL.GetSource(alSourceId, ALSourceb.EfxAuxiliarySendFilterGainAuto, out value);
            return value;
        }

        public OpenALSource SetEfxAuxiliarySendFilterGainAuto(bool value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourceb.EfxAuxiliarySendFilterGainAuto, value); });
            return this;
        }
        public bool GetEfxAuxiliarySendFilterGainHighFrequencyAuto() {
            bool value;
            AL.GetSource(alSourceId, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, out value);
            return value;
        }

        public OpenALSource SetEfxAuxiliarySendFilterGainHighFrequencyAuto(bool value) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSourceb.EfxAuxiliarySendFilterGainHighFrequencyAuto, value); });
            return this;
        }

        #endregion

        #region ALSource3f		
        public Vector3 GetPosition() {
            Vector3 value;
            AL.GetSource(alSourceId, ALSource3f.Position, out value);
            return value;
        }

        public OpenALSource SetPosition(float x, float y, float z) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSource3f.Position, x, y, -z); });
            return this;
        }

        public Vector3 GetVelocity() {
            Vector3 value;
            AL.GetSource(alSourceId, ALSource3f.Velocity, out value);
            return value;
        }

        public OpenALSource SetVelocity(float x, float y, float z) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSource3f.Velocity, x, y, -z); });
            return this;
        }

        public Vector3 GetDirection() {
            Vector3 value;
            AL.GetSource(alSourceId, ALSource3f.Direction, out value);
            return value;
        }

        public OpenALSource SetDirection(float x, float y, float z) {
            AudioCTX.ALCall(() => { AL.Source(alSourceId, ALSource3f.Direction, x, y, -z); });
            return this;
        }

        #endregion

        //ALSource3i properties seem to be duplicates of ALSource3f, so they will not be exposed here


        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                }

                AL.DeleteSource(alSourceId);
                disposedValue = true;
            }
        }

        ~OpenALSource() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

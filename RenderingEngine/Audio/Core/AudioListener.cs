using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace MinimalAF.Audio.Core
{
    public class AudioListener
    {
        public float Gain { get; private set; } = 1;
        public float EfxMetersPerUnit { get; private set; } = 1;

        public Vector3 Position { get; private set; } = new Vector3(0, 0, 0);
        public Vector3 Velocity { get; private set; } = new Vector3(0, 0, 0);
        public Vector3 OrientationLookAt { get; private set; } = new Vector3(0, 0, -1);
        public Vector3 OrientationUp { get; private set; } = new Vector3(0, 1, 0);

        public AudioListener SetGain(float value)
        {
            Gain = value;
            AudioCTX.UpdateListenerGain(this);
            return this;
        }
        public AudioListener SetEfxMetersPerUnit(float value)
        {
            EfxMetersPerUnit = value;
            AudioCTX.UpdateListenerEfxMetersPerUnit(this);
            return this;
        }

        public AudioListener SetOrientation(float lookAtX, float lookAtY, float lookAtZ, float upX, float upY, float upZ)
        {
            OrientationLookAt = new Vector3(lookAtX, lookAtY, -lookAtZ);
            OrientationUp = new Vector3(upX, upY, upZ);

            AudioCTX.UpdateListenerOrientation(this);
            return this;
        }

        public AudioListener SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
            AudioCTX.UpdateListenerPosition(this);
            return this;
        }

        public AudioListener SetVelocity(float x, float y, float z)
        {
            Velocity = new Vector3(x, y, z);
            AudioCTX.UpdateListenerVelocity(this);
            return this;
        }
    }
}

using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace MinimalAF.Audio {
    public class AudioListener {
        public float Gain { get; set; } = 1;
        public float EfxMetersPerUnit { get; set; } = 1;

        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
        public Vector3 Velocity { get; set; } = new Vector3(0, 0, 0);
        public Vector3 OrientationLookAt { get; set; } = new Vector3(0, 0, -1);
        public Vector3 OrientationUp { get; set; } = new Vector3(0, 1, 0);

        public void SetGain(float value) {
            Gain = value;
        }
        public void SetEfxMetersPerUnit(float value) {
            EfxMetersPerUnit = value;
        }

        public void SetOrientation(float lookAtX, float lookAtY, float lookAtZ, float upX, float upY, float upZ) {
            OrientationLookAt = new Vector3(lookAtX, lookAtY, -lookAtZ);
            OrientationUp = new Vector3(upX, upY, upZ);
        }

        public AudioListener MakeCurrent() {
            AudioCTX.SetCurrentListener(this);
            return this;
        }

        public void Update() {
            AudioCTX.ALCall(() => {
                AL.Listener(ALListenerf.Gain, Gain);
                AL.Listener(ALListenerf.EfxMetersPerUnit, EfxMetersPerUnit);
                AL.Listener(ALListener3f.Position, Position.X, Position.Y, Position.Z);
                AL.Listener(ALListener3f.Velocity, Velocity.X, Velocity.Y, Velocity.Z);

                var at = OrientationLookAt;
                var up = OrientationUp;
                AL.Listener(ALListenerfv.Orientation, ref at, ref up);
            });
        }
    }
}

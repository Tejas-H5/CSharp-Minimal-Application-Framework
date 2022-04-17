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

        public void SetPosition(float x, float y, float z) {
            Position = new Vector3(x, y, z);
        }

        public void SetVelocity(float x, float y, float z) {
            Velocity = new Vector3(x, y, z);
        }

        public AudioListener MakeCurrent() {
            ALAudioListener.CurrentSelectedInstance = this;
            return this;
        }
    }
}

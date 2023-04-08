using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace MinimalAF.Audio {
    public class AudioListener {
        public float Gain = 1;
        public float EfxMetersPerUnit = 1;
        public Vector3 Position = new Vector3(0, 0, 0);
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Velocity = new Vector3(0, 0, 0);
        public Vector3 OrientationUp = new Vector3(0, 1, 0);

        public void MakeCurrent() {
            AudioCTX.SetCurrentListener(this);
        }

        public void Update() {
            AudioCTX.ALCall(() => {
                AL.Listener(ALListenerf.Gain, Gain);
                AL.Listener(ALListenerf.EfxMetersPerUnit, EfxMetersPerUnit);
                AL.Listener(ALListener3f.Position, Position.X, Position.Y, Position.Z);
                AL.Listener(ALListener3f.Velocity, Velocity.X, Velocity.Y, Velocity.Z);

                var lookAt = Rotation * new Vector3(0, 0, 1);
                var up = Rotation * new Vector3(0, 1, 0);
                AL.Listener(ALListenerfv.Orientation, ref lookAt, ref up);
            });
        }
    }
}

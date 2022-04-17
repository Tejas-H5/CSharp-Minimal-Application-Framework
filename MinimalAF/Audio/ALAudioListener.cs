using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio {
    internal static class ALAudioListener {
        private static AudioListener currentSelectedListener;
        public static AudioListener CurrentSelectedInstance {
            get {
                return currentSelectedListener;
            }
            set {
                SetCurrentListener(value);
            }
        }

        public static void Update() {
            SendCurrentListenerDataToOpenAL();
        }

        public static void Cleanup() {
            CurrentSelectedInstance = null;
        }


        public static void SetCurrentListener(AudioListener instance) {
            currentSelectedListener = instance;
            SendCurrentListenerDataToOpenAL();
        }

        private static void SendCurrentListenerDataToOpenAL() {
            if (currentSelectedListener == null)
                return;

            AL.Listener(ALListenerf.Gain, currentSelectedListener.Gain);
            AL.Listener(ALListenerf.EfxMetersPerUnit, currentSelectedListener.EfxMetersPerUnit);
            AL.Listener(ALListener3f.Position, currentSelectedListener.Position.X, currentSelectedListener.Position.Y, currentSelectedListener.Position.Z);
            AL.Listener(ALListener3f.Velocity, currentSelectedListener.Velocity.X, currentSelectedListener.Velocity.Y, currentSelectedListener.Velocity.Z);

            var at = currentSelectedListener.OrientationLookAt;
            var up = currentSelectedListener.OrientationUp;
            AL.Listener(ALListenerfv.Orientation, ref at, ref up);
        }
    }
}

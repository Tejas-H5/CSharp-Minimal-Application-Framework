using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio {
    internal static class ALAudioListener {
        private static AudioListener _currentSelectedListener;
        public static AudioListener CurrentSelectedInstance {
            get {
                return _currentSelectedListener;
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
            _currentSelectedListener = instance;
            SendCurrentListenerDataToOpenAL();
        }

        private static void SendCurrentListenerDataToOpenAL() {
            if (_currentSelectedListener == null)
                return;

            AL.Listener(ALListenerf.Gain, _currentSelectedListener.Gain);
            AL.Listener(ALListenerf.EfxMetersPerUnit, _currentSelectedListener.EfxMetersPerUnit);
            AL.Listener(ALListener3f.Position, _currentSelectedListener.Position.X, _currentSelectedListener.Position.Y, _currentSelectedListener.Position.Z);
            AL.Listener(ALListener3f.Velocity, _currentSelectedListener.Velocity.X, _currentSelectedListener.Velocity.Y, _currentSelectedListener.Velocity.Z);

            var at = _currentSelectedListener.OrientationLookAt;
            var up = _currentSelectedListener.OrientationUp;
            AL.Listener(ALListenerfv.Orientation, ref at, ref up);
        }
    }
}

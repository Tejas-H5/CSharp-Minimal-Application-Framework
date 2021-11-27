namespace MinimalAF.Audio
{
	internal static class ALAudioListener
    {
        private static AudioListener _currentSelectedListener;
        public static AudioListener CurrentSelectedInstance {
            get {
                return _currentSelectedListener;
            }
            set {
                SetCurrentListener(value);
            }
        }

        public static void Update()
        {
            SendCurrentListenerDataToOpenAL();
        }

        public static void Cleanup()
        {
            CurrentSelectedInstance = null;
        }


        public static void SetCurrentListener(AudioListener instance)
        {
            _currentSelectedListener = instance;
            SendCurrentListenerDataToOpenAL();
        }

        private static void SendCurrentListenerDataToOpenAL()
        {
            if (_currentSelectedListener == null)
                return;

            AudioCTX.AL.Listener(ALListenerf.Gain, _currentSelectedListener.Gain);
            AudioCTX.AL.Listener(ALListenerf.EfxMetersPerUnit, _currentSelectedListener.EfxMetersPerUnit);
            AudioCTX.AL.Listener(ALListener3f.Position, _currentSelectedListener.Position.X, _currentSelectedListener.Position.Y, _currentSelectedListener.Position.Z);
            AudioCTX.AL.Listener(ALListener3f.Velocity, _currentSelectedListener.Velocity.X, _currentSelectedListener.Velocity.Y, _currentSelectedListener.Velocity.Z);

            var at = _currentSelectedListener.OrientationLookAt;
            var up = _currentSelectedListener.OrientationUp;
            AudioCTX.AL.Listener(ALListenerfv.Orientation, ref at, ref up);
        }
    }
}

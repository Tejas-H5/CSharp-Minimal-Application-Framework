using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

namespace MinimalAF.Audio {
    public enum PlaybackState {
        Playing,
        Paused,
        Stopped
    }

    public interface IAudioSourceInput {
        void Play(OpenALSource alSource);
        void Pause(OpenALSource alSource);
        void Stop(OpenALSource alSource);
        void Update(OpenALSource alSource);
        double GetRealtimePlaybackPosition(OpenALSource? alSource);
        void SetPlaybackPosition(OpenALSource? alSource, double pos);
    }

    // This class is heavily coupled to AudioCTX
    public class AudioSource {
        // This field is heavily coupled to AudioCTX
        internal int Index = -1;
        private OpenALSource? CurrentALSource {
            get {
                if (Index == -1) {
                    return null;
                }

                return AudioCTX.GetALSourceForAudioSource(this);
            }
        }

        // In theory, this will cause a tonne of bugs, so we are not going to be entertaining this for now.
        // (even though it may have been the whole point)
        //// If all OpenAL audio outputs are being used, and this priority number is higher than 
        //// any of the sources using them, then the lower priority AudioSource will release it's
        //// resource and give it to this one
        // public int Priority = 0;
        public float Gain { get; set; } = 1;
        public float Pitch { get; set; } = 1;
        public bool Relative { get; set; } = false;
        public bool Looping { get; set; } = false;
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Velocity { get; set; } = Vector3.Zero;
        public Vector3 Direction { get; set; } = Vector3.Zero;


        IAudioSourceInput _currentInput;

        public void SetInput(IAudioSourceInput input) {
            if (_currentInput != null) {
                // TODO
            }

            _currentInput = input;
        }

        public void Play() {
            if (_currentInput == null) return;

            var alSource = AudioCTX.GetALSourceForAudioSource(this);
            if (alSource == null) {
                return;
            }

            _currentInput.Play(alSource);
        }


        public void Pause() {
            if (_currentInput == null || CurrentALSource == null) return;

            _currentInput.Pause(CurrentALSource);
        }

        public void Stop() {
            if (_currentInput == null || CurrentALSource == null) return;

            _currentInput.Stop(CurrentALSource);
        }

        public double PlaybackPosition {
            get {
                if (_currentInput == null) return 0;

                return _currentInput.GetRealtimePlaybackPosition(CurrentALSource);
            }

            set {
                if (_currentInput == null) return;

                _currentInput.SetPlaybackPosition(CurrentALSource, value);
            }
        }

        public void Update() {
            if (_currentInput == null || CurrentALSource == null) return;

            _currentInput.Update(CurrentALSource);
        }

        public PlaybackState PlaybackState {
            get {
                if (_currentInput == null || CurrentALSource == null) {
                    return PlaybackState.Stopped;
                }

                var state = CurrentALSource.GetSourceState();
                if (state == ALSourceState.Playing) {
                    return PlaybackState.Playing;
                }

                if (state == ALSourceState.Paused) {
                    return PlaybackState.Paused;
                }

                return PlaybackState.Stopped;
            }
        }

        /// <summary>
        /// Returns if our system thinks this is playing something, and is high-enough 
        /// priority for that something to make it to the speakers.
        /// 
        /// (Basically, is this audio source connected to an OpenAL source from the source pool?)
        /// </summary>
        public bool IsActive() {
            return Index != -1;
        }
    }
}

namespace MinimalAF.Audio {
    /// <summary>
    /// Use this one to load and play small sound effects. 
    /// Despite the name, the audio may be looped
    /// 
    /// Advantages:
    ///     - relatively easy to use, no audio updating subroutines/threads need to be implemented to use this
    ///     
    /// Disadvantages:
    ///     - Cannot play procedurally generated audio or anything that must be streamed from somewhere
    ///         - No support for custom signal processing effect chains as a consequence of this
    ///         - this use case requires AudioSourceStreamed
    /// </summary>
    public class AudioSourceOneShot : AudioSource {
        AudioClipOneShot clip;
        float pausedTime = 0;

        public AudioSourceOneShot(bool relative, bool looping, AudioClipOneShot sound = null)
            : base(relative, looping) {
            SetAudioClip(sound);
        }

        public void SetAudioClip(AudioClipOneShot sound) {
            clip = sound;

            if (sound == null)
                return;
        }


        public override void Play() {
            playInternal(pausedTime);
        }

        public void Play(float offset) {
            playInternal(offset);
        }

        private void playInternal(float offset) {
            if (clip == null)
                return;

            OpenALSource alSource = ALAudioSourcePool.AcquireSource(this);
            if (alSource == null)
                return;

            alSource.SetBuffer(clip.ALBuffer);
            alSource.SetSecOffset(offset);

            alSource.Play();
        }

        public override void Pause() {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            alSource.Pause();

            pausedTime = alSource.GetSecOffset();
        }

        public override void Stop() {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            alSource.StopAndUnqueueAllBuffers();

            pausedTime = 0;
        }

        public override double GetPlaybackPosition() {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return 0;

            return alSource.GetSecOffset();
        }

        public override void SetPlaybackPosition(double pos) {
            OpenALSource alSource = ALAudioSourcePool.GetActiveSource(this);
            if (alSource == null)
                return;

            alSource.SetSecOffset((float)pos);
        }

    }
}

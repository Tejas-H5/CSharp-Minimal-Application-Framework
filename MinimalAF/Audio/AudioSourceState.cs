using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio {
    public enum AudioSourceState {
        Initial = ALSourceState.Initial,
        Playing = ALSourceState.Playing,
        Paused = ALSourceState.Paused,
        Stopped = ALSourceState.Stopped
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Audio.OpenAL;

namespace MinimalAF.Audio.Core
{
    public enum AudioSourceState
    {
        Initial = ALSourceState.Initial,
        Playing = ALSourceState.Playing,
        Paused = ALSourceState.Paused,
        Stopped = ALSourceState.Stopped
    }
}

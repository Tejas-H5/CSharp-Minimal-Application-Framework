using MinimalAF;
using MinimalAF.Audio;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AudioEngineTests.AudioTests {
    // This test might not be needed
    public class BasicWavPlayingTest : IRenderable {
        AudioSourceOneShot _clackSound;
        AudioListener _listener;
        DrawableFont _font = new DrawableFont("Consolas", 16);

        List<AudioSource> _sources = new List<AudioSource>();

        public BasicWavPlayingTest() {
            _clackSound = new AudioSourceOneShot(
                AudioData.FromFile("./Res/keyboardClack0.wav")
            );

            _listener = new AudioListener().MakeCurrent();
        }

        int sourceCount = 0;
        public void Render(FrameworkContext ctx) {
            // TODO: implement.
            // I want exactly n sources playing at some given time. or something likle that


            while(_sources.Count < sourceCount) {

            }
        }
    }
}

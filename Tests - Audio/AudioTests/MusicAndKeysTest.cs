using MinimalAF;
using MinimalAF.Audio;
using System;
using System.Text;

namespace AudioEngineTests.AudioTests {
    // Test that the music and clips can both play at the same time. 
    // Right now, this test is failing imo. The one-shot sounds are not audible over the music.
    public class MusicAndKeysTest : IRenderable {
        AudioSourceStreamed streamedSource;
        AudioClipStream streamProvider;

        BasicWavPlayingTest basicWavPlayingTest;
        MusicPlayingTest musicPlayingTest;

        public MusicAndKeysTest() {
            basicWavPlayingTest = new BasicWavPlayingTest();
            musicPlayingTest = new MusicPlayingTest();
        }

        public void Render(FrameworkContext ctx) {
            basicWavPlayingTest.Render(ctx);
            musicPlayingTest.Render(ctx);
        }
    }
}

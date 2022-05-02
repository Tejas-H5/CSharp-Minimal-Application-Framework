using System.Collections.Generic;

namespace MinimalAF.Audio {
    internal static class ALAudioSourcePool {
        const int MAX_AUDIO_SOURCES = 256;
        private static Stack<OpenALSource> allAvailableOpenALSources = new Stack<OpenALSource>();

        private static Dictionary<AudioSource, OpenALSource> activeSources = new Dictionary<AudioSource, OpenALSource>();
        private static List<AudioSource> unactiveList = new List<AudioSource>();

        internal static void Init() {
            // used to contain stuff, for now it is a no-op
        }


        internal static void Update() {
            UpdateActiveSources();

            ReclaimInactiveSources();
        }

        private static void UpdateActiveSources() {
            foreach ((AudioSource virtualSource, OpenALSource alSource) in activeSources) {
                alSource.PullDataFrom(virtualSource);
            }
        }

        internal static void Cleanup() {
            DisposeAllOpenALSources();
        }

        private static void DisposeAllOpenALSources() {
            ReclaimAll();
            DisposeAll();
        }

        internal static OpenALSource AcquireSource(AudioSource audioSource) {
            OpenALSource active = GetActiveSource(audioSource);
            if (active != null)
                return active;


            if (allAvailableOpenALSources.Count > 0) {
                OpenALSource newALSource = allAvailableOpenALSources.Pop();
                return AssignALSourceToAudioSource(newALSource, audioSource);
            }


            AudioSource lowerPrioritiySource = null;
            foreach ((AudioSource audioSourceLoop, OpenALSource alSource) in activeSources) {
                if (audioSourceLoop.Priority < audioSource.Priority) {
                    lowerPrioritiySource = audioSource;
                    break;
                }
            }

            if (lowerPrioritiySource != null) {
                OpenALSource newALSource = activeSources[lowerPrioritiySource];
                RemoveActiveSource(lowerPrioritiySource);

                return AssignALSourceToAudioSource(newALSource, audioSource);
            }

            return null;
        }

        internal static OpenALSource GetActiveSource(AudioSource audioSource) {
            if (activeSources.ContainsKey(audioSource))
                return activeSources[audioSource];

            return null;
        }


        private static void ReclaimInactiveSources() {
            FindUnactiveSources();
            ReturnUnactiveSourcesToPool();
        }


        private static void FindUnactiveSources() {
            foreach (var pairs in activeSources) {
                if (SourceIsntPlayingAnything(pairs.Value)) {
                    unactiveList.Add(pairs.Key);
                }
            }
        }

        private static void ReturnUnactiveSourcesToPool() {
            foreach (AudioSource source in unactiveList) {
                ReturnToPool(source);
            }

            unactiveList.Clear();
        }

        private static void ReturnToPool(AudioSource connectedSource) {
            OpenALSource source = activeSources[connectedSource];

            RemoveActiveSource(connectedSource);

            source.StopAndUnqueueAllBuffers();

            allAvailableOpenALSources.Push(source);
        }

        private static void RemoveActiveSource(AudioSource connectedSource) {
            //Console.WriteLine("Source [" + connectedSource.SourceID + "] no longer active");

            activeSources.Remove(connectedSource);
        }

        private static OpenALSource AssignALSourceToAudioSource(OpenALSource alSource, AudioSource audioSource) {
            //Console.WriteLine("Made source [" + audioSource.SourceID + "] active");

            activeSources[audioSource] = alSource;
            alSource.StopAndUnqueueAllBuffers();

            return alSource;
        }


        private static bool SourceIsntPlayingAnything(OpenALSource source) {
            return source.GetSourceState() != AudioSourceState.Playing;
        }


        private static void CreateAllOpenALSources() {
            OpenALSource source = null;
            while (allAvailableOpenALSources.Count < MAX_AUDIO_SOURCES && 
                (source = OpenALSource.CreateOpenALSource()) != null) {
                allAvailableOpenALSources.Push(source);
            }
        }

        private static void DisposeAll() {
            foreach (OpenALSource alSource in allAvailableOpenALSources) {
                alSource.Dispose();
            }
            allAvailableOpenALSources.Clear();
        }

        private static void ReclaimAll() {
            foreach (var items in activeSources) {
                ReturnToPool(items.Key);
            }
        }
    }
}

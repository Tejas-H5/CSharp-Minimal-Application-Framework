using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Audio
{
    internal static class ALAudioSourcePool
    {
        const int MAX_AUDIO_SOURCES = 256;
        private static Stack<OpenALSource> _allAvailableOpenALSources = new Stack<OpenALSource>();

        private static Dictionary<AudioSource, OpenALSource> _activeSources = new Dictionary<AudioSource, ALSourceAOpenALSourceudioSourcePair>();
        private static List<AudioSource> _unactiveList = new List<AudioSource>();

        internal static void Init()
        {
            CreateAllOpenALSources();
        }

        internal static void Update()
        {
            ReclaimUnactiveSources();
        }

        internal static void Cleanup()
        {
            DisposeAllOpenALSources();
        }


        internal static OpenALSource AcquireSource(AudioSource audioSource)
        {
            OpenALSource active = GetActiveSource(audioSource);
            if (active != null)
                return active;

            if (PooledSourcesAreAvailable())
            {
                OpenALSource newALSource = _allAvailableOpenALSources.Pop();
                return AssignALSourceToAudioSource(newALSource, audioSource);
            }


            AudioSource lowerPrioritiySource = null;
            foreach (var pairs in _activeSources)
            {
                bool isLowerPriority = pairs.Key.Priority < audioSource.Priority;
                if (isLowerPriority)
                {
                    lowerPrioritiySource = pairs.Key;
                    break;
                }
            }

            if (lowerPrioritiySource != null)
            {
                OpenALSource newALSource = _activeSources[lowerPrioritiySource];
                _activeSources.Remove(lowerPrioritiySource);

                return AssignALSourceToAudioSource(newALSource, audioSource);
            }

            return null;
        }

        internal static OpenALSource GetActiveSource(AudioSource audioSource)
        {
            if (SourceAlreadyActive(audioSource))
                return _activeSources[audioSource];

            return null;
        }


        private static bool SourceAlreadyActive(AudioSource source)
        {
            return _activeSources.ContainsKey(source);
        }


        private static void ReclaimUnactiveSources()
        {
            FindUnactiveSources();
            ReturnUnactiveSourcesToPool();
        }


        private static void FindUnactiveSources()
        {
            foreach (var pairs in _activeSources)
            {
                if (SourceIsntPlayingAnything(pairs.Value.OpenAlAudioSource))
                {
                    _unactiveList.Add(pairs.Key);
                }
            }
        }

        private static void ReturnUnactiveSourcesToPool()
        {
            foreach (int id in _unactiveList)
            {
                ReturnToPool(id);
            }

            _unactiveList.Clear();
        }

        private static void ReturnToPool(int id)
        {
            OpenALSource source = _activeSources[id].OpenAlAudioSource;

            _activeSources.Remove(id);

            source.StopAndUnqueueAllBuffers();

            _allAvailableOpenALSources.Push(source);
        }


        private static OpenALSource AssignALSourceToAudioSource(OpenALSource alSource, AudioSource audioSource)
        {
            _activeSources[audioSource] = alSource;

            alSource.StopAndUnqueueAllBuffers();

            return alSource;
        }


        private static bool SourceIsntPlayingAnything(OpenALSource source)
        {
            return source.GetSourceState() != AudioSourceState.Playing;
        }


        private static void CreateAllOpenALSources()
        {
            OpenALSource source = null;
            while (SourceLimitNotReached() && (source = OpenALSource.CreateOpenALSource()) != null)
            {
                _allAvailableOpenALSources.Push(source);
            }
        }

        private static void DisposeAllOpenALSources()
        {
            foreach(var items in _activeSources)
            {
                _unactiveList.Add(items.Key);
            }

            ReclaimUnactiveSources();

            foreach(OpenALSource alSource in _allAvailableOpenALSources)
            {
                alSource.Dispose();
            }
        }

        private static bool SourceLimitNotReached()
        {
            return _allAvailableOpenALSources.Count < MAX_AUDIO_SOURCES;
        }

        internal static bool PooledSourcesAreAvailable()
        {
            return _allAvailableOpenALSources.Count > 0;
        }
    }
}

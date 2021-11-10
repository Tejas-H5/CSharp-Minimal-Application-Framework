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

        private static Dictionary<AudioSource, OpenALSource> _activeSources = new Dictionary<AudioSource, OpenALSource>();
        private static List<AudioSource> _unactiveList = new List<AudioSource>();

        internal static void Init()
        {
            CreateAllOpenALSources();
        }

        internal static void Update()
        {
            SendAudioSourceDataToALSources();

            ReclaimUnactiveSources();
        }

        private static void SendAudioSourceDataToALSources()
        {
            foreach(var items in _activeSources)
            {
                AudioSource virtualSource = items.Key;
                OpenALSource alSource = items.Value;
                alSource.PullDataFrom(virtualSource);
            }
        }

        internal static void Cleanup()
        {
            DisposeAllOpenALSources();
        }

        private static void DisposeAllOpenALSources()
        {
            ReclaimAllSources();
            DisposeAllPooledSources();
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
                RemoveActiveSource(lowerPrioritiySource);

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
                if (SourceIsntPlayingAnything(pairs.Value))
                {
                    _unactiveList.Add(pairs.Key);
                }
            }
        }

        private static void ReturnUnactiveSourcesToPool()
        {
            foreach (AudioSource source in _unactiveList)
            {
                ReturnToPool(source);
            }

            _unactiveList.Clear();
        }

        private static void ReturnToPool(AudioSource connectedSource)
        {
            OpenALSource source = _activeSources[connectedSource];

            RemoveActiveSource(connectedSource);

            source.StopAndUnqueueAllBuffers();

            _allAvailableOpenALSources.Push(source);
        }

        private static void RemoveActiveSource(AudioSource connectedSource)
        {
            //Console.WriteLine("Source [" + connectedSource.SourceID + "] no longer active");

            _activeSources.Remove(connectedSource);
        }

        private static OpenALSource AssignALSourceToAudioSource(OpenALSource alSource, AudioSource audioSource)
        {
            //Console.WriteLine("Made source [" + audioSource.SourceID + "] active");

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

        private static void DisposeAllPooledSources()
        {
            foreach (OpenALSource alSource in _allAvailableOpenALSources)
            {
                alSource.Dispose();
            }
            _allAvailableOpenALSources.Clear();
        }

        private static void ReclaimAllSources()
        {
            foreach (var items in _activeSources)
            {
                ReturnToPool(items.Key);
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

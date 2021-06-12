using MinimalAF.ResourceManagement;

namespace MinimalAF.Audio
{
    public static class AudioMap
    {
        public static AudioData RegisterAudioData(string name, string path, AudioDataImportSettings settings)
        {
            ResourceMap<AudioData>.RegisterResource(name, path, settings, AudioData.FromFile);
            return GetAudioData(name);
        }

        public static AudioData GetAudioData(string name)
        {
            return ResourceMap<AudioData>.GetCached(name);
        }

        public static void UnloadAllCachedAudio()
        {
            ResourceMap<AudioData>.UnloadResources();
        }

        public static void UnloadAudio(string name)
        {
            ResourceMap<AudioData>.UnloadResource(name);
        }
    }
}

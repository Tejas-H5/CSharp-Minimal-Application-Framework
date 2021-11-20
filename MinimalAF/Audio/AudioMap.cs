using MinimalAF.ResourceManagement;

namespace MinimalAF.Audio
{
	public static class AudioMap
    {
        public static AudioData LoadAudio(string name, string path, AudioDataImportSettings settings)
        {
            ResourceMap<AudioData>.LoadResource(name, path, settings, AudioData.FromFile);
            return GetAudio(name);
        }

        public static AudioData GetAudio(string name)
        {
            return ResourceMap<AudioData>.GetResource(name);
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

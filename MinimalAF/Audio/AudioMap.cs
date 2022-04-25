using MinimalAF.ResourceManagement;

namespace MinimalAF.Audio {
    public static class AudioMap {
        public static AudioData Load(string name, string path, AudioDataImportSettings settings) {
            AudioData data = ResourceMap<AudioData>.Get(name);
            if(name == null) {
                data = AudioData.FromFile(path, settings);
                ResourceMap<AudioData>.Put(name, data);
            }

            return data;
        }

        public static AudioData GetAudio(string name) {
            return ResourceMap<AudioData>.Get(name);
        }

        public static void UnloadAll() {
            ResourceMap<AudioData>.UnloadAll();
        }

        public static void UnloadAudio(string name) {
            ResourceMap<AudioData>.Delete(name);
        }
    }
}

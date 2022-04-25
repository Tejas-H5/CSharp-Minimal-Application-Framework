using MinimalAF.ResourceManagement;

namespace MinimalAF.Rendering {
    public static class TextureMap {
        public static Texture Load(string name, string path, TextureImportSettings settings = null) {
            if (ResourceMap<Texture>.Has(name)) {
                return ResourceMap<Texture>.Get(name);
            }


            Texture t = Texture.LoadFromFile(path, settings);
            ResourceMap<Texture>.Put(name, t);
            return t;
        }

        //TODO: return a pink texture or similar
        public static Texture Get(string name) {
            return ResourceMap<Texture>.Get(name);
        }

        public static void UnloadAll() {
            ResourceMap<Texture>.UnloadAll();
        }

        public static void Unload(string name) {
            ResourceMap<Texture>.Delete(name);
        }
    }
}

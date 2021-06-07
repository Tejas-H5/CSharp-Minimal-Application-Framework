using MinimalAF.ResourceManagement;

namespace MinimalAF.Rendering
{
    public static class TextureMap
    {
        public static void RegisterTexture(string name, string path, TextureImportSettings settings)
        {
            ResourceMap<Texture>.RegisterResource(name, path, settings, Texture.LoadFromFile);
        }

        //TODO: return a pink texture or similar
        public static Texture GetTexture(string name)
        {
            return ResourceMap<Texture>.GetCached(name);
        }

        public static void UnloadTextures()
        {
            ResourceMap<Texture>.UnloadResources();
        }

        public static void UnloadTexture(string name)
        {
            ResourceMap<Texture>.UnloadResource(name);
        }
    }
}

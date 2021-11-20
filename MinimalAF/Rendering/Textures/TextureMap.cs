using MinimalAF.ResourceManagement;

namespace MinimalAF.Rendering
{
	public static class TextureMap
    {
        public static Texture LoadTexture(string name, string path, TextureImportSettings settings)
        {
            ResourceMap<Texture>.LoadResource(name, path, settings, Texture.LoadFromFile);
            return GetTexture(name);
        }

        //TODO: return a pink texture or similar
        public static Texture GetTexture(string name)
        {
            return ResourceMap<Texture>.GetResource(name);
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

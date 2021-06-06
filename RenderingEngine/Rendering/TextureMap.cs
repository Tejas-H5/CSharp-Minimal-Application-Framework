using RenderingEngine.ResourceManagement;
using System.Collections.Generic;

namespace RenderingEngine.Rendering
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

        //TODO: implement this if it is ever needed
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

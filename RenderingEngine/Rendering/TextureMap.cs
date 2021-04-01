using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Rendering
{
    public static class TextureMap
    {
        private static Dictionary<string, Texture> _textureMap = new Dictionary<string, Texture>();

        public static void RegisterTexture(string name, string path, TextureImportSettings settings)
        {
            if (_textureMap.ContainsKey(name))
                return;

            var tex = Texture.LoadFromFile("./Res/" + path, settings);
            if (tex != null)
                _textureMap[name] = tex;
        }

        //TODO: return a pink texture or similar
        public static Texture GetTexture(string name)
        {
            if (!_textureMap.ContainsKey(name))
                return null;

            return _textureMap[name];
        }

        //TODO: implement this if it is ever needed
        public static void UnloadTextures()
        {
            foreach(var item in _textureMap)
            {
                item.Value.Dispose();
            }

            _textureMap.Clear();
        }
    }
}

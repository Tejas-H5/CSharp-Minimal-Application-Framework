using OpenTK.Graphics.OpenGL;

namespace MinimalAF.Rendering {
    public enum FilteringType {
        NearestNeighbour,
        Bilinear
    }

    public enum ClampingType {
        Repeat = TextureWrapMode.Repeat,
        ClampToEdge = TextureWrapMode.ClampToEdge
    }


    public class TextureImportSettings {
        public FilteringType Filtering = FilteringType.Bilinear;
        public ClampingType Clamping = ClampingType.Repeat;

        internal PixelInternalFormat InternalFormat = PixelInternalFormat.Rgba;
        internal PixelFormat PixelFormatType = PixelFormat.Bgra;

        public TextureMinFilter GetGLMinFilter() {
            switch (Filtering) {
                case FilteringType.NearestNeighbour:
                    return TextureMinFilter.Nearest;
                default:
                    return TextureMinFilter.Linear;
            }
        }

        public TextureMagFilter GetGLMagFilter() {
            switch (Filtering) {
                case FilteringType.NearestNeighbour:
                    return TextureMagFilter.Nearest;
                default:
                    return TextureMagFilter.Linear;
            }
        }
    }
}
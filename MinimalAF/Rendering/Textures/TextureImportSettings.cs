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
    }
}
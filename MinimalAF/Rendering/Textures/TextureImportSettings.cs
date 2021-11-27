using Silk.NET.OpenGL;

namespace MinimalAF.Rendering
{
	public enum FilteringType
    {
        NearestNeighbour,
        Bilinear
    }

    public enum ClampingType
    {
        Repeat = TextureWrapMode.Repeat,
        ClampToEdge = TextureWrapMode.ClampToEdge
    }


    public class TextureImportSettings
    {
        public FilteringType Filtering = FilteringType.Bilinear;
        public ClampingType Clamping = ClampingType.Repeat;

        internal InternalFormat InternalFormat = InternalFormat.Rgba;
        internal PixelFormat PixelFormatType = PixelFormat.Bgra;
    }
}
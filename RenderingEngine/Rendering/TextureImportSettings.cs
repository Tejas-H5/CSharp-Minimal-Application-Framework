namespace RenderingEngine.Rendering
{
    public enum FilteringType
    {
        NearestNeighbour,
        Bilinear
    }


    public class TextureImportSettings
    {
        public FilteringType Filtering = FilteringType.Bilinear;
    }
}
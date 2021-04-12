namespace RenderingEngine.Rendering.ImmediateMode
{
    public interface IGeometryOutput
    {
        uint AddVertex(Vertex v);
        void MakeTriangle(uint v1, uint v2, uint v3);
        void Flush();
        bool FlushIfRequired(int numIncomingVerts, int numIncomingIndices);
        uint CurrentV();
        uint CurrentI();
    }
}

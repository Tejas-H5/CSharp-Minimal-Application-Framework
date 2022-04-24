namespace MinimalAF.Rendering.ImmediateMode {
    /// <summary>
    ///  This is the interface used to output geometry to the screen.
    /// </summary>
    public interface IGeometryOutput {
        uint AddVertex(Vertex v);
        void MakeTriangle(uint v1, uint v2, uint v3);

        void Flush();

        /// <summary>
        /// Return true if this function called Flush()
        /// </summary>
        bool FlushIfRequired(int numIncomingVerts, int numIncomingIndices);
        uint CurrentV();
        uint CurrentI();
    }
}

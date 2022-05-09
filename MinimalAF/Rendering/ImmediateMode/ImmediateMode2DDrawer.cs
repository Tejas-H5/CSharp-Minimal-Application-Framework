namespace MinimalAF.Rendering {
    public class ImmediateMode2DDrawer<V> where V : struct, IVertexUV, IVertexPosition {
        public readonly TriangleDrawer<V> Triangle;
        public readonly QuadDrawer<V> Quad;
        public readonly RectangleDrawer<V> Rect;
        public readonly NGonDrawer<V> NGon;
        public readonly PolyLineDrawer<V> NLine;
        public readonly ArcDrawer<V> Arc;
        public readonly CircleDrawer<V> Circle;
        public readonly LineDrawer<V> Line;
        public readonly MeshOutputStream<V> MeshOutputStream;

        public ImmediateMode2DDrawer(IGeometryOutput<V> meshOutputStream) {
            Triangle = new TriangleDrawer<V>(meshOutputStream, this);
            NGon = new NGonDrawer<V>(meshOutputStream);
            Quad = new QuadDrawer<V>(meshOutputStream);
            NLine = new PolyLineDrawer<V>(meshOutputStream, this);

            Line = new LineDrawer<V>(this);
            Arc = new ArcDrawer<V>(circleEdgeLength: 5, maxCircleEdgeCount: 32, this);
            Rect = new RectangleDrawer<V>(this);
            Circle = new CircleDrawer<V>(this);
        }

        internal static V CreateVertex(float x, float y, float u, float v) {
            V vert = new V();
            vert.Position = new OpenTK.Mathematics.Vector3(x, y, 0);
            vert.UV = new OpenTK.Mathematics.Vector2(u, v);

            return vert;
        }
    }
}

using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MinimalAF {
    public class IM : ImmediateModeHelpers<Vertex> {}

    public class MeshOutput : Mesh<Vertex> {
        public MeshOutput(int vertexCount, int triangleCount, bool stream, bool allowResizing) 
            : base(vertexCount, triangleCount, stream, allowResizing) {}
    }
}


namespace MinimalAF.Rendering {
    /// <summary>
    ///  This is the interface used to output geometry to the screen.
    /// </summary>
    public interface IGeometryOutput<V> where V : struct {
        uint AddVertex(V v);
        void MakeTriangle(uint v1, uint v2, uint v3);

        public bool FlushIfRequired(int incomingVertexCount, int incomingIndexConut);
    }


    public interface IVertexCreator2D<V> where V: struct {
        public V CreateVertex(float x, float y, float u, float v);
    }


    /// <summary>
    /// Required to hold the state required to draw an NLine in an immedaite mode style.
    /// V is a type of vertex.
    /// </summary>
    public struct NLineDrawer<V, Out> where V : struct where Out : IGeometryOutput<V> {
        public NLineDrawer(){}

        public V FirstVertex = default(V);
        public V SecondVertex = default(V);
        public uint FirstVertexIndex = 0;
        public uint SecondVertexIndex = 0;
        public bool Started = false;

        public Out OutputStream = default(Out);

        /// <summary>
        /// Start drawing the NLine, or extend the NLine. 
        /// Because different usecases require different line tangents, I have deffered this responsibility to you.
        /// Therefore, for each 'point' in the line, you will actually be inputting 2 vertices, which will be bridged with the two previous vertices to 
        /// make a quad. This method should really be called something like `DrawQuadStrip`.
        /// 
        /// Note: Ideally this should be drawn entirely in 1 go. It is unclear what will happen
        /// when you have two NLines and you are interlacing calls to this method
        /// </summary>
        public void ExtendNLine(V v1, V v2) {
            if (!Started) {
                FirstVertex = v1;
                SecondVertex = v2;
                FirstVertexIndex = OutputStream.AddVertex(v1);
                SecondVertexIndex = OutputStream.AddVertex(v2);
                Started = true;
                return;
            }

            uint nextLast1Index = OutputStream.AddVertex(v1);
            uint nextLast2Index = OutputStream.AddVertex(v2);

            if (OutputStream.FlushIfRequired(2, 6)) {
                // the last two vertex just got flushed, so we need to add them back.
                FirstVertexIndex = OutputStream.AddVertex(FirstVertex);
                SecondVertexIndex = OutputStream.AddVertex(SecondVertex);
            }

            OutputStream.MakeTriangle(FirstVertexIndex, SecondVertexIndex, nextLast2Index);
            OutputStream.MakeTriangle(nextLast2Index, nextLast1Index, FirstVertexIndex);

            FirstVertex = v1;
            SecondVertex = v2;
            FirstVertexIndex = nextLast1Index;
            SecondVertexIndex = nextLast2Index;
        }
    }


    /// <summary>
    /// Required to hold the state required to draw an NGon in an immedaite mode style.
    /// V is a type of vertex
    /// </summary>
    public struct NGonDrawer<V, Out> where V : struct where Out : IGeometryOutput<V> {
        public NGonDrawer() {}

        public uint PolygonFirst = 0;
        public uint PolygonSecond = 0;
        public V FirstVertex = default(V);
        public V SecondVertex = default(V);
        public uint FirstVertexIndex = 0;
        public uint SecondVertexIndex = 0;

        public Out OutputStream = default(Out);

        // we need at least 2 vertices to start creating triangles with NGonContinue.
        // TODO: 
        public void DrawNGonBegin(V v1, V v2) {
            PolygonFirst = OutputStream.AddVertex(v1);
            FirstVertex = v1;

            PolygonSecond = OutputStream.AddVertex(v2);
            SecondVertex = v2;
        }

        public void DrawNGonExtend(V v) {
            if (OutputStream.FlushIfRequired(1, 3)) {
                // the first and second verts got flushed, so we need to re-add them
                PolygonFirst = OutputStream.AddVertex(FirstVertex);
                PolygonSecond = OutputStream.AddVertex(SecondVertex);
            }

            uint polygonThird = OutputStream.AddVertex(v);
            OutputStream.MakeTriangle(PolygonFirst, PolygonSecond, polygonThird);

            PolygonSecond = polygonThird;
            SecondVertex = v;
        }
    }


    /// <summary>
    /// Short for ImmediateMode, it's been abbreviated because we will be using it ALL the time.
    /// 
    /// Has a couple of helper functions used to append vertices and indices onto geometry ouput streams,
    /// mainly buffered mesh outptus like the one in FrameworkContext and MeshBuffers, but you should
    /// be able to make your own IGeometryOutput
    /// </summary>
    public class ImmediateModeHelpers<V> where V : struct {
        static IVertexCreator2D<V> _vertexCreator;
        public static IVertexCreator2D<V> VertexCreator2D {
            get {
                #if DEBUG
                if (_vertexCreator == null) {
                    throw new Exception("You need to set IM<V>.VertexCreator before calling this function for this to work.");
                }
                #endif

                return _vertexCreator;
            }
            set {
                _vertexCreator = value;
            }
        }

        

        public void DrawTriangle<Out>(Out outputStream, V v1, V v2, V v3) where Out : IGeometryOutput<V> {
            outputStream.FlushIfRequired(3, 3);

            uint i1 = outputStream.AddVertex(v1);
            uint i2 = outputStream.AddVertex(v2);
            uint i3 = outputStream.AddVertex(v3);

            outputStream.MakeTriangle(i1, i2, i3);
        }


        // Basically, I don't want people directly referencing our Vertex class very often, and
        // this is how I am doing that

        public static NLineDrawer<V, Out> NewNLineDrawer<Out>(Out outputStream) where Out : IGeometryOutput<V> {
            return new NLineDrawer<V, Out> { OutputStream = outputStream };
        }

        public static NGonDrawer<V, Out> NewNGonDrawer<Out>(Out outputStream) where Out : IGeometryOutput<V> {
            return new NGonDrawer<V, Out> { OutputStream = outputStream };
        }

        public void DrawTriangle<Out>(
            Out outputStream,
            float x0, float y0, float x1, float y1, float x2, float y2,
            float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.5f, float v1 = 1f, float u2 = 1, float v2 = 0
        ) where Out : IGeometryOutput<V> {
            V vertex1 = VertexCreator2D.CreateVertex(x0, y0, u0, v0);
            V vertex2 = VertexCreator2D.CreateVertex(x1, y1, u1, v1);
            V vertex3 = VertexCreator2D.CreateVertex(x2, y2, u2, v2);

            DrawTriangle(outputStream, vertex1, vertex2, vertex3);
        }

        public void DrawTriangleOutline<Out>(Out outputStream, float thickness, float x0, float y0, float x1, float y1, float x2, float y2) where Out : IGeometryOutput<V> {
            var v0Inner = VertexCreator2D.CreateVertex(x0, y0, x0, y0);
            var v1Inner = VertexCreator2D.CreateVertex(x1, y1, x1, y1);
            var v2Inner = VertexCreator2D.CreateVertex(x2, y2, x2, y2);

            var v0 = new Vector2(x0, y0);
            var v1 = new Vector2(x1, y1);
            var v2 = new Vector2(x2, y2);

            var mean = (v0 + v1 + v2) / 3.0f;
            var meanToV0 = (v0 - mean).Normalized();
            var meanToV1 = (v1 - mean).Normalized();
            var meanToV2 = (v2 - mean).Normalized();
            var v00 = v0 + meanToV0 * thickness;
            var v11 = v1 + meanToV1 * thickness;
            var v22 = v2 + meanToV2 * thickness;

            var v0Outer = VertexCreator2D.CreateVertex(v00.X, v00.Y, v00.X, v00.Y);
            var v1Outer = VertexCreator2D.CreateVertex(v11.X, v11.Y, v11.X, v11.Y);
            var v2Outer = VertexCreator2D.CreateVertex(v22.X, v22.Y, v22.X, v22.Y);

            var nLineDrawer = NewNLineDrawer(outputStream);
            nLineDrawer.ExtendNLine(v0Inner, v0Outer);
            nLineDrawer.ExtendNLine(v1Inner, v1Outer);
            nLineDrawer.ExtendNLine(v2Inner, v2Outer);
            nLineDrawer.ExtendNLine(v0Inner, v0Outer);
        }

        /// <summary>
        /// Assumes the verts are defined clockwise
        /// </summary>
        public static void DrawQuad<Out>(Out outputStream, V v1, V v2, V v3, V v4) where Out : IGeometryOutput<V> {
            outputStream.FlushIfRequired(4, 6);

            uint i1 = outputStream.AddVertex(v1);
            uint i2 = outputStream.AddVertex(v2);
            uint i3 = outputStream.AddVertex(v3);
            uint i4 = outputStream.AddVertex(v4);

            outputStream.MakeTriangle(i1, i2, i3);
            outputStream.MakeTriangle(i3, i4, i1);
        }

        public static void DrawRect<Out>(Out outputStream, float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 0, float u1 = 1, float v1 = 1) where Out : IGeometryOutput<V> {
            var vert0 = VertexCreator2D.CreateVertex(x0, y0, u0, v1);
            var vert1 = VertexCreator2D.CreateVertex(x1, y0, u1, v1);
            var vert2 = VertexCreator2D.CreateVertex(x1, y1, u1, v0);
            var vert3 = VertexCreator2D.CreateVertex(x0, y1, u0, v0);

            DrawQuad(outputStream, vert0, vert1, vert2, vert3);
        }

        public static void DrawRect<Out>(Out outputStream, Rect rect, Rect uvs) where Out : IGeometryOutput<V> {
            DrawRect(outputStream, rect.X0, rect.Y0, rect.X1, rect.Y1, uvs.X0, uvs.Y0, uvs.X1, uvs.Y1);
        }

        public static void DrawRect<Out>(Out outputStream, Rect rect) where Out : IGeometryOutput<V> {
            DrawRect(outputStream, rect.X0, rect.Y0, rect.X1, rect.Y1);
        }

        public static void DrawRectOutline<Out>(Out outputStream, float thickness, Rect rect) where Out : IGeometryOutput<V> {
            DrawRectOutline(outputStream, thickness, rect.X0, rect.Y0, rect.X1, rect.Y1);
        }

        public static void DrawRectOutline<Out>(Out outputStream, float thickness, float x0, float y0, float x1, float y1) where Out : IGeometryOutput<V> {
            DrawRect(outputStream, x0 - thickness, y0 - thickness, x1, y0);
            DrawRect(outputStream, x0, y1, x1 + thickness, y1 + thickness);

            DrawRect(outputStream, x0 - thickness, y0, x0, y1 + thickness);
            DrawRect(outputStream, x1, y0 - thickness, x1 + thickness, y1);
        }

        public static void DrawArc<Out>(Out outputStream, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int maxCircleEdgeCount = 32, float circleEdgeLength = 1) where Out : IGeometryOutput<V> {
            int edgeCount = GetEdgeCount(radius, startAngle, endAngle, maxCircleEdgeCount, circleEdgeLength);

            DrawArc(outputStream, xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        private static int GetEdgeCount(float radius, float startAngle, float endAngle, int maxCircleEdgeCount, float circleEdgeLength) {
            float deltaAngle = circleEdgeLength / radius;
            int edgeCount = (int)((endAngle - startAngle) / deltaAngle) + 1;

            if (edgeCount > maxCircleEdgeCount) {
                edgeCount = maxCircleEdgeCount;
            }

            return edgeCount;
        }

        private static void DrawArc<Out>(Out outputStream, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) where Out : IGeometryOutput<V> {
            if (edgeCount < 0)
                return;

            float deltaAngle = (endAngle - startAngle) / edgeCount;

            var vCenter = VertexCreator2D.CreateVertex(xCenter, yCenter, 0, 0);

            bool first = true;
            var ngonDrawer = NewNGonDrawer(outputStream);
            for (float angle = endAngle; angle > startAngle - deltaAngle + 0.001f; angle -= deltaAngle) {
                float X = xCenter + radius * MathF.Sin(angle);
                float Y = yCenter + radius * MathF.Cos(angle);
                var v = _vertexCreator.CreateVertex(X, Y, X / radius, Y / radius);

                if (first == true) {
                    first = false;
                    ngonDrawer.DrawNGonBegin(vCenter, v);
                } else {
                    ngonDrawer.DrawNGonExtend(v);
                }
            }
        }

        public static void DrawArcOutline<Out>(Out outputStream, float thickness, float x0, float y0, float r, float startAngle, float endAngle, int maxCircleEdgeCount = 32, float circleEdgeLength = 1) where Out : IGeometryOutput<V> {
            int edges = GetEdgeCount(r, startAngle, endAngle, maxCircleEdgeCount, circleEdgeLength);
            DrawArcOutline(outputStream, thickness, x0, y0, r, startAngle, endAngle, edges);
        }

        public static void DrawArcOutline<Out>(Out outputStream, float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) where Out : IGeometryOutput<V> {
            if (edgeCount < 0)
                return;

            float deltaAngle = (endAngle - startAngle) / edgeCount;

            var nLineDrawer = NewNLineDrawer(outputStream);
            for (float angle = startAngle; angle < endAngle + deltaAngle - 0.001f; angle += deltaAngle) {
                float sinAngle = MathF.Sin(angle);
                float cosAngle = MathF.Cos(angle);
                float X1 = xCenter + radius * sinAngle;
                float Y1 = yCenter + radius * cosAngle;

                float X2 = xCenter + (radius + thickness) * sinAngle;
                float Y2 = yCenter + (radius + thickness) * cosAngle;

                var v1 = VertexCreator2D.CreateVertex(X1, Y1, sinAngle, cosAngle);
                var v2 = VertexCreator2D.CreateVertex(X2, Y2, sinAngle, cosAngle);

                nLineDrawer.ExtendNLine(v1, v2);
            }
        }

        public static void DrawLine<Out>(Out outputStream, float x0, float y0, float x1, float y1, float thickness, CapType cap = CapType.None) where Out : IGeometryOutput<V> {
            thickness /= 2;

            float dirX = x1 - x0;
            float dirY = y1 - y0;
            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            float perpX = -thickness * dirY / mag;
            float perpY = thickness * dirX / mag;

            var v1 = VertexCreator2D.CreateVertex(x0 + perpX, y0 + perpY, x0 + perpX, y0 + perpY);
            var v2 = VertexCreator2D.CreateVertex(x0 - perpX, y0 - perpY, x0 - perpX, y0 - perpY);
            var v3 = VertexCreator2D.CreateVertex(x1 - perpX, y1 - perpY, x1 - perpX, y1 - perpY);
            var v4 = VertexCreator2D.CreateVertex(x1 + perpX, y1 + perpY, x1 + perpX, y1 + perpY);
            DrawQuad(outputStream, v1, v2, v3, v4);

            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            DrawCap(outputStream, x0, y0, thickness, cap, startAngle);
            DrawCap(outputStream, x1, y1, thickness, cap, startAngle + MathF.PI);
        }

        public static void DrawLineOutline<Out>(Out outputStream, float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap) where Out : IGeometryOutput<V> {
            thickness /= 2;

            float dirX = x1 - x0;
            float dirY = y1 - y0;
            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            float perpXInner = -(thickness) * dirY / mag;
            float perpYInner = (thickness) * dirX / mag;

            float perpXOuter = -(thickness + outlineThickness) * dirY / mag;
            float perpYOuter = (thickness + outlineThickness) * dirX / mag;

            // draw quad on one side of the line
            var vInner = VertexCreator2D.CreateVertex(
                x0 + perpXInner, y0 + perpYInner,
                perpXInner, perpYInner
            );
            var vOuter = VertexCreator2D.CreateVertex(
                x0 + perpXOuter, y0 + perpYOuter,
                perpXOuter, perpYOuter
            );
            var v1Inner = VertexCreator2D.CreateVertex(
                x1 + perpXInner, y1 + perpYInner,
                perpXInner, perpYInner
            );
            var v1Outer = VertexCreator2D.CreateVertex(
                x1 + perpXOuter, y1 + perpYOuter,
                perpXOuter, perpYOuter
            );
            DrawQuad(outputStream, vInner, vOuter, v1Outer, v1Inner);

            // draw quad on other side of the line
            vInner = VertexCreator2D.CreateVertex(
                x0 - perpXInner, y0 - perpYInner,
                -perpXInner, -perpYInner
            );
            vOuter = VertexCreator2D.CreateVertex(
                x0 - perpXOuter, y0 - perpYOuter,
                perpXOuter, perpYOuter
            );
            v1Inner = VertexCreator2D.CreateVertex(
                x1 - perpXInner, y1 - perpYInner,
                -perpXInner, -perpYInner
            );
            v1Outer = VertexCreator2D.CreateVertex(
                x1 - perpXOuter, y1 - perpYOuter,
                -perpXOuter, -perpYOuter
            );
            DrawQuad(outputStream, vInner, vOuter, v1Outer, v1Inner);

            // Draw both caps
            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            DrawCapOutline(outputStream, outlineThickness, x0, y0, thickness, cap, startAngle);
            DrawCapOutline(outputStream, outlineThickness, x1, y1, thickness, cap, startAngle + MathF.PI);
        }


        public static void DrawCap<Out>(Out outputStream, float x0, float y0, float radius, CapType cap, float startAngle) where Out : IGeometryOutput<V> {
            switch (cap) {
                case CapType.Circle:
                    DrawCircleCap(outputStream, x0, y0, radius, startAngle);
                    break;
                default:
                    break;
            }
        }


        public static void DrawCircleCap<Out>(Out outputStream, float x0, float y0, float thickness, float angle) where Out : IGeometryOutput<V> {
            DrawArc(outputStream, x0, y0, thickness, angle, angle + MathF.PI);
        }

        public static void DrawCapOutline<Out>(Out outputStream, float outlineThickness, float x0, float y0, float radius, CapType cap, float startAngle) where Out : IGeometryOutput<V> {
            switch (cap) {
                case CapType.Circle:
                    DrawCircleCapOutline(outputStream, outlineThickness, x0, y0, radius, startAngle);
                    break;
                default:
                    DrawDefaultCapOutline(outputStream, outlineThickness, x0, y0, radius, startAngle);
                    break;
            }
        }

        public static void DrawDefaultCapOutline<Out>(Out outputStream, float thickness, float x0, float y0, float radius, float angle) where Out : IGeometryOutput<V> {
            DrawArcOutline(outputStream, thickness, x0, y0, radius, angle, angle + MathF.PI, 1);
        }

        public static void DrawCircleCapOutline<Out>(Out outputStream, float thickness, float x0, float y0, float radius, float angle) where Out : IGeometryOutput<V> {
            DrawArcOutline(outputStream, thickness, x0, y0, radius, angle, angle + MathF.PI);
        }

        public static void DrawCircle<Out>(Out outputStream, float x0, float y0, float r, int edges) where Out : IGeometryOutput<V> {
            DrawArc(outputStream, x0, y0, r, 0, MathF.PI * 2, edges);
        }

        public static void DrawCircle<Out>(Out outputStream, float x0, float y0, float r) where Out : IGeometryOutput<V> {
            DrawArc(outputStream, x0, y0, r, 0, MathF.PI * 2);
        }

        public static void DrawCircleOutline<Out>(Out outputStream, float thickness, float x0, float y0, float r, int edges = 32) where Out : IGeometryOutput<V> {
            DrawArcOutline(outputStream, thickness, x0, y0, r, 0, MathF.PI * 2, edges);
        }
    }
}

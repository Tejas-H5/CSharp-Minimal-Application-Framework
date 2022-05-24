using OpenTK.Mathematics;
using System;

namespace MinimalAF.Rendering {
    //TODO: add support for 3D lines if needed
    public class PolyLineDrawer<V> where V : struct, IVertexUV, IVertexPosition {
        IGeometryOutput<V> geometryOutput;
        ImmediateMode2DDrawer<V> immediateModeDrawer;

        public PolyLineDrawer(IGeometryOutput<V> geometryOutput, ImmediateMode2DDrawer<V> immediateModeDrawer) {
            this.geometryOutput = geometryOutput;
            this.immediateModeDrawer = immediateModeDrawer;
        }


        bool canStart = true;
        bool canEnd = true;

        float thickness = 0;

        Vector2 lastLast;
        Vector2 last;
        Vector2 lastPerp;

        uint lastV1;
        uint lastV2;
        V lastV1Vert;
        V lastV2Vert;
        uint lastV3;
        uint lastV4;
        uint count = 0;


        CapType capType;

        //Can also be used to continue an unfinished polyline
        public void Begin(float x, float y, float thickness, CapType cap) {
            if (!canStart) {
                Continue(x, y);
                return;
            }

            this.thickness = thickness;
            lastLast = new Vector2(x, y);
            last = lastLast;

            capType = cap;
            count = 1;
        }


        public void Continue(float x, float y, bool useAverage = true) {
            CalculateLineParameters(x, y, out Vector2 dir, out Vector2 perp);
            float mag = dir.Length;

            if (mag < 0.00001f)
                return;


            if (count == 1) {
                StartLineSegment(x, y);
            } else {
                MoveLineSegmentInDirectionOf(x, y, useAverage);
            }

            lastLast = last;

            last = new Vector2(x, y);
            count++;
        }

        private void CalculateLineParameters(float x, float y, out Vector2 dir, out Vector2 perp) {
            dir = new Vector2(x, y) - last;

            float mag = dir.Length;

            perp = new Vector2();
            perp.X = -thickness * dir.Y / mag;
            perp.Y = thickness * dir.X / mag;
        }

        private void StartLineSegment(float x, float y) {
            CalculateLineParameters(x, y, out Vector2 dir, out Vector2 perp);


            V v1 = ImmediateMode2DDrawer<V>.CreateVertex(last.X + perp.X, last.Y + perp.Y, 0, 0);
            V v2 = ImmediateMode2DDrawer<V>.CreateVertex(last.X - perp.X, last.Y - perp.Y, 0, 0);

            geometryOutput.FlushIfRequired(2, 0);
            lastV1 = geometryOutput.AddVertex(v1);
            lastV2 = geometryOutput.AddVertex(v2);

            lastV1Vert = v1;
            lastV2Vert = v2;
            lastPerp = perp;


            float startAngle = MathF.Atan2(dir.X, dir.Y) + MathF.PI / 2;
            immediateModeDrawer.Line.DrawCap(last.X, last.Y, thickness, capType, startAngle);
        }

        private void MoveLineSegmentInDirectionOf(float x, float y, bool averageAngle = true) {
            CalculateLineParameters(x, y, out Vector2 dir, out Vector2 perp);

            Vector2 perpUsed;

            if (averageAngle) {
                perpUsed = (perp + lastPerp) / 2f;

                float mag = perpUsed.Length;
                perpUsed = thickness * perpUsed / mag;
            } else {
                perpUsed = perp;
            }


            V v3 = ImmediateMode2DDrawer<V>.CreateVertex(last.X + perpUsed.X, last.Y + perpUsed.Y, 0, 0);
            V v4 = ImmediateMode2DDrawer<V>.CreateVertex(last.X - perpUsed.X, last.Y - perpUsed.Y, 0, 0);

            if (geometryOutput.FlushIfRequired(4, 6)) {
                lastV1 = geometryOutput.AddVertex(lastV1Vert);
                lastV2 = geometryOutput.AddVertex(lastV2Vert);
            }


            //check if v3 and v4 intersect with v1 and v2
            var lastDir = new Vector2(-lastPerp.Y, lastPerp.X);
            var vec1 = last + perpUsed - lastLast;
            var vec2 = last - perpUsed - lastLast;

            bool v3IsArtifacting = Vector2.Dot(vec1, lastDir) > 0;
            bool v4IsArtifacting = Vector2.Dot(vec2, lastDir) > 0;

            if (v3IsArtifacting || v4IsArtifacting) {
                if (v3IsArtifacting) {
                    lastV4 = geometryOutput.AddVertex(v4);
                    geometryOutput.MakeTriangle(lastV1, lastV2, lastV4);
                    lastV2 = lastV4;
                    lastV2Vert = v4;
                } else if (v4IsArtifacting) {
                    lastV3 = geometryOutput.AddVertex(v3);
                    geometryOutput.MakeTriangle(lastV1, lastV2, lastV3);
                    lastV1 = lastV3;
                    lastV1Vert = v3;
                }
            } else {
                lastV3 = geometryOutput.AddVertex(v3);
                lastV4 = geometryOutput.AddVertex(v4);

                geometryOutput.MakeTriangle(lastV1, lastV2, lastV3);
                geometryOutput.MakeTriangle(lastV3, lastV2, lastV4);

                lastV1 = lastV3;
                lastV2 = lastV4;
                lastV1Vert = v3;
                lastV2Vert = v4;
            }

            lastPerp = perp;
        }

        public void End(float x, float y) {
            if (!canEnd) {
                Continue(x, y);
                return;
            }

            Vector2 dir = new Vector2(x, y) - last;

            float mag = dir.Length;
            if (mag < 0.001f) {
                dir = new Vector2(x, y) - lastLast;
            } 

            Continue(x, y);
            Continue(x + dir.X, y + dir.Y, false);

            last = new Vector2(x, y);

            float startAngle = MathF.Atan2(dir.X, dir.Y) + MathF.PI / 2;

            if (count == 1) {
                immediateModeDrawer.Line.DrawCap(last.X, last.Y, thickness, capType, startAngle);
            }

            immediateModeDrawer.Line.DrawCap(last.X, last.Y, thickness, capType, startAngle + MathF.PI);

            canStart = true;
        }


        /// <summary>
        /// Use very carefully.
        /// </summary>
        public void DisableEnding() {
            canEnd = false;
            canStart = false;
        }

        /// <summary>
        /// Use very carefully.
        /// </summary>
        public void EnableEnding() {
            canEnd = true;
        }
    }
}

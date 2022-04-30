using MinimalAF.Util;
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

        float lastLastX;
        float lastLastY;
        float lastX;
        float lastY;
        float lastPerpX;
        float lastPerpY;

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
            lastLastX = x;
            lastLastY = y;
            lastX = x;
            lastY = y;
            capType = cap;
            count = 1;
        }


        public void Continue(float x, float y, bool useAverage = true) {
            float dirX, dirY, perpX, perpY;
            CalculateLineParameters(x, y, out dirX, out dirY, out perpX, out perpY);

            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            if (mag < 0.00001f)
                return;


            if (count == 1) {
                StartLineSegment(x, y);
            } else {
                MoveLineSegmentInDirectionOf(x, y, useAverage);
            }

            lastLastX = lastX;
            lastLastY = lastY;

            lastX = x;
            lastY = y;
            count++;
        }

        private void CalculateLineParameters(float x, float y, out float dirX, out float dirY, out float perpX, out float perpY) {
            dirX = x - lastX;
            dirY = y - lastY;

            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            perpX = -thickness * dirY / mag;
            perpY = thickness * dirX / mag;
        }

        private void StartLineSegment(float x, float y) {
            float dirX, dirY, perpX, perpY;
            CalculateLineParameters(x, y, out dirX, out dirY, out perpX, out perpY);


            V v1 = ImmediateMode2DDrawer<V>.CreateVertex(lastX + perpX, lastY + perpY, 0, 0);
            V v2 = ImmediateMode2DDrawer<V>.CreateVertex(lastX - perpX, lastY - perpY, 0, 0);

            geometryOutput.FlushIfRequired(2, 0);
            lastV1 = geometryOutput.AddVertex(v1);
            lastV2 = geometryOutput.AddVertex(v2);

            lastV1Vert = v1;
            lastV2Vert = v2;
            lastPerpX = perpX;
            lastPerpY = perpY;


            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            immediateModeDrawer.Line.DrawCap(lastX, lastY, thickness, capType, startAngle);
        }

        private void MoveLineSegmentInDirectionOf(float x, float y, bool averageAngle = true) {
            float dirX, dirY, perpX, perpY;
            CalculateLineParameters(x, y, out dirX, out dirY, out perpX, out perpY);


            float perpUsedX, perpUsedY;

            if (averageAngle) {
                perpUsedX = (perpX + lastPerpX) / 2f;
                perpUsedY = (perpY + lastPerpY) / 2f;

                float mag = MathUtilF.Mag(perpUsedX, perpUsedY);
                perpUsedX = thickness * perpUsedX / mag;
                perpUsedY = thickness * perpUsedY / mag;
            } else {
                perpUsedX = perpX;
                perpUsedY = perpY;
            }


            V v3 = ImmediateMode2DDrawer<V>.CreateVertex(lastX + perpUsedX, lastY + perpUsedY, 0, 0);
            V v4 = ImmediateMode2DDrawer<V>.CreateVertex(lastX - perpUsedX, lastY - perpUsedY, 0, 0);

            if (geometryOutput.FlushIfRequired(4, 6)) {
                lastV1 = geometryOutput.AddVertex(lastV1Vert);
                lastV2 = geometryOutput.AddVertex(lastV2Vert);
            }


            //check if v3 and v4 intersect with v1 and v2
            float lastDirX = -lastPerpY;
            float lastDirY = lastPerpX;
            float vec1X = (lastX + perpUsedX) - lastLastX;
            float vec1Y = (lastY + perpUsedY) - lastLastY;
            float vec2X = (lastX - perpUsedX) - lastLastX;
            float vec2Y = (lastY - perpUsedY) - lastLastY;

            bool v3IsArtifacting = ((vec1X * lastDirX + vec1Y * lastDirY) > 0);
            bool v4IsArtifacting = ((vec2X * lastDirX + vec2Y * lastDirY) > 0);

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

            lastPerpX = perpX;
            lastPerpY = perpY;
        }

        public void End(float x, float y) {
            if (!canEnd) {
                Continue(x, y);
                return;
            }

            float dirX = x - lastX;
            float dirY = y - lastY;

            float mag = MathUtilF.Mag(dirX, dirY);
            if (mag < 0.001f) {
                dirX = x - lastLastX;
                dirY = y - lastLastY;
            }

            Continue(x, y);
            Continue(x + dirX, y + dirY, false);

            lastX = x;
            lastY = y;

            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;

            if (count == 1) {
                immediateModeDrawer.Line.DrawCap(lastX, lastY, thickness, capType, startAngle);
            }

            immediateModeDrawer.Line.DrawCap(lastX, lastY, thickness, capType, startAngle + MathF.PI);

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

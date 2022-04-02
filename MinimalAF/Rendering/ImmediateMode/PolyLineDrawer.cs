using MinimalAF.Util;
using System;

namespace MinimalAF.Rendering.ImmediateMode {
    //TODO: add support for 3D lines if needed
    public class PolyLineDrawer {
        IGeometryOutput _geometryOutput;

        public PolyLineDrawer(IGeometryOutput geometryOutput) {
            _geometryOutput = geometryOutput;
        }


        bool _canStart = true;
        bool _canEnd = true;

        float _thickness = 0;

        float _lastLastX;
        float _lastLastY;
        float _lastX;
        float _lastY;
        float _lastPerpX;
        float _lastPerpY;

        uint _lastV1;
        uint _lastV2;
        Vertex _lastV1Vert;
        Vertex _lastV2Vert;
        uint _lastV3;
        uint _lastV4;
        uint _count = 0;


        CapType _capType;

        //Can also be used to continue an unfinished polyline
        public void Begin(float x, float y, float thickness, CapType cap) {
            if (!_canStart) {
                Continue(x, y);
                return;
            }

            _thickness = thickness;
            _lastLastX = x;
            _lastLastY = y;
            _lastX = x;
            _lastY = y;
            _capType = cap;
            _count = 1;
        }


        public void Continue(float x, float y, bool useAverage = true) {
            float dirX, dirY, perpX, perpY;
            CalculateLineParameters(x, y, out dirX, out dirY, out perpX, out perpY);

            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            if (mag < 0.00001f)
                return;


            if (_count == 1) {
                StartLineSegment(x, y);
            } else {
                MoveLineSegmentInDirectionOf(x, y, useAverage);
            }

            _lastLastX = _lastX;
            _lastLastY = _lastY;

            _lastX = x;
            _lastY = y;
            _count++;
        }

        private void CalculateLineParameters(float x, float y, out float dirX, out float dirY, out float perpX, out float perpY) {
            dirX = x - _lastX;
            dirY = y - _lastY;

            float mag = MathF.Sqrt(dirX * dirX + dirY * dirY);

            perpX = -_thickness * dirY / mag;
            perpY = _thickness * dirX / mag;
        }

        private void StartLineSegment(float x, float y) {
            float dirX, dirY, perpX, perpY;
            CalculateLineParameters(x, y, out dirX, out dirY, out perpX, out perpY);


            Vertex v1 = new Vertex(_lastX + perpX, _lastY + perpY, 0);
            Vertex v2 = new Vertex(_lastX - perpX, _lastY - perpY, 0);

            _geometryOutput.FlushIfRequired(2, 0);
            _lastV1 = _geometryOutput.AddVertex(v1);
            _lastV2 = _geometryOutput.AddVertex(v2);

            _lastV1Vert = v1;
            _lastV2Vert = v2;
            _lastPerpX = perpX;
            _lastPerpY = perpY;


            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;
            CTX.Line.DrawCap(_lastX, _lastY, _thickness, _capType, startAngle);
        }

        private void MoveLineSegmentInDirectionOf(float x, float y, bool averageAngle = true) {
            float dirX, dirY, perpX, perpY;
            CalculateLineParameters(x, y, out dirX, out dirY, out perpX, out perpY);


            float perpUsedX, perpUsedY;

            if (averageAngle) {
                perpUsedX = (perpX + _lastPerpX) / 2f;
                perpUsedY = (perpY + _lastPerpY) / 2f;

                float mag = MathUtilF.Mag(perpUsedX, perpUsedY);
                perpUsedX = _thickness * perpUsedX / mag;
                perpUsedY = _thickness * perpUsedY / mag;
            } else {
                perpUsedX = perpX;
                perpUsedY = perpY;
            }


            Vertex v3 = new Vertex(_lastX + perpUsedX, _lastY + perpUsedY, 0);
            Vertex v4 = new Vertex(_lastX - perpUsedX, _lastY - perpUsedY, 0);

            if (_geometryOutput.FlushIfRequired(4, 6)) {
                _lastV1 = _geometryOutput.AddVertex(_lastV1Vert);
                _lastV2 = _geometryOutput.AddVertex(_lastV2Vert);
            }


            //check if v3 and v4 intersect with v1 and v2
            float lastDirX = -_lastPerpY;
            float lastDirY = _lastPerpX;
            float vec1X = (_lastX + perpUsedX) - _lastLastX;
            float vec1Y = (_lastY + perpUsedY) - _lastLastY;
            float vec2X = (_lastX - perpUsedX) - _lastLastX;
            float vec2Y = (_lastY - perpUsedY) - _lastLastY;

            bool v3IsArtifacting = ((vec1X * lastDirX + vec1Y * lastDirY) > 0);
            bool v4IsArtifacting = ((vec2X * lastDirX + vec2Y * lastDirY) > 0);

            if (v3IsArtifacting || v4IsArtifacting) {
                if (v3IsArtifacting) {
                    _lastV4 = _geometryOutput.AddVertex(v4);
                    _geometryOutput.MakeTriangle(_lastV1, _lastV2, _lastV4);
                    _lastV2 = _lastV4;
                    _lastV2Vert = v4;
                } else if (v4IsArtifacting) {
                    _lastV3 = _geometryOutput.AddVertex(v3);
                    _geometryOutput.MakeTriangle(_lastV1, _lastV2, _lastV3);
                    _lastV1 = _lastV3;
                    _lastV1Vert = v3;
                }
            } else {
                _lastV3 = _geometryOutput.AddVertex(v3);
                _lastV4 = _geometryOutput.AddVertex(v4);

                _geometryOutput.MakeTriangle(_lastV1, _lastV2, _lastV3);
                _geometryOutput.MakeTriangle(_lastV3, _lastV2, _lastV4);

                _lastV1 = _lastV3;
                _lastV2 = _lastV4;
                _lastV1Vert = v3;
                _lastV2Vert = v4;
            }

            _lastPerpX = perpX;
            _lastPerpY = perpY;
        }

        public void End(float x, float y) {
            if (!_canEnd) {
                Continue(x, y);
                return;
            }

            float dirX = x - _lastX;
            float dirY = y - _lastY;

            float mag = MathUtilF.Mag(dirX, dirY);
            if (mag < 0.001f) {
                dirX = x - _lastLastX;
                dirY = y - _lastLastY;
            }

            Continue(x, y);
            Continue(x + dirX, y + dirY, false);

            _lastX = x;
            _lastY = y;

            float startAngle = MathF.Atan2(dirX, dirY) + MathF.PI / 2;

            if (_count == 1) {
                CTX.Line.DrawCap(_lastX, _lastY, _thickness, _capType, startAngle);
            }

            CTX.Line.DrawCap(_lastX, _lastY, _thickness, _capType, startAngle + MathF.PI);

            _canStart = true;
        }


        /// <summary>
        /// Use very carefully.
        /// </summary>
        public void DisableEnding() {
            _canEnd = false;
            _canStart = false;
        }

        /// <summary>
        /// Use very carefully.
        /// </summary>
        public void EnableEnding() {
            _canEnd = true;
        }
    }
}

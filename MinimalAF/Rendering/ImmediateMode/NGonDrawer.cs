namespace MinimalAF.Rendering {
    public class NGonDrawer<V> where V : struct, IVertexUV, IVertexPosition {
        IGeometryOutput<V> outputStream;

        bool polygonBegun = false;
        uint polygonFirst;
        uint polygonSecond = 0;
        uint polygonCount = 0;
        V firstVertex;
        V secondVertex;

        public NGonDrawer(IGeometryOutput<V> outputStream) {
            this.outputStream = outputStream;
        }

        public void Begin(float centerX, float centerY, int n) {
            Begin(ImmediateMode2DDrawer<V>.CreateVertex(centerX, centerY, 0, 0), n);
        }

        public void Begin(V v1, int n) {
            if (n < 3)
                n = 3;

            polygonBegun = true;

            outputStream.FlushIfRequired(n, 3 * (n - 2));

            polygonFirst = outputStream.AddVertex(v1);
            firstVertex = v1;

            polygonCount = 1;
        }

        public void Continue(float centerX, float centerY) {
            Continue(ImmediateMode2DDrawer<V>.CreateVertex(centerX, centerY, 0, 0));
        }


        public void Continue(V v) {
            if (polygonBegun == false) {
                return;
            }

            if (polygonCount == 1) {
                AppendSecondVertexToNGon(v);
                return;
            }


            if (outputStream.FlushIfRequired(1, 3)) {
                polygonFirst = outputStream.AddVertex(firstVertex);
                polygonSecond = outputStream.AddVertex(secondVertex);
            }


            polygonCount++;

            uint polygonThird = outputStream.AddVertex(v);
            outputStream.MakeTriangle(polygonFirst, polygonSecond, polygonThird);

            polygonSecond = polygonThird;
            secondVertex = v;
        }

        private void AppendSecondVertexToNGon(V v) {
            polygonSecond = outputStream.AddVertex(v);
            secondVertex = v;

            polygonCount = 2;
        }

        public void End() {
            polygonBegun = false;
        }
    }
}

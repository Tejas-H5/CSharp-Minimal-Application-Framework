namespace MinimalAF.Rendering.ImmediateMode {
    public class NGonDrawer {
        IGeometryOutput outputStream;
        bool polygonBegun = false;
        uint polygonFirst;
        uint polygonSecond = 0;
        uint polygonCount = 0;
        Vertex firstVertex;
        Vertex secondVertex;

        public NGonDrawer(IGeometryOutput outputStream) {
            this.outputStream = outputStream;
        }

        public void Begin(float centerX, float centerY, int n) {
            Begin(new Vertex(centerX, centerY, CTX.Current2DDepth), n);
        }

        public void Begin(Vertex v1, int n) {
            if (n < 3)
                n = 3;

            polygonBegun = true;

            outputStream.FlushIfRequired(n, 3 * (n - 2));

            polygonFirst = outputStream.AddVertex(v1);
            firstVertex = v1;

            polygonCount = 1;
        }

        public void Continue(float centerX, float centerY) {
            Continue(new Vertex(centerX, centerY, CTX.Current2DDepth));
        }


        public void Continue(Vertex v) {
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

        private void AppendSecondVertexToNGon(Vertex v) {
            polygonSecond = outputStream.AddVertex(v);
            secondVertex = v;

            polygonCount = 2;
        }

        public void End() {
            polygonBegun = false;
        }
    }
}

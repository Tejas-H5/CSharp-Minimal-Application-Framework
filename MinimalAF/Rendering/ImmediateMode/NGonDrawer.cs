namespace MinimalAF.Rendering.ImmediateMode
{
	public class NGonDrawer : GeometryDrawer
    {
        IGeometryOutput _outputStream;
        bool _polygonBegun = false;
        uint _polygonFirst;
        uint _polygonSecond = 0;
        uint _polygonCount = 0;
        Vertex _firstVertex;
        Vertex _secondVertex;

        public NGonDrawer(IGeometryOutput outputStream)
        {
            _outputStream = outputStream;
        }

        public void Begin(Vertex v1, int n)
        {
            if (n < 3)
                n = 3;

            _polygonBegun = true;

            _outputStream.FlushIfRequired(n, 3 * (n - 2));

            _polygonFirst = _outputStream.AddVertex(v1);
            _firstVertex = v1;

            _polygonCount = 1;
        }

        public void Continue(Vertex v)
        {
            if (_polygonBegun == false)
            {
                return;
            }

            if (_polygonCount == 1)
            {
                AppendSecondVertexToNGon(v);
                return;
            }


            if (_outputStream.FlushIfRequired(1, 3))
            {
                _polygonFirst = _outputStream.AddVertex(_firstVertex);
                _polygonSecond = _outputStream.AddVertex(_secondVertex);
            }


            _polygonCount++;

            uint polygonThird = _outputStream.AddVertex(v);
            _outputStream.MakeTriangle(_polygonFirst, _polygonSecond, polygonThird);

            _polygonSecond = polygonThird;
            _secondVertex = v;
        }

        private void AppendSecondVertexToNGon(Vertex v)
        {
            _polygonSecond = _outputStream.AddVertex(v);
            _secondVertex = v;

            _polygonCount = 2;
        }

        public void End()
        {
            _polygonBegun = false;
        }
    }
}

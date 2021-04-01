using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Rendering.ImmediateMode
{
    class NGonDrawer
    {
        IGeometryOutput _outputStream;
        bool _polygonBegun = false;
        uint _polygonFirst;
        uint _polygonSecond = 0;
        uint _polygonCount = 0;

        public NGonDrawer(IGeometryOutput outputStream)
        {
            _outputStream = outputStream;
        }

        public void BeginNGon(Vertex v1, int n)
        {
            if (n < 3)
                n = 3;

            _polygonBegun = true;

            _outputStream.FlushIfRequired(n, 3 * (n - 2));

            _polygonFirst = _outputStream.AddVertex(v1);
            _polygonCount = 1;
        }

        public void AppendToNGon(Vertex v)
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


            uint currentFirst = _polygonFirst;
            uint currentSecond = _polygonSecond;

            /*
            if (_outputStream.FlushIfRequired(1, 3))
            {
                _backingMesh.Vertices[0] = _backingMesh.Vertices[currentFirst];
                _backingMesh.Vertices[1] = _backingMesh.Vertices[currentSecond];
                _polygonFirst = 0;
                _polygonSecond = 1;

                _currentVertexIndex = 2;
            }
            */


            _polygonCount++;

            uint polygonThird = _outputStream.AddVertex(v);
            _outputStream.MakeTriangle(_polygonFirst, _polygonSecond, polygonThird);

            _polygonSecond = polygonThird;
        }

        private void AppendSecondVertexToNGon(Vertex v)
        {
            _polygonSecond = _outputStream.AddVertex(v);

            _polygonCount = 2;
        }

        public void EndNGon()
        {
            _polygonBegun = false;
        }

    }
}

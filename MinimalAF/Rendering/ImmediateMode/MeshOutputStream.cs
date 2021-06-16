using System;

namespace MinimalAF.Rendering.ImmediateMode
{
    public class MeshOutputStream : IGeometryOutput, IDisposable
    {
        private Mesh _backingMesh;
        private uint _currentVertexIndex = 0;
        private uint _currentIndexIndex = 0;

        private double _utilization = 0;

        public double Utilization {
            get {
                return _utilization;
            }
        }

        public Vertex GetVertex(uint i)
        {
            return _backingMesh.Vertices[i];
        }

        public uint GetIndex(uint i)
        {
            return _backingMesh.Indices[i];
        }

        public MeshOutputStream(int vertexBufferSize, int indexBufferSize)
        {
            vertexBufferSize /= 3;
            vertexBufferSize *= 3;

            _backingMesh = new Mesh(new Vertex[vertexBufferSize], new uint[indexBufferSize], stream: true);
        }

        public uint AddVertex(Vertex v)
        {
            _backingMesh.Vertices[_currentVertexIndex] = v;
            return _currentVertexIndex++;
        }

        public void MakeTriangle(uint v1, uint v2, uint v3)
        {
            _backingMesh.Indices[_currentIndexIndex] = v1;
            _backingMesh.Indices[_currentIndexIndex + 1] = v2;
            _backingMesh.Indices[_currentIndexIndex + 2] = v3;

            _currentIndexIndex += 3;
        }

        public void Flush()
        {
            if (_currentIndexIndex == 0)
                return;

            _backingMesh.UpdateBuffers(_currentVertexIndex, _currentIndexIndex);

            _backingMesh.Draw();

            _utilization = _currentIndexIndex / (double)_currentVertexIndex;

            _currentVertexIndex = 0;
            _currentIndexIndex = 0;
        }

        public bool FlushIfRequired(int numIncomingVerts, int numIncomingIndices)
        {
            if (_currentIndexIndex + numIncomingIndices >= _backingMesh.Indices.Length ||
                    _currentVertexIndex + numIncomingVerts >= _backingMesh.Vertices.Length)
            {
                Flush();
                return true;
            }

            return false;
        }

        public uint CurrentV()
        {
            return _currentVertexIndex;
        }

        public uint CurrentI()
        {
            return _currentIndexIndex;
        }

        public void Dispose()
        {
            _backingMesh.Dispose();
        }
    }
}

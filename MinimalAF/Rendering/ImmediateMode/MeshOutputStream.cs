using System;

namespace MinimalAF.Rendering.ImmediateMode {
    public class MeshOutputStream : IGeometryOutput, IDisposable {
        private Mesh<Vertex> backingMesh;
        private uint currentVertexIndex = 0;
        private uint currentIndexIndex = 0;

        private double utilization = 0;

        public double Utilization {
            get {
                return utilization;
            }
        }

        /// <summary>
        /// Only used in DEBUG mode
        /// </summary>
        public int TimesVertexThresholdReached = 0;
        /// <summary>
        /// Only used in DEBUG mode
        /// </summary>
        public int TimesIndexThresholdReached = 0;


        public Vertex GetVertex(uint i) {
            return backingMesh.Vertices[i];
        }

        public uint GetIndex(uint i) {
            return backingMesh.Indices[i];
        }

        public MeshOutputStream(int vertexBufferSize, int indexBufferSize) {
            vertexBufferSize /= 3;
            vertexBufferSize *= 3;

            backingMesh = new Mesh<Vertex>(new Vertex[vertexBufferSize], new uint[indexBufferSize], stream: true);
        }

        public uint AddVertex(Vertex v) {
            backingMesh.Vertices[currentVertexIndex] = v;
            return currentVertexIndex++;
        }

        public void MakeTriangle(uint v1, uint v2, uint v3) {
            backingMesh.Indices[currentIndexIndex] = v1;
            backingMesh.Indices[currentIndexIndex + 1] = v2;
            backingMesh.Indices[currentIndexIndex + 2] = v3;

            currentIndexIndex += 3;
        }

        public void Flush() {
            if (currentIndexIndex == 0)
                return;

            // actually draw what we have so far
            backingMesh.UpdateBuffers(currentVertexIndex, currentIndexIndex);
            backingMesh.Draw();

            currentVertexIndex = 0;
            currentIndexIndex = 0;
        }

        public bool FlushIfRequired(int numIncomingVerts, int numIncomingIndices) {
            if (currentIndexIndex + numIncomingIndices >= backingMesh.Indices.Length ||
                    currentVertexIndex + numIncomingVerts >= backingMesh.Vertices.Length) {
#if DEBUG
                if(currentIndexIndex + numIncomingIndices >= backingMesh.Indices.Length) {
                    TimesIndexThresholdReached++;
                } else if(currentVertexIndex + numIncomingVerts >= backingMesh.Vertices.Length)  {
                    TimesVertexThresholdReached++;
                }
#endif

                Flush();
                return true;
            }

            return false;
        }

        public uint CurrentV() {
            return currentVertexIndex;
        }

        public uint CurrentI() {
            return currentIndexIndex;
        }

        public void Dispose() {
            backingMesh.Dispose();
        }
    }
}

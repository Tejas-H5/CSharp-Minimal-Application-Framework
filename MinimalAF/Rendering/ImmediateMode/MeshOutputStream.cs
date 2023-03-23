using System;

namespace MinimalAF.Rendering {
    public class MeshOutputStream<V> : IDisposable, IGeometryOutput<V> where V : struct {
        private Mesh<V> backingMesh;

        public V GetVertex(uint i) {
            return backingMesh.Vertices[i];
        }

        public uint GetIndex(int i) {
            return backingMesh.Indices[i];
        }

        public MeshOutputStream(int vertexBufferVerts, int indexBufferTriangles) {
            backingMesh = new Mesh<V>(
                vertexBufferVerts,
                indexBufferTriangles,
                stream: true, 
                allowResizing: false
            );
            backingMesh.Clear();
        }

        public uint AddVertex(V v) {
            return backingMesh.AddVertex(v);
        }

        public void MakeTriangle(uint v1, uint v2, uint v3) {
            backingMesh.MakeTriangle(v1, v2, v3);
        }

        public void Flush() {
            // actually draw what we have so far
            backingMesh.UploadToGPU();
            backingMesh.Render();
            backingMesh.Clear();
        }

        /// <summary>
        /// Return true if this function called Flush().
        /// Useful for when you want to ensure that we can send numIncomingVerts and numIncomingIndices without flushing.
        /// </summary>
        public bool FlushIfRequired(int numIncomingVerts, int numIncomingIndices) {
            int currentIndexIndex = backingMesh.IndexCount;
            int currentVertexIndex = backingMesh.VertexCount;
            if (currentIndexIndex + numIncomingIndices >= backingMesh.Indices.Length ||
                    currentVertexIndex + numIncomingVerts >= backingMesh.Vertices.Length) {

                Flush();
                return true;
            }

            return false;
        }


        public void Dispose() {
            backingMesh.Dispose();
        }
    }
}

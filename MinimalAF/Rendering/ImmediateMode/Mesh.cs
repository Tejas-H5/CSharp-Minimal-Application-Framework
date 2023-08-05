using MinimalAF.ResourceManagement;
using OpenTK.Graphics.OpenGL;
using System;

namespace MinimalAF.Rendering {
    public interface IMesh : IDisposable {
    }

    public enum MeshBufferType {
        Static, Dynamic, Stream
    }

    public class Mesh<V> : IMesh, IGeometryOutput<V> where V : struct {
        int _vbo;
        int _ebo;
        int _vao;
        int _sizeOfVertex;
        bool _changed = false;
        BufferUsageHint _bufferUsage;

        bool _allowResizing;

        ArrayList<V> _vertices;
        ArrayList<uint> _indices;

        public V[] Vertices => _vertices.Data;
        public uint[] Indices => _indices.Data;
        public int IndexCount => _indices.Length;
        public int VertexCount => _vertices.Length;

        int _lastVertexCount, _lastIndexCount;

        public int Handle => _vao;

        public void Clear() {
            _vertices.Clear();
            _indices.Clear();
        }

        /// <summary>
        /// If you are going to update the data every frame with UpdateBuffers, then set stream=true.
        /// </summary>
        public Mesh(
            int vertexCount, 
            int triangleCount, 
            bool stream, 
            bool allowResizing
        ) {
            _vertices = new ArrayList<V>(new V[vertexCount], vertexCount);
            _indices = new ArrayList<uint>(new uint[triangleCount * 3], triangleCount * 3);

            _allowResizing = allowResizing;

            // DynamicDraw should be used for what I thought StreamDraw was for
            _bufferUsage = stream ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw;

            // Set up mesh within OpenGL
            {
                var vertexDesc = VertexTypes.GetVertexDescription<V>();
                _sizeOfVertex = vertexDesc.GetSizeInBytes();
                var vertexAttributeInfo = vertexDesc.Attributes;

                _vao = GL.GenVertexArray();
                GL.BindVertexArray(_vao);

                _vbo = GL.GenBuffer();
                ReallocateVertices();

                _ebo = GL.GenBuffer();
                ReallocateIndices();

                // register vertex attributes using vertex description.
                // Might remove this if it becomes too much of a hassle
                int currentOffset = 0;
                for (int attribute = 0; attribute < vertexAttributeInfo.Length; attribute++) {
                    int fieldCount = vertexAttributeInfo[attribute].ComponentCount; ;
                    var fieldType = vertexAttributeInfo[attribute].GLType;
                    bool normalized = false;
                    int stride = _sizeOfVertex;
                    int offsetIntoVertex = currentOffset;

                    GL.VertexAttribPointer(attribute, fieldCount, fieldType, normalized, stride, offsetIntoVertex);
                    GL.EnableVertexAttribArray(attribute);

                    currentOffset += fieldCount * vertexAttributeInfo[attribute].SizeOf;
                }

                // unbind this thing so that future GL calls won't act on this by accident
                GL.BindVertexArray(0);
            }
        }

        private void ReallocateVertices() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Data.Length * _sizeOfVertex, _vertices.Data, _bufferUsage);
            _lastVertexCount = _vertices.Data.Length;
        }

        private void ReallocateIndices() {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Data.Length * sizeof(uint), _indices.Data, _bufferUsage);
            _lastIndexCount = _indices.Data.Length;
        }

        /// <summary>
        /// newVertexCount and newIndexCount MUST be less than the total number of vertices and indices on this mesh object.
        /// </summary>
        public void UploadToGPU() {
            GL.BindVertexArray(_vao);

            if (_indices.Length > _lastIndexCount || _vertices.Length > _lastVertexCount) {
                if (!_allowResizing) {
                    throw new Exception("This mesh buffer cannot be resized. You added too many verts or indices");
                }

                // re-allocate the vertex and index buffers. these functions automatically copy things, 
                // so we don't need to use buffer sub-data
                ReallocateVertices();
                ReallocateIndices();
            } else {
                // update subset of vertices
                GL.BindBuffer(
                    BufferTarget.ArrayBuffer, _vbo
                );
                GL.BufferSubData(
                    BufferTarget.ArrayBuffer, (IntPtr)0, _vertices.Length * _sizeOfVertex, _vertices.Data
                );

                // update subset of indices
                GL.BindBuffer(
                    BufferTarget.ElementArrayBuffer, _ebo
                );
                GL.BufferSubData(
                    BufferTarget.ElementArrayBuffer, (IntPtr)0, _indices.Length * sizeof(uint), _indices.Data
                );
            }

            _changed = false;
        }
        
        public void Render() {
            if (_changed) {
                _changed = false;
                UploadToGPU();
            }

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, (int)_indices.Length, DrawElementsType.UnsignedInt, (IntPtr)0);
        }


        private bool disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (disposed)
                return;

            GLDeletionQueue.QueueBufferForDeletion(_vbo);
            GLDeletionQueue.QueueBufferForDeletion(_ebo);
            GLDeletionQueue.QueueVertexArrayForDeletion(_vao);
            Console.WriteLine("Mesh destructed");

            disposed = true;
        }

        ~Mesh() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public uint AddVertex(V v) {
            _changed = true;

            _vertices.Append(v);
            return (uint)_vertices.Length - 1;
        }

        public void MakeTriangle(uint v1, uint v2, uint v3) {
            _indices.Append(v1);
            _indices.Append(v2);
            _indices.Append(v3);

            _changed = true;
        }

        public bool FlushIfRequired(int incomingVertexCount, int incomingIndexConut) {
            // The concept of flushing doesn't apply here
            return false;
        }
    }
}

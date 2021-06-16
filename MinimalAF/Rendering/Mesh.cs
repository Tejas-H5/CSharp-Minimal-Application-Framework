using OpenTK.Graphics.OpenGL;
using System;

namespace MinimalAF.Rendering
{
    public class Mesh : IDisposable
    {
        Vertex[] _vertices;
        uint[] _indices;

        int _vbo;
        int _ebo;

        int _vao;

        uint _indexCount;
        uint _vertexCount;

        public int Handle {
            get {
                return _vao;
            }
        }

        public Vertex[] Vertices {
            get {
                return _vertices;
            }
        }

        public uint[] Indices {
            get {
                return _indices;
            }
        }


        /// <summary>
        /// If you are going to update the data every frame with UpdateBuffers, then set stream=true.
        /// </summary>
        public Mesh(Vertex[] data, uint[] indices, bool stream = false)
        {
            BufferUsageHint bufferUsage = BufferUsageHint.StaticDraw;
            if (stream)
                bufferUsage = BufferUsageHint.StreamDraw;

            _vertices = data;
            _indices = indices;

            _indexCount = (uint)_indices.Length;
            _vertexCount = (uint)_vertices.Length;

            InitMeshOpenGL(bufferUsage);
        }

        private void InitMeshOpenGL(BufferUsageHint bufferUsage)
        {
            InitializeVertices(bufferUsage);
            InitializeIndices(bufferUsage);

            GL.BindVertexArray(0);
        }

        private void InitializeVertices(BufferUsageHint bufferUsage)
        {
            GenerateAndBindVertexBuffer();
            SendDataToBoundBuffer(bufferUsage);
            GenerateAndBindVertexArray();
            RegisterVertexAttributes();
        }

        private void GenerateAndBindVertexBuffer()
        {
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        }

        private void SendDataToBoundBuffer(BufferUsageHint bufferUsage)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Vertex.SizeOf(), _vertices, bufferUsage);
        }

        private void GenerateAndBindVertexArray()
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);
        }

        private static void RegisterVertexAttributes()
        {
            CreateVertexAttribPointer(attribute: 0, length: 3, offsetInArray: 0);
            CreateVertexAttribPointer(attribute: 1, length: 2, offsetInArray: 3);
        }

        private static void CreateVertexAttribPointer(int attribute, int length, int offsetInArray)
        {
            GL.VertexAttribPointer(attribute, length, VertexAttribPointerType.Float, false, Vertex.SizeOf(), offsetInArray * sizeof(float));
            GL.EnableVertexAttribArray(attribute);
        }

        private void InitializeIndices(BufferUsageHint bufferUsage)
        {
            GenerateAndBindIndexBuffer();
            SendIndicesToBoundIndexBuffer(bufferUsage);
        }

        private void GenerateAndBindIndexBuffer()
        {
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        }

        private void SendIndicesToBoundIndexBuffer(BufferUsageHint bufferUsage)
        {
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, bufferUsage);
        }

        public void UpdateBuffers()
        {
            UpdateBuffers((uint)_vertices.Length, (uint)_indices.Length);
        }

        /// <summary>
        /// newVertexCount and newIndexCount MUST be less than the total number of vertices and indices on this mesh object.
        /// </summary>
        public void UpdateBuffers(uint newVertexCount, uint newIndexCount)
        {
            _indexCount = newIndexCount;
            _vertexCount = newVertexCount;

            if(_indexCount > _indices.Length || _vertexCount > _vertices.Length)
            {
                throw new Exception("The mesh buffer does not have this many vertices."
                    + "you may only specify new index and vertex counts that are less than the amount initially allocated");
            }


            GL.BindVertexArray(_vao);

            UpdateVertexBuffer();

            UpdateIndexBuffer();
        }

        private void UpdateVertexBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (int)_vertexCount * Vertex.SizeOf(), _vertices);
        }

        private void UpdateIndexBuffer()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)0, (int)_indexCount * sizeof(uint), _indices);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, (int)_indexCount, DrawElementsType.UnsignedInt, (IntPtr)0);
        }


        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            GL.BindVertexArray(0);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);

            Console.WriteLine("Mesh destructed");

            disposed = true;
        }

        ~Mesh()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

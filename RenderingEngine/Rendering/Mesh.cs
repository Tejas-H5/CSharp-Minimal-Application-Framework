using OpenTK.Graphics.OpenGL;
using System;

namespace RenderingEngine.Rendering
{

    public class Mesh : IDisposable
    {
        Vertex[] _data;
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
                return _data;
            }
        }

        public uint[] Indices {
            get {
                return _indices;
            }
        }

        public Mesh(Vertex[] data, uint[] indices, bool stream = false)
        {
            _data = data;
            _indices = indices;

            _indexCount = (uint)_indices.Length;
            _vertexCount = (uint)_data.Length;

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            if (stream)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Vertex.SizeOf(), _data, BufferUsageHint.StreamDraw);
            }
            else
            {
                GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Vertex.SizeOf(), _data, BufferUsageHint.StaticDraw);
            }

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            int positionInArray = 0;
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeOf(), positionInArray * sizeof(float));
            GL.EnableVertexAttribArray(0);
            positionInArray += 3;

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.SizeOf(), positionInArray * sizeof(float));
            GL.EnableVertexAttribArray(1);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

            if (stream)
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StreamDraw);
            }
            else
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            }

            GL.BindVertexArray(0);
        }


        public void UpdateBuffer()
        {
            UpdateBuffer((uint)_data.Length, (uint)_indices.Length);
        }

        public void UpdateBuffer(uint vertexCount, uint indexCount)
        {
            _indexCount = indexCount;
            _vertexCount = vertexCount;

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (int)_vertexCount * Vertex.SizeOf(), _data);

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

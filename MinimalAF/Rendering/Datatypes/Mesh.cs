using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace MinimalAF.Rendering {
    public class Mesh : IDisposable {
        Vertex[] vertices;
        uint[] indices;

        int vbo;
        int ebo;

        int vao;

        uint indexCount;
        uint vertexCount;

        public int Handle {
            get {
                return vao;
            }
        }

        public Vertex[] Vertices {
            get {
                return vertices;
            }
        }

        public uint[] Indices {
            get {
                return indices;
            }
        }

        /// <summary>
        /// If you are going to update the data every frame with UpdateBuffers, then set stream=true.
        /// </summary>
        public Mesh(Vertex[] data, uint[] indices, bool stream = false) {
            BufferUsageHint bufferUsage = BufferUsageHint.StaticDraw;
            if (stream)
                bufferUsage = BufferUsageHint.StreamDraw;

            vertices = data;
            this.indices = indices;

            indexCount = (uint)indices.Length;
            vertexCount = (uint)vertices.Length;

            InitMeshOpenGL(bufferUsage);
        }



        private void InitMeshOpenGL(BufferUsageHint bufferUsage) {
            InitializeVertices(bufferUsage);
            InitializeIndices(bufferUsage);

            GL.BindVertexArray(0);
        }

        private void InitializeVertices(BufferUsageHint bufferUsage) {
            GenerateAndBindVertexBuffer();
            SendDataToBoundBuffer(bufferUsage);
            GenerateAndBindVertexArray();
            RegisterVertexAttributes();
        }

        private void GenerateAndBindVertexBuffer() {
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        }

        private void SendDataToBoundBuffer(BufferUsageHint bufferUsage) {
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.VERTEX_SIZE, vertices, bufferUsage);
        }

        private void GenerateAndBindVertexArray() {
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
        }

        

        private void RegisterVertexAttributes() {
            int currentOffset = 0;

            for(int i = 0; i < Vertex.VERTEX_COMPONENTS.Length; i++) {
                int fieldCount = Vertex.VERTEX_COMPONENTS[i];
                CreateVertexAttribPointer(i, fieldCount, currentOffset);

                currentOffset += fieldCount * sizeof(float);
            }
        }

        private void CreateVertexAttribPointer(int attribute, int length, int offsetInArray) {
            GL.VertexAttribPointer(attribute, length, VertexAttribPointerType.Float, false, Vertex.VERTEX_SIZE, offsetInArray);
            GL.EnableVertexAttribArray(attribute);
        }

        private void InitializeIndices(BufferUsageHint bufferUsage) {
            GenerateAndBindIndexBuffer();
            SendIndicesToBoundIndexBuffer(bufferUsage);
        }

        private void GenerateAndBindIndexBuffer() {
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        }

        private void SendIndicesToBoundIndexBuffer(BufferUsageHint bufferUsage) {
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, bufferUsage);
        }

        public void UpdateBuffers() {
            UpdateBuffers((uint)vertices.Length, (uint)indices.Length);
        }

        /// <summary>
        /// newVertexCount and newIndexCount MUST be less than the total number of vertices and indices on this mesh object.
        /// </summary>
        public void UpdateBuffers(uint newVertexCount, uint newIndexCount) {
            indexCount = newIndexCount;
            vertexCount = newVertexCount;

            if (indexCount > indices.Length || vertexCount > vertices.Length) {
                throw new Exception("The mesh buffer does not have this many vertices."
                    + "you may only specify new index and vertex counts that are less than the amount initially allocated");
            }


            GL.BindVertexArray(vao);

            UpdateVertexBuffer();
            UpdateIndexBuffer();
        }

        private void UpdateVertexBuffer() {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (int)vertexCount * Vertex.VERTEX_SIZE, vertices);
        }

        private void UpdateIndexBuffer() {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)0, (int)indexCount * sizeof(uint), indices);
        }

        public void Draw() {
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, (int)indexCount, DrawElementsType.UnsignedInt, (IntPtr)0);
        }


        private bool disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (disposed)
                return;

            GL.BindVertexArray(0);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);

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
    }
}

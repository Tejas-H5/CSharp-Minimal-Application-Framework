using MinimalAF.ResourceManagement;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace MinimalAF.Rendering {
    public interface IMesh : IDisposable {
    }


    public class Mesh<V> : IMesh where V : struct {
        V[] vertices;
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

        public V[] Vertices {
            get {
                return vertices;
            }
        }

        public uint[] Indices {
            get {
                return indices;
            }
        }

        VertexAttributeInfo[] vertexAttributeInfo;
        int sizeOfVertex;

        /// <summary>
        /// If you are going to update the data every frame with UpdateBuffers, then set stream=true.
        /// </summary>
        public Mesh(V[] data, uint[] indices, bool stream = false) {
            vertexAttributeInfo = VertexTypes.GetVertexDescription<V>();
            sizeOfVertex = 0;
            foreach (var info in vertexAttributeInfo) {
                sizeOfVertex += info.ComponentCount * info.SizeOf;
            }

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
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeOfVertex, vertices, bufferUsage);
        }

        private void GenerateAndBindVertexArray() {
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
        }



        private void RegisterVertexAttributes() {
            int currentOffset = 0;

            for (int i = 0; i < vertexAttributeInfo.Length; i++) {
                int fieldCount = vertexAttributeInfo[i].ComponentCount; ;
                CreateVertexAttribPointer(i, fieldCount, currentOffset, vertexAttributeInfo[i].GLType);

                currentOffset += fieldCount * vertexAttributeInfo[i].SizeOf;
            }
        }

        private void CreateVertexAttribPointer(int attribute, int length, int offsetInArray, VertexAttribPointerType type) {
            GL.VertexAttribPointer(attribute, length, type, false, sizeOfVertex, offsetInArray);
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
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (int)vertexCount * sizeOfVertex, vertices);
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

            GLDeletionQueue.QueueBufferForDeletion(vbo);
            GLDeletionQueue.QueueBufferForDeletion(ebo);
            GLDeletionQueue.QueueVertexArrayForDeletion(vao);
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

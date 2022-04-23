using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace MinimalAF.Rendering {
    /// <summary>
    /// V must be a vertex class, where all fields are either
    /// float, OpenTK.Mathematics.Vector2, Vector3 or Vector4.
    /// All fields must have a [VertexComponent(...)] c# attribute.
    /// 
    /// I can extend this more if needed.
    /// </summary>
    public class Mesh<V> : IDisposable where V : unmanaged {
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


        (int, int) GetFieldSize(Type t) {
            const int f = sizeof(float);

            if (t == typeof(float)) {
                return (f, 1);
            } else if (t == typeof(Vector2)) {
                return (f, 2);
            } else if (t == typeof(Vector3)) {
                return (f, 3);
            } else if (t == typeof(Vector4)) {
                return (f, 4);
            }

            return (-1, -1);
        }

        static readonly Type[] AllowedFieldTypes = new Type[] {
            typeof(float),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4)
        };

        VertexComponentAttribute[] vertexComponents;
        int vertexSize;

        /// <summary>
        /// If you are going to update the data every frame with UpdateBuffers, then set stream=true.
        /// </summary>
        public Mesh(V[] data, uint[] indices, bool stream = false) {
            GetVertexComponentInfoWithReflection(typeof(V));

            BufferUsageHint bufferUsage = BufferUsageHint.StaticDraw;
            if (stream)
                bufferUsage = BufferUsageHint.StreamDraw;

            vertices = data;
            this.indices = indices;

            indexCount = (uint)indices.Length;
            vertexCount = (uint)vertices.Length;

            InitMeshOpenGL(bufferUsage);
        }

        void GetVertexComponentInfoWithReflection(Type type) {
            var fields = type.GetFields()
                .OrderBy(field => (uint)Marshal.OffsetOf(type, field.Name))
                .ToArray();

            foreach (var field in fields) {
                (int typeSize, int count) = GetFieldSize(field.FieldType);
                bool isAllowedType = typeSize != -1;
                if (!isAllowedType) {
                    throw new Exception(
                        "A field of type " + field.FieldType.Name + " is not allowed on a vertex.\n" +
                        "They need to be one of:\n" +
                            string.Join(", ", AllowedFieldTypes.Select(t => t.Assembly.FullName + t.Name))
                    );
                }

                var attributes = field.GetCustomAttributes(false);
                if (attributes.Length != 1 || !(attributes[0] is VertexComponentAttribute)) {
                    throw new Exception(
                        "All fields in a vertex must have a [VertexComponentAttribute(...)] C# attribute."
                    );
                }
            }

            vertexComponents = new VertexComponentAttribute[fields.Length];
            vertexSize = 0;
            for(int i = 0; i < fields.Length; i++) {
                var attribute = fields[i].GetCustomAttributes(false)[0] as VertexComponentAttribute;

                (int size, int count) = GetFieldSize(fields[i].FieldType);
                attribute.FieldSize = size;
                attribute.FieldCount = count;

                vertexComponents[i] = attribute;
                vertexSize += size * count;
            }
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
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * vertexSize, vertices, bufferUsage);
        }

        private void GenerateAndBindVertexArray() {
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
        }

        private void RegisterVertexAttributes() {
            int currentOffset = 0;
            for(int i = 0; i < vertexComponents.Length; i++) {
                CreateVertexAttribPointer(i, vertexComponents[i].FieldCount, currentOffset);
                currentOffset += vertexComponents[i].FieldSize * vertexComponents[i].FieldCount;
            }
        }

        private void CreateVertexAttribPointer(int attribute, int length, int offsetInArray) {
            GL.VertexAttribPointer(attribute, length, VertexAttribPointerType.Float, false, vertexSize, offsetInArray);
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
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (int)vertexCount * vertexSize, vertices);
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

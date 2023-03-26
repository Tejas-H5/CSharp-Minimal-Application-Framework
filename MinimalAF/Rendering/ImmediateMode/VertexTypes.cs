using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MinimalAF {

    public struct VertexAttributeInfo {
        public int ComponentCount;
        public int SizeOf;
        public VertexAttribPointerType GLType;
        public string Name;
    }

    public struct VertexDescription {
        public VertexAttributeInfo[] Attributes;

        /// <summary>
        /// Returns the vertex size in bytes
        /// </summary>
        public int GetSizeInBytes() {
            int sizeOfVertex = 0;
            foreach (var info in Attributes) {
                sizeOfVertex += info.ComponentCount * info.SizeOf;
            }
            return sizeOfVertex;
        }
    }


    internal static class VertexTypes {
        static int GetComponentCount(Type t) {
            if (t == typeof(int)) return 1;
            if (t == typeof(float)) return 1;
            if (t == typeof(bool)) return 1;
            if (t == typeof(Vector2)) return 2;
            if (t == typeof(Vector3)) return 3;
            if (t == typeof(Vector4)) return 4;
            if (t == typeof(Matrix2)) return 4;
            if (t == typeof(Matrix3)) return 9;
            if (t == typeof(Matrix4)) return 16; // why tf would anyone have a Matrix4 vertex ?

            return -1;
        }

        static int SizeOf(Type t) {
            if (t == typeof(int)) return sizeof(int);
            if (t == typeof(float)) return sizeof(float);
            if (t == typeof(bool)) return sizeof(bool);
            if (t == typeof(Vector2)) return sizeof(float);
            if (t == typeof(Vector3)) return sizeof(float);
            if (t == typeof(Vector4)) return sizeof(float);
            if (t == typeof(Matrix2)) return sizeof(float);
            if (t == typeof(Matrix3)) return sizeof(float);
            if (t == typeof(Matrix4)) return sizeof(float);

            return -1;
        }

        static VertexAttribPointerType AttribType(Type t) {
            if (t == typeof(int)) return VertexAttribPointerType.Int;
            if (t == typeof(float)) return VertexAttribPointerType.Float;
            if (t == typeof(bool)) return VertexAttribPointerType.Int;
            if (t == typeof(Vector2)) return VertexAttribPointerType.Float;
            if (t == typeof(Vector3)) return VertexAttribPointerType.Float;
            if (t == typeof(Vector4)) return VertexAttribPointerType.Float;
            if (t == typeof(Matrix2)) return VertexAttribPointerType.Float;
            if (t == typeof(Matrix3)) return VertexAttribPointerType.Float;
            if (t == typeof(Matrix4)) return VertexAttribPointerType.Float;
            return (VertexAttribPointerType)(-1);
        }


        public static VertexDescription GetVertexDescription<T>() where T : struct {
            var fields = typeof(T).GetFields()
                .OrderBy(field => (uint)Marshal.OffsetOf(typeof(T), field.Name))
                .ToArray();

            VertexAttributeInfo[] info = new VertexAttributeInfo[fields.Length];

            for (int i = 0; i < fields.Length; i++) {
                Type t = fields[i].FieldType;
                int componentCount = GetComponentCount(t);
                if (componentCount == -1) {
                    throw new Exception("Vertex struct can't have a field of type " + t.Name);
                }

                // we assume the others work if GetComponentCount works
                info[i] = new VertexAttributeInfo {
                    SizeOf = SizeOf(t),
                    ComponentCount = componentCount,
                    GLType = AttribType(t),
                    Name = fields[i].Name
                };
            }

            return new VertexDescription {
                Attributes = info
            };
        }
    }
}

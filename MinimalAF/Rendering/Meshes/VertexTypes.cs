using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace MinimalAF.Rendering {

    public struct VertexAttributeInfo {
        public int ComponentCount;
        public int SizeOf;
        public VertexAttribPointerType GLType;
        public string Name;
    }


    internal static class VertexTypes {
        static readonly Dictionary<Type, int> ComponentCounts = new Dictionary<Type, int> {
            {typeof(int), 1 },
            {typeof(float), 1 },
            {typeof(bool), 1 },
            {typeof(Vector2), 2 },
            {typeof(Vector3), 3 },
            {typeof(Vector4), 4 },
            {typeof(Matrix2), 4 },
            {typeof(Matrix3), 9 },
            {typeof(Matrix4), 16 }, // why tf would anyone have a Matrix4 vertex ?
        };

        static readonly Dictionary<Type, int> SubcomponentSizeOf = new Dictionary<Type, int> {
            {typeof(int), sizeof(int) },
            {typeof(float), sizeof(float) },
            {typeof(bool), sizeof(bool) },
            {typeof(Vector2), sizeof(float) },
            {typeof(Vector3), sizeof(float) },
            {typeof(Vector4), sizeof(float) },
            {typeof(Matrix2), sizeof(float) },
            {typeof(Matrix3), sizeof(float) },
            {typeof(Matrix4), sizeof(float) },
        };

        static readonly Dictionary<Type, VertexAttribPointerType> GLType = new Dictionary<Type, VertexAttribPointerType> {
            {typeof(int), VertexAttribPointerType.Int },
            {typeof(float), VertexAttribPointerType.Float },
            {typeof(bool), VertexAttribPointerType.Int },
            {typeof(Vector2), VertexAttribPointerType.Float },
            {typeof(Vector3), VertexAttribPointerType.Float },
            {typeof(Vector4), VertexAttribPointerType.Float },
            {typeof(Matrix2), VertexAttribPointerType.Float },
            {typeof(Matrix3), VertexAttribPointerType.Float },
            {typeof(Matrix4), VertexAttribPointerType.Float },
        };

        static readonly Dictionary<Type, VertexAttributeInfo[]> vertexInfoCache = new Dictionary<Type, VertexAttributeInfo[]>();

        static int GetComponentCount(Type t) {
            if (ComponentCounts.ContainsKey(t)) {
                return ComponentCounts[t];
            }

            return -1;
        }

        static int SizeOf(Type t) {
            return SubcomponentSizeOf[t];
        }

        static VertexAttribPointerType AttribType(Type t) {
            return GLType[t];
        }


        public static VertexAttributeInfo[] GetVertexDescription<T>() where T : struct {
            if(vertexInfoCache.ContainsKey(typeof(T))) {
                return vertexInfoCache[typeof(T)];
            }

            var fields = typeof(T).GetFields()
                .OrderBy(field => (uint)Marshal.OffsetOf(typeof(T), field.Name))
                .ToArray();

            VertexAttributeInfo[] info = new VertexAttributeInfo[fields.Length];

            for(int i = 0; i < fields.Length; i++) {
                Type t = fields[i].FieldType;
                int componentCount = GetComponentCount(t);

                if(componentCount == -1) {
                    throw new Exception("Vertex struct can't have a field of type " + t.Name);
                }

                info[i] = new VertexAttributeInfo {
                    SizeOf = SizeOf(t),
                    ComponentCount = componentCount,
                    GLType = AttribType(t),
                    Name = fields[i].Name
                };
            }

            vertexInfoCache[typeof(T)] = info;
            return info;
        }
    }
}

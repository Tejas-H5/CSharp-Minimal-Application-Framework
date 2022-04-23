using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MinimalAF.Rendering {
    public readonly struct VertexTypeInfo {
        public readonly VertexComponentAttribute[] VertexComponents;
        public readonly int VertexSize;

        public VertexTypeInfo(VertexComponentAttribute[] vertexComponents, int vertexSize) {
            VertexComponents = vertexComponents;
            VertexSize = vertexSize;
        }
    }

    public static class VertexTypes {
        static Dictionary<Type, VertexTypeInfo> loadedTypes = new Dictionary<Type, VertexTypeInfo>();

        static (int, int) GetFieldSize(Type t) {
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

        public static VertexTypeInfo GetvertexTypeInfo(Type type) {
            if(loadedTypes.ContainsKey(type)) {
                return loadedTypes[type];
            }

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

            var vertexComponents = new VertexComponentAttribute[fields.Length];
            int vertexSize = 0;
            for (int i = 0; i < fields.Length; i++) {
                var attribute = fields[i].GetCustomAttributes(false)[0] as VertexComponentAttribute;

                (int size, int count) = GetFieldSize(fields[i].FieldType);
                attribute.FieldSize = size;
                attribute.FieldCount = count;

                vertexComponents[i] = attribute;
                vertexSize += size * count;
            }

            return new VertexTypeInfo(vertexComponents, vertexSize);
        }
    }
}

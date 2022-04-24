using MinimalAF.Rendering.ImmediateMode;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Rendering {
    public class MeshData<V> where V : unmanaged {
        List<V> vertices = new List<V>();
        List<uint> indices = new List<uint>();

        public List<V> Vertices => vertices;
        public List<uint> Indices => indices;

        public uint AddVertex(V v) {
            vertices.Add(v);

            return (uint)(vertices.Count - 1);
        }

        public void MakeFace(uint v1, uint v2, uint v3) {
            indices.Add(v1);
            indices.Add(v2);
            indices.Add(v3);
        }

        public Mesh<V> ToDrawableMesh() {
            return new Mesh<V>(vertices.ToArray(), indices.ToArray(), false);
        }


        public static MeshData<Vertex> FromOBJ(string text) {
            return OBJParser.FromOBJ(text);
        }
    }
}
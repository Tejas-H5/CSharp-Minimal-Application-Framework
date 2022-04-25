using MinimalAF.Rendering.ImmediateMode;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Rendering {
    public class MeshData  {
        List<Vertex> vertices;
        List<uint> indices;

        public MeshData() {
            vertices = new List<Vertex>();
            indices = new List<uint>();
        }


        public MeshData(List<Vertex> vertices, List<uint> indices) {
            this.vertices = vertices;
            this.indices = indices;
        }

        public List<Vertex> Vertices => vertices;
        public List<uint> Indices => indices;

        public uint AddVertex(Vertex v) {
            vertices.Add(v);

            return (uint)(vertices.Count - 1);
        }

        public void MakeTriangle(uint v1, uint v2, uint v3) {
            indices.Add(v1);
            indices.Add(v2);
            indices.Add(v3);
        }

        public Mesh ToDrawableMesh() {
            return new Mesh(vertices.ToArray(), indices.ToArray(), false);
        }


        public static MeshData FromOBJ(string text) {
            return OBJParser.FromOBJ(text);
        }

        public string ToCodeString() {
            StringBuilder sb = new StringBuilder();

            sb.Append("List<Vertex> verts = new List<Vertex>(new Vertex[] {");
            for(int i = 0; i < vertices.Count; i++) {
                sb.Append(vertices[i].ToCodeString());
            }
            sb.Append("});");

            return sb.ToString();
        }
    }
}
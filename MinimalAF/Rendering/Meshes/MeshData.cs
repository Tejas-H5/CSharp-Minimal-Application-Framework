using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Rendering {
    public class MeshData<V> : IGeometryOutput<V> where V : struct {
        List<V> vertices;
        List<uint> indices;

        public MeshData() {
            vertices = new List<V>();
            indices = new List<uint>();
        }

        public MeshData(List<V> vertices, List<uint> indices) {
            this.vertices = vertices;
            this.indices = indices;
        }

        public List<V> Vertices => vertices;
        public List<uint> Indices => indices;

        public uint AddVertex(V v) {
            vertices.Add(v);

            return (uint)(vertices.Count - 1);
        }

        public void MakeTriangle(uint v1, uint v2, uint v3) {
            indices.Add(v1);
            indices.Add(v2);
            indices.Add(v3);
        }

        public Mesh<V> ToDrawableMesh() {
            return new Mesh<V>(vertices.ToArray(), indices.ToArray(), false);
        }


        public static MeshData<V1> FromOBJ<V1>(string text) where V1 : struct, IVertexPosition, IVertexUV {
            return MeshParserWavefrontOBJ<V1>.FromOBJ(text);
        }

        public void Flush() {

        }

        public bool FlushIfRequired(int numIncomingVerts, int numIncomingIndices) {
            return false;
        }

        public uint CurrentV() {
            return (uint)(vertices.Count - 1);
        }

        public uint CurrentI() {
            return (uint)(indices.Count - 1);
        }
    }

    public static class MeshFormatter {
        public static string ToCodeString<V1>(this MeshData<V1> mesh) where V1 : struct, ICodeSerializeable {
            StringBuilder sb = new StringBuilder();

            sb.Append("List<V> verts = new List<V>(new V[] {");
            for (int i = 0; i < mesh.Vertices.Count; i++) {
                sb.Append(mesh.Vertices[i].ToCodeString());
            }
            sb.Append("});");

            return sb.ToString();
        }
    }

}
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
            MeshData<Vertex> mesh = new MeshData<Vertex>();
            List<Vector2> textureCoords = new List<Vector2>();
            int vertexCount = 0;
            int triangleCount = 0;

            // so that we can enfore vertexCount == uvCount
            int uvCount = 0;

            // lets find out how many of each there are, so we can set a capacity
            foreach (var line in text.IterSplit("\n")) {
                var lineIter = new StringIterator(line, " ");

                var start = lineIter.GetNext();
                if (start == "v") {
                    vertexCount++;
                } else if (start == "f") {
                    lineIter.MoveNext();
                    lineIter.MoveNext();

                    // every vertex onwards will require a new triangle
                    while (!lineIter.MoveNext()) {
                        triangleCount++;
                    }
                } else if (start == "vt") {
                    uvCount++;
                }
            }

            mesh.Vertices.Capacity = vertexCount;
            mesh.Indices.Capacity = triangleCount;

            foreach (var line in text.IterSplit("\n")) {
                var lineIter = new StringIterator(line, " ");

                var start = lineIter.GetNext();
                string useMtl = "";
                string mtl = "";

                if (start == "usemlt") {
                    useMtl = lineIter.GetNext().ToString();
                } else if (start == "mtllib") {
                    mtl = lineIter.GetNext().ToString();
                } else if (start == "v") {
                    mesh.AddVertex(new Vertex(
                        float.Parse(lineIter.GetNext().ToString()),
                        float.Parse(lineIter.GetNext().ToString()),
                        float.Parse(lineIter.GetNext().ToString())
                    ));
                } else if (start == "f") {
                    // TODO: Load faces here


                } else if (start == "vt") {
                    textureCoords.Add(new Vector2(
                        float.Parse(lineIter.GetNext()),
                        float.Parse(lineIter.GetNext())
                    ));
                }
            }

            return mesh;
        }
    }
}

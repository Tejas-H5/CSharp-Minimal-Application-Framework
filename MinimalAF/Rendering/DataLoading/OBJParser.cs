using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Rendering {
    internal class OBJParser {
        public static MeshData<Vertex> FromOBJ(string text) {
            MeshData<Vertex> mesh = new MeshData<Vertex>();
            List<Vector2> textureCoords = new List<Vector2>();

            (int vertexCount, int triangleCount, int uvCount) 
                = CalculateDataCounts(text);

            mesh.Vertices.Capacity = vertexCount;
            mesh.Indices.Capacity = triangleCount;
            textureCoords.Capacity = uvCount;

            foreach (var line in text.IterSplit("\n")) {
                var lineIter = new StringIterator(line, " ");

                var start = lineIter.GetNext();
                string useMtl = "";
                string mtl = "";

                if (start == "usemlt") {
                    useMtl = ParseCurrentMaterial(lineIter);
                } else if (start == "mtllib") {
                    mtl = ParseMaterial(lineIter);
                } else if (start == "v") {
                    ParseVertex(mesh, lineIter);
                } else if (start == "f") {
                    ParseFace(mesh, textureCoords, lineIter);
                } else if (start == "vt") {
                    ParseTexCoord(textureCoords, lineIter);
                }
            }

            return mesh;
        }

        private static (int, int, int) CalculateDataCounts(string text) {
            int vertexCount = 0;
            int triangleCount = 0;
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

            return (vertexCount, triangleCount, uvCount);
        }

        private static string ParseCurrentMaterial(StringIterator lineIter) {
            return lineIter.GetNext().ToString();
        }

        private static string ParseMaterial(StringIterator lineIter) {
            return lineIter.GetNext().ToString();
        }

        private static void ParseVertex(MeshData<Vertex> mesh, StringIterator lineIter) {
            mesh.AddVertex(new Vertex(
                float.Parse(lineIter.GetNext().ToString()),
                float.Parse(lineIter.GetNext().ToString()),
                float.Parse(lineIter.GetNext().ToString())
            ));
        }

        private static void ParseFace(MeshData<Vertex> mesh, List<Vector2> textureCoords, StringIterator lineIter) {
            int index1 = -1;
            int index2 = -1;

            do {
                var face = new StringIterator(lineIter.Current, "/", false);

                var vertexIndex = int.Parse(face.GetNext());
                var textureIndexText = face.GetNext();
                var textureIndex = textureIndexText == "" ? -1 : int.Parse(textureIndexText);

                if (index1 == -1) {
                    index1 = vertexIndex;
                    if (textureIndex != -1) {
                        mesh.Vertices[vertexIndex].SetUV(textureCoords[textureIndex]);
                    }
                } else if (index2 == -1) {
                    index2 = vertexIndex;
                    if (textureIndex != -1) {
                        mesh.Vertices[vertexIndex].SetUV(textureCoords[textureIndex]);
                    }
                } else {
                    mesh.MakeFace((uint)index1, (uint)index2, (uint)vertexIndex);
                    if (textureIndex != -1) {
                        mesh.Vertices[vertexIndex].SetUV(textureCoords[textureIndex]);
                    }

                    index2 = vertexIndex;
                }
            } while (lineIter.MoveNext());
        }

        private static void ParseTexCoord(List<Vector2> textureCoords, StringIterator lineIter) {
            textureCoords.Add(new Vector2(
                float.Parse(lineIter.GetNext()),
                float.Parse(lineIter.GetNext())
            ));
        }
    }
}

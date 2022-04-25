using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Rendering {
    internal class OBJParser {



        public static MeshData FromOBJ(string text) {
            MeshData mesh = new MeshData();
            List<Vector3> positions = new List<Vector3>();
            List<Vector2> textureCoords = new List<Vector2>();

            (int vertexCount, int triangleCount, int uvCount) 
                = CalculateDataCounts(text);

            mesh.Indices.Capacity = triangleCount;
            positions.Capacity = vertexCount;
            textureCoords.Capacity = uvCount;

            string useMtl = "";
            string mtl = "";

            foreach (var line in IterationUtil.Split(text, "\n")) {
                var trimmed = line.Trim();
                var lineIter = new StringIterator(trimmed, " ");
                var start = lineIter.GetNext();

                if(start.StartsWith("#")) {
                    continue;
                }

                if (start.SequenceEqual("usemlt")) {
                    useMtl = ParseCurrentMaterial(lineIter);
                } else if (start.SequenceEqual("mtllib")) {
                    mtl = ParseMaterial(lineIter);
                } else if (start.SequenceEqual("v")) {
                    ParseVertex(positions, lineIter);
                } else if (start.SequenceEqual("f")) {
                    ParseFace(mesh, positions, textureCoords, lineIter);
                } else if (start.SequenceEqual("vt")) {
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
            foreach (var line in IterationUtil.Split(text, "\n")) {
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

        private static void ParseVertex(List<Vector3> positions, StringIterator lineIter) {
            positions.Add(
                new Vector3(
                    float.Parse(lineIter.GetNext().ToString()),
                    float.Parse(lineIter.GetNext().ToString()),
                    float.Parse(lineIter.GetNext().ToString())
                )
            );
        }

        private static void ParseFace(MeshData mesh, List<Vector3> positions, List<Vector2> textureCoords, StringIterator lineIter) {
            uint index1 = uint.MaxValue;
            uint index2 = uint.MaxValue;

            while (lineIter.MoveNext()) {
                var face = new StringIterator(lineIter.Current, "/", false);

                // an obj file will reuse the same vertex for different faces/UV islands, so we need to create new vertices for each point
                // on a face

                var vertexIndex = int.Parse(face.GetNext()) - 1;
                var textureIndexText = face.GetNext();
                var textureIndex = textureIndexText == "" ? -1 : int.Parse(textureIndexText) - 1;
                Vector2 uv = new Vector2(0, 0);
                if(textureIndex != -1) {
                    uv = textureCoords[textureIndex];
                }
                // because UVs are upside down otherwise
                uv.Y = 1 - uv.Y;

                Vertex v = new Vertex(positions[vertexIndex], uv);

                if (index1 == uint.MaxValue) {
                    index1 = mesh.AddVertex(v);
                } else if (index2 == uint.MaxValue) {
                    index2 = mesh.AddVertex(v);
                } else {
                    uint index3 = mesh.AddVertex(v);
                    mesh.MakeTriangle(index1, index2, index3);
                    index2 = index3;
                }
            }
        }

        private static void ParseTexCoord(List<Vector2> textureCoords, StringIterator lineIter) {
            textureCoords.Add(new Vector2(
                float.Parse(lineIter.GetNext()),
                float.Parse(lineIter.GetNext())
            ));
        }
    }
}

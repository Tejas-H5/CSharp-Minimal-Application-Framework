//using OpenTK.Mathematics;
//using System;
//using System.Collections.Generic;

//namespace MinimalAF.Rendering {
//    public enum ObjFileLineType {
//        Vertex, UV, Triangle
//    };

//    public struct ObjFileLine {
//        public ObjFileLineType Type;
//        public Vector3 Vertex;
//        public Vector2 UV;
//        public int V1, V2, V3;
//    }


//    public class MeshParser {
//        class ParserContext {
//            public int Pos = 0;
//            public string Text = "";

//            public string NextLine() {
//                int start = Pos;
//                Pos = Text.IndexOf('\n', start + 1);
//                return Text.Substring(start, Pos);
//            }

//            public string NextWord() {
//                int start = Pos;
//                Pos = Text.IndexOfAny(new char[] { ' ', '\n' }, start + 1);
//                return Text.Substring(start, Pos).Trim();
//            }

//        }

//        /// <summary>
//        /// Not yet complete. It just loads verts and UVS for now.
//        /// </summary>
//        public static void FromOBJ(string text, Action<ObjFileLine> parserCallback) {
//            // These are not the final vertices sent to the mesh.
//            // Each triangle needs to be seperate vertices.
//            List<Vector3> positions = new List<Vector3>();
//            List<Vector2> textureCoords = new List<Vector2>();

//            string useMtl = "";
//            string mtl = "";


//            ParserContext ctx = new ParserContext {
//                Pos = 0,
//                Text = text
//            };

//            while (ctx.Pos < text.Length) {
//                int start = ctx.Pos;

//                string startWord = ctx.NextWord();

//                if (startWord[0] == '#') {
//                    ctx.Pos = text.IndexOf('\n', start + 1);
//                    continue;
//                }

//                if (startWord == "usemtl") {
//                    useMtl = ctx.NextLine().Trim();
//                } else if (startWord == "mtllib") {
//                    mtl = ctx.NextLine().Trim();
//                } else if (startWord == "v") {
//                    ParseVertex(ctx, parserCallback);
//                } else if (startWord == "f") {
//                    ParseFace(ctx, parserCallback);
//                } else if (startWord == "vt") {
//                    ParseTexCoord(ctx, parserCallback);
//                }
//            }
//        }

//        //private static (int, int, int) CalculateDataCounts(string text) {
//        //    int vertexCount = 0;
//        //    int triangleCount = 0;
//        //    int uvCount = 0;

//        //    // lets find out how many of each there are, so we can set a capacity
//        //    foreach (var line in new StringIterator(text, "\n")) {
//        //        var lineIter = new StringIterator(line, " ");

//        //        var start = lineIter.GetNext();
//        //        if (start == "v") {
//        //            vertexCount++;
//        //        } else if (start == "f") {
//        //            lineIter.MoveNext();
//        //            lineIter.MoveNext();

//        //            // every vertex onwards will require a new triangle
//        //            while (!lineIter.MoveNext()) {
//        //                triangleCount++;
//        //            }
//        //        } else if (start == "vt") {
//        //            uvCount++;
//        //        }
//        //    }

//        //    return (vertexCount, triangleCount, uvCount);
//        //}

//        private static void ParseVertex(ParserContext ctx, Action<ObjFileLine> parserCallback) {
//            parserCallback(new ObjFileLine {
//                Type = ObjFileLineType.Vertex,
//                Vertex = new Vector3(
//                    float.Parse(ctx.NextWord()),
//                    float.Parse(ctx.NextWord()),
//                    float.Parse(ctx.NextWord())
//                )
//            });
//        }

//        private static void ParseFace(ParserContext ctx, Action<ObjFileLine> parserCallback) {
//            uint index1 = uint.MaxValue;
//            uint index2 = uint.MaxValue;

//            int lineEnd = ctx.Text.IndexOf('\n', ctx.Pos);
//            while (ctx.Pos < lineEnd) {
//                var face = ctx.NextWord().Split("/");

//                // an obj file will reuse the same vertex for different faces/UV islands,
//                // so we need to create new vertices for each point on a face (we probably dont, but idk how yet)

//                var vertexIndex = int.Parse(face[0]) - 1;
//                var textureIndex = face[1] == "" ? -1 : int.Parse(face[1]) - 1;
//                Vector2 uv = new Vector2(0, 0);
//                if (textureIndex != -1) {
//                    uv = textureCoords[textureIndex];
//                }

//                // because UVs are upside down otherwise
//                uv.Y = 1 - uv.Y;
//                V v = CreateVertex(
//                    positions[vertexIndex].X, positions[vertexIndex].Y, positions[vertexIndex].Z,
//                    uv.X, uv.Y
//                );

//                if (index1 == uint.MaxValue) {
//                    index1 = mesh.AddVertex(v);
//                } else if (index2 == uint.MaxValue) {
//                    index2 = mesh.AddVertex(v);
//                } else {
//                    uint index3 = mesh.AddVertex(v);
//                    mesh.MakeTriangle(index1, index2, index3);
//                    index2 = index3;
//                }
//            }
//        }

//        private static void ParseTexCoord(ParserContext ctx, Action<ObjFileLine> parserCallback) {

//            textureCoords.Add(new Vector2(
//                float.Parse(lineIter.GetNext()),
//                float.Parse(lineIter.GetNext())
//            ));
//        }



//        private static V CreateVertex(float x, float y, float z, float u, float v) {
//            V vert = new V();
//            vert.Position = new Vector3(x, y, z);
//            vert.UV = new Vector2(u, v);
//            return vert;
//        }
//    }
//}

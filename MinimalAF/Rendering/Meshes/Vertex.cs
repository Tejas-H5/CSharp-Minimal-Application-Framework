using OpenTK.Mathematics;
using System;
using System.Runtime.InteropServices;

namespace MinimalAF.Rendering {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex {
        public const int VERTEX_SIZE = sizeof(float) * 
            (3 + 2);

        public static readonly int[] VERTEX_COMPONENTS = {
            3, 2
        };

        public Vector3 Position;
        public Vector2 UV;

        public Vertex(float x, float y, float z)
            : this(new Vector3(x, y, z), new Vector2(x, y)) {
        }

        public Vertex(Vector3 position, Vector2 uv) {
            Position = position;
            UV = uv;
        }

        public void SetPos(Vector3 pos) {
            Position = pos;
        }

        public void SetUV(Vector2 uv) {
            UV = uv; ;
        }

        public string ToCodeString() {
            return "new Vertex(" + Position.X + ", " + Position.Y + ", " + Position.Z + ", "
                + UV.X + ", " + UV.Y + ")";
        }
    }
}

using OpenTK.Mathematics;
using System;
using System.Runtime.InteropServices;

namespace MinimalAF.Rendering {
    public interface ICodeSerializeable {
        string ToCodeString();
    }

    public interface IVertexPosition {
        Vector3 Position {
            get;set;
        }
    }

    public interface IVertexUV {
        Vector2 UV {
            get; set;
        }
    }

    public interface IVertexNormal {
        Vector3 Normal {
            get;set;
        }
    }

    public interface IVertexTangent {
        Vector3 Tangent {
            get; set;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex : IVertexPosition, IVertexUV, ICodeSerializeable {
        public Vector3 position;
        public Vector2 uv;

        public Vector3 Position {
            get => position;
            set => position = value;
        }

        public Vector2 UV {
            get => uv;
            set => uv = value;
        }

        public Vertex(float x, float y, float z)
          : this(new Vector3(x, y, z), new Vector2(x, y)) {
        }

        public Vertex(Vector3 position, Vector2 uv) {
            this.position = position;
            this.uv = uv;
        }
        public string ToCodeString() {
            return "new Vertex(" + position.X + ", " + position.Y + ", " + position.Z + ", "
                + uv.X + ", " + uv.Y + ")";
        }
    }
}

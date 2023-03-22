using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace MinimalAF.Rendering {
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex {
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
    }
}

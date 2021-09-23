namespace MinimalAF.Datatypes
{
    public struct Color4
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color4(float f, float a = 1.0f)
            : this(f, f, f, a)
        {
        }

        public static implicit operator OpenTK.Mathematics.Color4(Color4 value)
        {
            return new OpenTK.Mathematics.Color4(value.R, value.G, value.B, value.A);
        }

        public static Color4 FromRGBA(int r, int g, int b, int a)
        {
            return new Color4(r / 255f, g / 255f, b / 255f, a / 255f);
        }
    }
}

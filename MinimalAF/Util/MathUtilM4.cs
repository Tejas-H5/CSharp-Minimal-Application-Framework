using OpenTK.Mathematics;

namespace MinimalAF.Util
{
	public static class MathUtilM4
    {
        public static Matrix4 Translation(float x, float y, float z = 0)
        {
            return Matrix4.Transpose(Matrix4.CreateTranslation(x, y, z));
        }

        public static Matrix4 Rotation2D(float zAngles)
        {
            return Matrix4.CreateRotationZ(zAngles);
        }

        public static Matrix4 Rotation(float x, float y, float z)
        {
            return Matrix4.CreateRotationX(x) * Matrix4.CreateRotationY(y) * Matrix4.CreateRotationY(z);
        }

        public static Matrix4 Scale(float sf)
        {
            return Matrix4.CreateScale(sf);
        }

        public static Matrix4 Scale(float x, float y, float z)
        {
            return Matrix4.CreateScale(x, y, z);
        }
    }
}

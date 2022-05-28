using OpenTK.Mathematics;
using System;

namespace MinimalAF {
    // am too lazy to type
    public partial class Element {
        public Matrix4 Translation(float x, float y, float z) {
            return Matrix4.CreateTranslation(x, y, z);
        }

        public Matrix4 Translation(Vector3 pos) {
            return Matrix4.CreateTranslation(pos);
        }

        public Quaternion Quat(float x, float y, float z) {
            return Quaternion.FromEulerAngles(x, y, z);
        }

        public Quaternion Quat(Vector3 axis, float angle) {
            return Quaternion.FromAxisAngle(axis, angle);
        }

        public Matrix4 Rotation(float x, float y, float z) {
            return Rotation(Quat(x, y, z));
        }

        public Matrix4 Rotation(Vector3 axis, float angle) {
            return Rotation(Quat(axis, angle));
        }

        public Matrix4 Rotation(Quaternion quat) {
            return Matrix4.CreateFromQuaternion(quat);
        }

        public Matrix4 Scale(float s) {
            return Matrix4.CreateScale(s);
        }

        public Matrix4 Scale(float x, float y, float z) {
            return Matrix4.CreateScale(x, y, z);
        }

        public Matrix4 Mat4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4) {
            return new Matrix4(row1, row2, row3, row4);
        }

        public Vector4 Vec4(float x = 0, float y = 0, float z = 0, float w = 1) {
            return new Vector4(x, y, z, w);
        }

        public Vector3 Vec3(float x = 0, float y = 0, float z = 0) {
            return new Vector3(x, y, z);
        }

        public Vector2 Vec2(float x = 0, float y = 0) {
            return new Vector2(x, y);
        }

        public Color RGB(float r, float g, float b, float a = 1) {
            return Color.RGBA(r, g, b, a);
        }

        public Color HSV(float h, float s, float v, float a = 1) {
            return Color.HSVA(h, s, v, a);
        }

        public Color Greyscale(float v, float a = 1) {
            return Color.VA(v, a);
        }

        public const float DegToRad = MathF.PI / 180f;

        public void TODO(string message="") {
            throw new NotImplementedException(message);
        }
    }
}

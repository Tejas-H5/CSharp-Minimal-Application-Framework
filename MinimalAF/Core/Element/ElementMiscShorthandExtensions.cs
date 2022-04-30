using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF {
    // am too lazy to type
    public partial class Element {
        protected Matrix4 Translation(float x, float y, float z) {
            return Matrix4.CreateTranslation(x, y, z);
        }

        protected Matrix4 Translation(Vector3 pos) {
            return Matrix4.CreateTranslation(pos);
        }

        protected Quaternion Quat(float x, float y, float z) {
            return Quaternion.FromEulerAngles(x, y, z);
        }

        protected Quaternion Quat(Vector3 axis, float angle) {
            return Quaternion.FromAxisAngle(axis, angle);
        }

        protected Matrix4 Rotation(float x, float y, float z) {
            return Rotation(Quat(x, y, z));
        }

        protected Matrix4 Rotation(Vector3 axis, float angle) {
            return Rotation(Quat(axis, angle));
        }

        protected Matrix4 Rotation(Quaternion quat) {
            return Matrix4.CreateFromQuaternion(quat);
        }

        protected Matrix4 Scale(float s) {
            return Matrix4.CreateScale(s);
        }

        protected Matrix4 Scale(float x, float y, float z) {
            return Matrix4.CreateScale(x, y, z);
        }

        protected Matrix4 Mat4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4) {
            return new Matrix4(row1, row2, row3, row4);
        }

        protected Vector4 Vec4(float x, float y, float z, float w) {
            return new Vector4(x, y, z, w);
        }

        protected Vector3 Vec3(float x, float y, float z) {
            return new Vector3(x, y, z);
        }

        protected Vector2 Vec2(float x, float y) {
            return new Vector2(x, y);
        }

        protected Color4 RGB(float r, float g, float b, float a = 1) {
            return Color4.RGBA(r, g, b, a);
        }

        protected Color4 HSV(float h, float s, float v, float a = 1) {
            return Color4.HSVA(h, s, v, a);
        }

        protected Color4 Greyscale(float v, float a = 1) {
            return Color4.VA(v, a);
        }

        public const float DegToRad = MathF.PI / 180f;
    }
}

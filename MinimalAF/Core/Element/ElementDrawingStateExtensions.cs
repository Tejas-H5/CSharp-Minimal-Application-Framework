using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;

namespace MinimalAF {
    public partial class Element {
        protected void SetDrawColor(float r, float g, float b, float a) {
            ctx.SetDrawColor(r, g, b, a);
        }

        protected void SetDrawColor(Color4 col) {
            SetDrawColor(col.R, col.G, col.B, col.A);
        }

        protected Texture GetTexture() {
            return ctx.Texture.Get();
        }

        protected void SetTexture(Texture texture) {
            ctx.Texture.Set(texture);
        }

        protected void SetFont(string name, int size=12) {
            ctx.Text.SetFont(name, size);
        }

        public float GetStringHeight(string s) {
            return ctx.Text.GetStringHeight(s);
        }

        public float GetStringHeight(string s, int start, int end) {
            return ctx.Text.GetStringHeight(s, start, end);
        }

        public float GetStringWidth(string s) {
            return ctx.Text.GetStringWidth(s);
        }

        public float GetStringWidth(string s, int start, int end) {
            return ctx.Text.GetStringWidth(s, start, end);
        }

        public float GetCharWidth(char c) {
            return ctx.Text.GetWidth(c);
        }

        public float GetCharHeight(char c) {
            return ctx.Text.GetHeight(c);
        }

        public float GetCharWidth() {
            return ctx.Text.GetWidth();
        }

        public float GetCharHeight() {
            return ctx.Text.GetHeight();
        }

        public IDisposable PushMatrix(Matrix4 value) {
            return ctx.PushMatrix(RenderContext.MODEL_MATRIX, value);
        }

        public Matrix4 GetModelMatrix() {
            return ctx.GetMatrix(RenderContext.MODEL_MATRIX);
        }

        public Matrix4 GetViewMatrix() {
            return ctx.GetMatrix(RenderContext.VIEW_MATRIX);
        }

        public Matrix4 GeProjectionMatrix() {
            return ctx.GetMatrix(RenderContext.PROJECTION_MATRIX);
        }
    }
}

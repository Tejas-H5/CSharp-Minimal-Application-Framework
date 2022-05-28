using MinimalAF.Rendering;
using OpenTK.Mathematics;

namespace MinimalAF {
    public partial class Element {
        public void SetDrawColor(float r, float g, float b, float a) {
            CTX.SetDrawColor(r, g, b, a);
        }

        public void SetDrawColor(Color col) {
            SetDrawColor(col.R, col.G, col.B, col.A);
        }

        public void SetDrawColor(Color col, float alpha) {
            SetDrawColor(col.R, col.G, col.B, alpha);
        }

        public Texture GetTexture() {
            return CTX.Texture.Get();
        }

        public void SetTexture(Texture texture) {
            CTX.Texture.Set(texture);
        }

        public void SetFont(string name, int size = 16) {
            CTX.Text.SetFont(name, size);
        }

        public void SetClearColor(float r, float g, float b, float a) {
            CTX.SetClearColor(Color.RGBA(r, g, b, a));
        }

        public void SetClearColor(Color value) {
            CTX.SetClearColor(value);
        }

        public float GetStringHeight(string s) {
            return CTX.Text.GetStringHeight(s);
        }

        public float GetStringHeight(string s, int start, int end) {
            return CTX.Text.GetStringHeight(s, start, end);
        }

        public float GetStringWidth(string s) {
            return CTX.Text.GetStringWidth(s);
        }

        public float GetStringWidth(string s, int start, int end) {
            return CTX.Text.GetStringWidth(s, start, end);
        }

        public float GetCharWidth(char c) {
            return CTX.Text.GetWidth(c);
        }

        public float GetCharHeight(char c) {
            return CTX.Text.GetHeight(c);
        }

        public float GetCharWidth() {
            return CTX.Text.GetWidth();
        }

        public float GetCharHeight() {
            return CTX.Text.GetHeight();
        }

        public Matrix4 GetModelMatrix() {
            return CTX.Shader.Model;
        }

        public Matrix4 GetViewMatrix() {
            return CTX.Shader.View;
        }

        public Matrix4 GetProjectionMatrix() {
            return CTX.Shader.Projection;
        }
    }
}

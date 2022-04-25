using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System.Drawing;
using System;

namespace MinimalAF {
    public partial class Element {

        private void AssertClipping() {
#if DEBUG
            if (!Clipping) {
                throw new Exception("If you intend to draw 3D stuff in this Element, set Clipping=true in the constructor. " +
                    "This is because we will be clearing the depth buffer, but that will mess with the rendering 'order' of all the other UI." +
                    "Things that are ment to be behind things will show up in front of them etc."
                );
            }
#endif
        }

        /// <summary>
        /// Sets the sharer's model matrix
        /// </summary>
        public void SetTransform(Matrix4 transform) {
            //AssertClipping();

            CTX.SetTransform(transform);
        }

        public void SetViewOrientation(Vector3 position, Quaternion rotation) {
            //AssertClipping();

            CTX.ViewOrientation(position, rotation);
        }

        public void SetViewLookAt(Vector3 position, Vector3 target, Vector3 up) {
            //AssertClipping();

            CTX.ViewLookAt(position, target, up);
        }

        public void SetView(Matrix4 matrix) {
            CTX.Shader.SetViewMatrix(matrix);
        }

        /// <summary>
        /// <inheritdoc cref="CTX.Perspective(float, float, float, float)"/>
        /// <para>
        /// The aspect-ratio is just computed using this element's rectangle size.
        /// </para>
        /// </summary>
        public void SetProjectionPerspective(float fovy, float depthNear, float depthFar) {
            //AssertClipping();

            CTX.Perspective(
                fovy, Width / Height, depthNear, depthFar,
                2 * ScreenRect.X0 + Width - CTX.ContextWidth,
                2 * ScreenRect.Y0 + Height - CTX.ContextHeight

                // I am not sure why we need to multuply by 2 here, but it works. going to
                // keep this intuitive version with more ops here in case it helps me figure it out later
                //2 * (ScreenRect.X0 + Width / 2 - CTX.ContextWidth / 2),
                //2 * (ScreenRect.Y0 + Height / 2 - CTX.ContextHeight / 2)
            );
        }

        /// <summary>
        /// <inheritdoc cref="CTX.Perspective(float, float, float, float)"/>
        /// <para>
        /// Size is just used to multiply the aspect ratio to calculate width and height.
        /// </para>
        /// </summary>
        public void SetProjectionOrthographic(float size, float depthNear, float depthFar) {
            //AssertClipping();

            float aspect = Width / Height;
            CTX.Orthographic(
                aspect * size, (1f / aspect) * size, depthNear, depthFar,
                2 * ScreenRect.X0 + Width - CTX.ContextWidth,
                2 * ScreenRect.Y0 + Height - CTX.ContextHeight
            );
        }


        public void SetProjection(Matrix4 matrix) {
            CTX.SetProjection(matrix);
        }

        /// <summary>
        /// <inheritdoc cref="CTX.Cartesian2D(float, float, float, float)"/>
        /// </summary>
        public void SetProjectionViewCartesian(float scaleX, float scaleY, float offsetX, float offsetY) {
            CTX.Cartesian2D(scaleX, scaleY, ScreenRect.X0 + offsetX, ScreenRect.Y0 + offsetY);
        }


        /// <summary>
        /// Shorthand for RectTransform.Height * amount
        /// </summary>
        public float VH(float amount) {
            return RelativeRect.Height * amount;
        }

        /// <summary>
        /// Shorthand for RectTransform.Width * amount
        /// </summary>
        public float VW(float amount) {
            return RelativeRect.Width * amount;
        }

        public void UseFramebuffer(int index) {
            CTX.Framebuffer.Use(index);
        }

        public void UseTransparentFramebuffer(int index) {
            CTX.Framebuffer.UseTransparent(index);
        }

        public void StopUsingFramebuffer() {
            CTX.Framebuffer.StopUsing();
        }

        public Texture GetFramebufferTexture(int framebufferIndex) {
            return CTX.Framebuffer.GetTexture(framebufferIndex);
        }

        public void UseFramebufferTexture(int framebufferIndex) {
            CTX.Framebuffer.UseTexture(framebufferIndex);
        }

        public void DrawFramebuffer(int index) {
            CTX.Framebuffer.DrawFramebufferToScreen2D(index);
        }

        public void DrawFramebuffer(int index, Rect screenRect) {
            CTX.Framebuffer.DrawFramebufferToScreen2D(index, screenRect);
        }

        public void StartStencillingWhileDrawing(bool inverseStencil = false) {
            CTX.StartStencillingWhileDrawing(inverseStencil);
        }

        public void StartStencillingWithoutDrawing(bool inverseStencil = false) {
            CTX.StartStencillingWithoutDrawing(inverseStencil);
        }

        public void StartUsingStencil() {
            CTX.StartUsingStencil();
        }

        public void LiftStencil() {
            CTX.LiftStencil();
        }

        public void Arc(float xCenter, float yCenter, float radius, float startAngle, float endAngle) {
            CTX.Arc.Draw(xCenter, yCenter, radius, startAngle, endAngle);
        }

        public void Arc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
            CTX.Arc.Draw(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public void RegularPolygon(float x0, float y0, float r, int edges) {
            CTX.Circle.Draw(x0, y0, r, edges);
        }

        public void Circle(float x0, float y0, float r) {
            CTX.Circle.Draw(x0, y0, r);
        }

        public void Line(float x0, float y0, float x1, float y1, float thickness, CapType cap) {
            CTX.Line.Draw(x0, y0, x1, y1, thickness, cap);
        }

        public void Quad(Vertex v1, Vertex v2, Vertex v3, Vertex v4) {
            CTX.Quad.Draw(v1, v2, v3, v4);
        }

        public void Rect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 0, float u1 = 1, float v1 = 1) {
            CTX.Rect.Draw(x0, y0, x1, y1, u0, v0, u1, v1);
        }

        public void Rect(Rect rect, Rect uvs) {
            CTX.Rect.Draw(rect, uvs);
        }

        public void Rect(Rect rect) {
            CTX.Rect.Draw(rect);
        }

        public PointF Text(string text, float startX, float startY, HorizontalAlignment hAlign, VerticalAlignment vAlign, float scale = 1.0f) {
            return CTX.Text.Draw(text, startX, startY, hAlign, vAlign, scale);
        }

        public PointF Text(string text, float startX, float startY, float scale = 1.0f) {
            return CTX.Text.Draw(text, startX, startY, scale);
        }

        public PointF Text(string text, int start, int end, float startX, float startY, float scale) {
            return CTX.Text.Draw(text, start, end, startX, startY, scale);
        }

        public void Triangle(Vertex v1, Vertex v2, Vertex v3) {
            CTX.Triangle.Draw(v1, v2, v3);
        }

        public void ArcOutline(float thickness, float x0, float y0, float r, float startAngle, float endAngle) {
            CTX.Arc.DrawOutline(thickness, x0, y0, r, startAngle, endAngle);
        }

        public void ArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
            CTX.Arc.DrawOutline(thickness, xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public void CircleOutline(float thickness, float x0, float y0, float r, int edges) {
            CTX.Circle.DrawOutline(thickness, x0, y0, r, edges);
        }

        public void CircleOutline(float thickness, float x0, float y0, float r) {
            CTX.Circle.DrawOutline(thickness, x0, y0, r);
        }

        public void LineOutline(float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap) {
            CTX.Line.DrawOutline(outlineThickness, x0, y0, x1, y1, thickness, cap);
        }

        public void RectOutline(float thickness, Rect rect) {
            CTX.Rect.DrawOutline(thickness, rect);
        }

        public void RectOutline(float thickness, float x0, float y0, float x1, float y1) {
            CTX.Rect.DrawOutline(thickness, x0, y0, x1, y1);
        }

        public void TriangleOutline(float thickness, float x0, float y0, float x1, float y1, float x2, float y2) {
            CTX.Triangle.DrawOutline(thickness, x0, y0, x1, y1, x2, y2);
        }

        public void StartPolyLine(float x, float y, float thickness, CapType cap) {
            CTX.NLine.Begin(x, y, thickness, cap);
        }

        public void ContinuePolyLine(float x, float y, bool useAverage = true) {
            CTX.NLine.Continue(x, y, useAverage);
        }

        public void EndPolyLine(float x, float y) {
            CTX.NLine.End(x, y);
        }
    }
}

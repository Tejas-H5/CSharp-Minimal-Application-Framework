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
        protected void SetTransform(Matrix4 transform) {
            //AssertClipping();

            CTX.SetTransform(transform);
        }

        protected void SetViewOrientation(Vector3 position, Quaternion rotation) {
            //AssertClipping();

            CTX.ViewOrientation(position, rotation);
        }

        protected void SetViewLookAt(Vector3 position, Vector3 target, Vector3 up) {
            //AssertClipping();

            CTX.ViewLookAt(position, target, up);
        }

        protected void SetView(Matrix4 matrix) {
            CTX.Shader.SetViewMatrix(matrix);
        }

        /// <summary>
        /// <inheritdoc cref="CTX.Perspective(float, float, float, float)"/>
        /// <para>
        /// The aspect-ratio is just computed using this element's rectangle size.
        /// </para>
        /// </summary>
        protected void SetProjectionPerspective(float fovy, float depthNear, float depthFar) {
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
        protected void SetProjectionOrthographic(float size, float depthNear, float depthFar) {
            //AssertClipping();

            float aspect = Width / Height;
            CTX.Orthographic(
                aspect * size, (1f / aspect) * size, depthNear, depthFar,
                2 * ScreenRect.X0 + Width - CTX.ContextWidth,
                2 * ScreenRect.Y0 + Height - CTX.ContextHeight
            );
        }


        protected void SetProjection(Matrix4 matrix) {
            CTX.SetProjection(matrix);
        }

        /// <summary>
        /// <inheritdoc cref="CTX.Cartesian2D(float, float, float, float)"/>
        /// </summary>
        protected void SetViewProjectionCartesian2D(float scaleX, float scaleY, float offsetX, float offsetY) {
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

        /// <summary>
        /// This name only makes sense alonside VW which means View Width and VH wich means View Height.
        /// Not sure what VDir means, but it will behave like VW if dir is horizontal, else VH
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public float VDir(Direction dir, float amount) {
            if(dir == Direction.Left || dir == Direction.Right) {
                return VW(amount);
            } else {
                return VH(amount);
            }
        }



        protected struct DrawCallRedirector : IDisposable {
            Framebuffer previous;
            Element el;
            public DrawCallRedirector(Framebuffer fb, Element el) {
                previous = CTX.Framebuffer.Current;
                this.el = el;

                RedirectDrawCalls(fb);
            }

            private void RedirectDrawCalls(Framebuffer framebuffer) {
                if (framebuffer == null) {
                    el._screenRect = el._defaultScreenRect;
                } else {
                    el._screenRect = new Rect(0, 0, framebuffer.Width, framebuffer.Height);
                }

                CTX.Framebuffer.Use(framebuffer);

                if (framebuffer == null) {
                    el.ResetCoordinates();
                } else {
                }
            }


            public void Dispose() {
                RedirectDrawCalls(previous);
            }
        }

        /// <summary>
        /// Starts drawing to a framebuffer. You should ideally use it like this:
        /// <code>
        /// using(RedirectDrawCalls(fb)) {
        ///     ... draw to fb here
        /// }
        /// 
        /// ... draw stuff to the screen here
        /// </code>
        /// Otherwise you should use it like this:
        /// <code>
        /// RedirectDrawCalls(fb1)
        /// ... code that renders to fb1
        /// RedirectDrawCalls(fb2)
        /// ... code that renders to fb2, etc.
        /// 
        /// RedirectDrawCalls(null)
        /// </code>
        /// </summary>
        protected DrawCallRedirector RedirectDrawCalls(Framebuffer framebuffer) {
            return new DrawCallRedirector(framebuffer, this);
        }

        protected void StartStencillingWhileDrawing(bool inverseStencil = false) {
            CTX.StartStencillingWhileDrawing(inverseStencil);
        }

        protected void StartStencillingWithoutDrawing(bool inverseStencil = false) {
            CTX.StartStencillingWithoutDrawing(inverseStencil);
        }

        protected void StartUsingStencil() {
            CTX.StartUsingStencil();
        }

        protected void LiftStencil() {
            CTX.LiftStencil();
        }

        protected void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle) {
            CTX.Arc.Draw(xCenter, yCenter, radius, startAngle, endAngle);
        }

        protected void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
            CTX.Arc.Draw(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        protected void DrawRegularPolygon(float x0, float y0, float r, int edges) {
            CTX.Circle.Draw(x0, y0, r, edges);
        }

        protected void DrawCircle(float x0, float y0, float r) {
            CTX.Circle.Draw(x0, y0, r);
        }

        protected void DrawLine(float x0, float y0, float x1, float y1, float thickness, CapType cap) {
            CTX.Line.Draw(x0, y0, x1, y1, thickness, cap);
        }

        protected void DrawQuad(Vertex v1, Vertex v2, Vertex v3, Vertex v4) {
            CTX.Quad.Draw(v1, v2, v3, v4);
        }

        protected void DrawRect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 0, float u1 = 1, float v1 = 1) {
            CTX.Rect.Draw(x0, y0, x1, y1, u0, v0, u1, v1);
        }

        protected void DrawRect(Rect rect, Rect uvs) {
            CTX.Rect.Draw(rect, uvs);
        }

        protected void DrawRect(Rect rect) {
            CTX.Rect.Draw(rect);
        }

        protected PointF DrawText(string text, float startX, float startY, HorizontalAlignment hAlign, VerticalAlignment vAlign, float scale = 1.0f) {
            return CTX.Text.Draw(text, startX, startY, hAlign, vAlign, scale);
        }

        protected PointF DrawText(string text, float startX, float startY, float scale = 1.0f) {
            return CTX.Text.Draw(text, startX, startY, scale);
        }

        protected PointF DrawText(string text, int start, int end, float startX, float startY, float scale) {
            return CTX.Text.Draw(text, start, end, startX, startY, scale);
        }

        protected void DrawTriangle(Vertex v1, Vertex v2, Vertex v3) {
            CTX.Triangle.Draw(v1, v2, v3);
        }

        protected void DrawArcOutline(float thickness, float x0, float y0, float r, float startAngle, float endAngle) {
            CTX.Arc.DrawOutline(thickness, x0, y0, r, startAngle, endAngle);
        }

        protected void DrawArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
            CTX.Arc.DrawOutline(thickness, xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        protected void DrawCircleOutline(float thickness, float x0, float y0, float r, int edges) {
            CTX.Circle.DrawOutline(thickness, x0, y0, r, edges);
        }

        protected void DrawCircleOutline(float thickness, float x0, float y0, float r) {
            CTX.Circle.DrawOutline(thickness, x0, y0, r);
        }

        protected void DrawLineOutline(float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap) {
            CTX.Line.DrawOutline(outlineThickness, x0, y0, x1, y1, thickness, cap);
        }

        protected void DrawRectOutline(float thickness, Rect rect) {
            CTX.Rect.DrawOutline(thickness, rect);
        }

        protected void DrawRectOutline(float thickness, float x0, float y0, float x1, float y1) {
            CTX.Rect.DrawOutline(thickness, x0, y0, x1, y1);
        }

        protected void DrawTriangleOutline(float thickness, float x0, float y0, float x1, float y1, float x2, float y2) {
            CTX.Triangle.DrawOutline(thickness, x0, y0, x1, y1, x2, y2);
        }

        protected void StartPolyLine(float x, float y, float thickness, CapType cap) {
            CTX.NLine.Begin(x, y, thickness, cap);
        }

        protected void ContinuePolyLine(float x, float y, bool useAverage = true) {
            CTX.NLine.Continue(x, y, useAverage);
        }

        protected void EndPolyLine(float x, float y) {
            CTX.NLine.End(x, y);
        }

        protected void Clear() {
            CTX.Clear();
        }

        protected void Clear(Color4 color) {
            CTX.Clear(color);
        }
    }
}

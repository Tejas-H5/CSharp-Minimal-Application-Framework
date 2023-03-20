using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace MinimalAF {
    public struct FrameworkContext {
        /// <summary>
        /// The rectangle where stuff will be drawn
        /// </summary>
        public Rect Rect;
        /// <summary>
        /// The window we are running in
        /// </summary>
        ProgramWindow window;

        /// <summary>
        /// Should we enable scissor test and set the scissor rect to Rect when we call Use?
        /// </summary>
        public bool RectShouldClipOverflow;


        internal FrameworkContext(Rect rect, ProgramWindow window, bool isClipping) {
            Rect = rect;
            this.window = window;
            this.RectShouldClipOverflow = isClipping;
        }

        public FrameworkContext WithRect(Rect newRect, bool clipOverflow = false) {
            var ctx = this;
            ctx.Rect = newRect;
            ctx.RectShouldClipOverflow = clipOverflow;

            return ctx;
        }

        public FrameworkContext Width(float newWidth, float pivot) {
            return WithRect(Rect.ResizedWidth(newWidth, pivot));
        }

        public FrameworkContext Height(float newHeight, float pivot) {
            return WithRect(Rect.ResizedHeight(newHeight, pivot));
        }

        public FrameworkContext Inset(float amount) {
            return WithRect(Rect.Inset(amount));
        }


        /// <summary>
        /// The render context is the way that we can recursively render components inside one another
        /// with minimal overhead. 
        /// 
        /// But you will need to call Use() on the context in order for it to work as expected. E.G:
        /// 
        /// <code>
        /// // (Your component's render method)
        /// void Render(FrameworkContext ctx) {
        ///     // ideally, whoever is calling this Render(ctx) method has already called Use() on their context by this point.
        ///     // It is up to your codebase's conventions though. Maybe you have a convention where people have to do
        ///     // ctx.Use() for the top of every render method. Either way, it needs to be somewhere.
        /// }
        /// 
        /// // Somewhere else that is calling your component's render method
        /// void Render(FrameworkContext ctx) {
        ///     component.Render(
        ///         ctx.Width(ctx.VW * 0.5, pivot=0).Use()
        ///     );
        /// }
        /// </code>
        /// </summary>
        public FrameworkContext Use() {
            CTX.Texture.Use(null);
            if (RectShouldClipOverflow) {
                CTX.CurrentClippingRect = Rect;
            } else {
                CTX.DisableClipping();
            }

            SetModel(Matrix4.Identity);
            SetProjectionCartesian2D(1, 1, 0, 0);
            return this;
        }



        public float VW => Rect.Width;
        public float VH => Rect.Height;


        public ProgramWindow Window => window;

        public Texture CurrentFontTexture => CTX.CurrentFontTexture;


        // ---- Drawing
        public void SetDrawColor(float r, float g, float b, float a) { CTX.SetDrawColor(r, g, b, a); }
        public void SetDrawColor(Color col) { SetDrawColor(col.R, col.G, col.B, col.A); }
        public void SetDrawColor(Color col, float alpha) { SetDrawColor(col.R, col.G, col.B, alpha); }
        public Texture GetTexture() { return CTX.Texture.Get(); }
        public void SetTexture(Texture texture) { CTX.Texture.Use(texture); }
        /// <summary>
        /// Note: Try to use a constant font and font size in as many places as you can, because currently we are creating a new font atlas
        /// for each new font name and font size that you specify
        /// </summary>
        public void SetFont(string name, int size = 16) { CTX.Text.SetFont(name, size); }
        public void SetClearColor(float r, float g, float b, float a) { CTX.SetClearColor(Color.RGBA(r, g, b, a)); }
        public void SetClearColor(Color value) { CTX.SetClearColor(value); }
        public float GetStringHeight(string s) { return CTX.Text.GetStringHeight(s); }
        public float GetStringHeight(string s, int start, int end) { return CTX.Text.GetStringHeight(s, start, end); }
        public float GetStringWidth(string s) { return CTX.Text.GetStringWidth(s); }
        public float GetStringWidth(string s, int start, int end) { return CTX.Text.GetStringWidth(s, start, end); }
        public float GetCharWidth(char c) { return CTX.Text.GetWidth(c); }
        public float GetCharHeight(char c) { return CTX.Text.GetHeight(c); }
        public float GetCharWidth() { return CTX.Text.GetWidth(); }
        public float GetCharHeight() { return CTX.Text.GetHeight(); }
        public Matrix4 GetModelMatrix() { return CTX.Shader.Model; }
        public Matrix4 GetViewMatrix() { return CTX.Shader.View; }
        public Matrix4 GetProjectionMatrix() { return CTX.Shader.Projection; }

        /// <summary>
        /// Clockwise-winding vertices are drawn, anti-clockwise are not
        /// </summary>
        public void SetBackfaceCulling(bool state) {
            CTX.SetBackfaceCulling(state);
        }

        /// <summary>
        /// Sets the shader's model matrix
        /// </summary>
        public void SetModel(Matrix4 transform) {
            CTX.SetModel(transform);
        }

        public void SetViewOrientation(Vector3 position, Quaternion rotation) {
            CTX.ViewOrientation(position, rotation);
        }

        public void SetViewLookAt(Vector3 position, Vector3 target, Vector3 up) {
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
                fovy, VW / VH, depthNear, depthFar,
                2 * Rect.X0 + VW - window.Width,
                2 * Rect.Y0 + VH - window.Height

            // I am not sure why we need to multuply by 2 here, but it works. going to
            // keep this intuitive version with more ops here in case it helps me figure it out later
            //2 * (Rect.X0 + Width / 2 - window.Width / 2),
            //2 * (Rect.Y0 + Height / 2 - window.Height / 2)
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

            float aspect = VW / VH;
            CTX.Orthographic(
                aspect * size, (1f / aspect) * size, depthNear, depthFar,
                2 * Rect.X0 + VW - window.Width,
                2 * Rect.Y0 + VH - window.Height
            );
        }


        public void SetProjection(Matrix4 matrix) {
            CTX.SetProjection(matrix);
        }

        /// <summary>
        /// <inheritdoc cref="CTX.Cartesian2D(float, float, float, float)"/>
        /// </summary>
        public void SetProjectionCartesian2D(float scaleX, float scaleY, float offsetX, float offsetY) {
            CTX.Cartesian2D(scaleX, scaleY, Rect.X0 + offsetX, Rect.Y0 + offsetY);
        }               

        /// <summary>
        /// Starts drawing to a framebuffer.
        /// </summary>
        public void UseFramebuffer(Framebuffer framebuffer) {
            CTX.Framebuffer.Use(framebuffer);
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

        public void StopUsingStencil() {
            CTX.LiftStencil();
        }

        public void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle) {
            CTX.Arc.Draw(xCenter, yCenter, radius, startAngle, endAngle);
        }

        public void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
            CTX.Arc.Draw(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public void DrawRegularPolygon(float x0, float y0, float r, int edges) {
            CTX.Circle.Draw(x0, y0, r, edges);
        }

        public void DrawRegularPolygonOutline(float thickness, float x0, float y0, float r, int edges) {
            CTX.Circle.DrawOutline(thickness, x0, y0, r, edges);
        }

        public void DrawCircle(float x0, float y0, float r) {
            CTX.Circle.Draw(x0, y0, r);
        }

        public void DrawLine(float x0, float y0, float x1, float y1, float thickness, CapType cap = CapType.None) {
            CTX.Line.Draw(x0, y0, x1, y1, thickness, cap);
        }

        public void DrawQuad(Vertex v1, Vertex v2, Vertex v3, Vertex v4) {
            CTX.Quad.Draw(v1, v2, v3, v4);
        }

        public void DrawRect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 0, float u1 = 1, float v1 = 1) {
            CTX.Rect.Draw(x0, y0, x1, y1, u0, v0, u1, v1);
        }

        public void DrawRect(Rect rect, Rect uvs) {
            CTX.Rect.Draw(rect, uvs);
        }

        public void DrawRect(Rect rect) {
            CTX.Rect.Draw(rect);
        }

        public Vector2 DrawChar(char character, float startX, float startY, float scale = 1.0f) {
            return CTX.Text.DrawChar(character, startX, startY, scale);
        }

        public Vector2 DrawText(string text, float startX, float startY, HAlign hAlign, VAlign vAlign, float scale = 1.0f) {
            return CTX.Text.Draw(text, startX, startY, hAlign, vAlign, scale);
        }

        public Vector2 DrawText(string text, float startX, float startY, float scale = 1.0f) {
            return CTX.Text.Draw(text, startX, startY, scale);
        }

        public Vector2 DrawText(string text, int start, int end, float startX, float startY, float scale) {
            return CTX.Text.Draw(text, start, end, startX, startY, scale);
        }

        public void DrawTriangle(Vertex v1, Vertex v2, Vertex v3) {
            CTX.Triangle.Draw(v1, v2, v3);
        }

        public void DrawArcOutline(float thickness, float x0, float y0, float r, float startAngle, float endAngle) {
            CTX.Arc.DrawOutline(thickness, x0, y0, r, startAngle, endAngle);
        }

        public void DrawArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
            CTX.Arc.DrawOutline(thickness, xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public void DrawCircleOutline(float thickness, float x0, float y0, float r, int edges) {
            CTX.Circle.DrawOutline(thickness, x0, y0, r, edges);
        }

        public void DrawCircleOutline(float thickness, float x0, float y0, float r) {
            CTX.Circle.DrawOutline(thickness, x0, y0, r);
        }

        public void DrawLineOutline(float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap) {
            CTX.Line.DrawOutline(outlineThickness, x0, y0, x1, y1, thickness, cap);
        }

        public void DrawRectOutline(float thickness, Rect rect) {
            CTX.Rect.DrawOutline(thickness, rect);
        }

        public void DrawRectOutline(float thickness, float x0, float y0, float x1, float y1) {
            CTX.Rect.DrawOutline(thickness, x0, y0, x1, y1);
        }

        public void DrawTriangleOutline(float thickness, float x0, float y0, float x1, float y1, float x2, float y2) {
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

        public void StartNGon(Vector2 vec, int numVerts) {
            StartNGon(vec.X, vec.Y, numVerts);
        }

        public void StartNGon(float x, float y, int numVerts) {
            CTX.NGon.Begin(x, y, numVerts);
        }

        public void ContinueNGon(Vector2 vec) {
            ContinueNGon(vec.X, vec.Y);
        }

        public void ContinueNGon(float x, float y) {
            CTX.NGon.Continue(x, y);
        }

        public void EndNGon() {
            CTX.NGon.End();
        }

        public void Clear() {
            CTX.Clear();
        }

        public void Clear(Color color) {
            CTX.Clear(color);
        }

        // -- Inputs
        public List<RepeatableKeyboardInput> RepeatableKeysInput => window.RepeatableKeysInput;
        public bool KeyJustPressed(KeyCode key) { return window.KeyJustPressed(key); }
        public bool KeyJustReleased(KeyCode key) { return window.KeyJustReleased(key); }
        public bool KeyWasDown(KeyCode key) { return window.KeyWasDown(key); }
        public bool KeyIsDown(KeyCode key) { return window.KeyIsDown(key); }
        public bool MouseIsOver(Rect rect) { return Intersections.IsInsideRect(MouseX, MouseY, rect); }
        public bool MouseButtonJustPressed(MouseButton b) { return window.MouseButtonJustPressed(b); }
        public bool MouseButtonJustReleased(MouseButton b) { return window.MouseButtonJustReleased(b); }
        public bool MouseButtonIsDown(MouseButton b) { return window.MouseButtonIsDown(b); }
        public bool MouseButtonWasDown(MouseButton b) { return window.MouseButtonWasDown(b); }
        public float MouseWheelNotches => window.MouseWheelNotches;
        public float MouseX => window.MouseX - Rect.X0;
        public float MouseY => window.MouseY - Rect.Y0;
        public float MouseXDelta => window.MouseXDelta;
        public float MouseYDelta => window.MouseYDelta;
    }

    public interface IRenderable {
        void Render(FrameworkContext ctx);
    }
}

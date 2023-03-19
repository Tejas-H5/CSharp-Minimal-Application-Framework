using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;

namespace MinimalAF.Rendering {
    /// <summary>
    /// CTX is short for RenderContext.
    /// It no longer needs to be short, we can rename to RenderContext later, or even turn this into an object and then a 
    /// singleton
    /// </summary>
    public static class CTX {
        //Here solely for the SwapBuffers function
        private static IGLFWGraphicsContext glContext;

        private static int contextWidth, contextHeight;
        internal static int ScreenWidth, ScreenHeight;
        // The context width and height depend on the current framebuffer that is being used
        // (And not the screen size, which would be ScreenWidth and ScreenHeight
        public static int ContextWidth {
            get => contextWidth;
            set => contextWidth = value;
        }

        public static int ContextHeight {
            get => contextHeight;
            set => contextHeight = value;
        }

        public static float Current2DDepth = 0;

        class VertexCreator {
            public VertexCreator() {
            }

            public Vertex New(float x, float y, float u, float v) {
                return new Vertex(
                    new Vector3(x, y, CTX.Current2DDepth - 1.0f), new Vector2(u, v)
                );
            }
        }


        private static Rect currentClippingRect;
        internal static Rect CurrentClippingRect {
            get => currentClippingRect;
            set {
                currentClippingRect = value;

                Flush();

                GL.Scissor(
                    (int)currentClippingRect.X0,
                    (int)currentClippingRect.Y0,
                    (int)currentClippingRect.Width,
                    (int)currentClippingRect.Height
                );

                GL.Enable(EnableCap.ScissorTest);
            }
        }

        /// <summary>
        /// If you want to enable clipping, just set CurrentClippingRect
        /// </summary>
        internal static void DisableClipping() {
            Flush();
            GL.Disable(EnableCap.ScissorTest);
        }

        internal static Color ClearColor;

        //Composition
        internal static TriangleDrawer<Vertex> Triangle => s_imDrawer.Triangle;
        internal static QuadDrawer<Vertex> Quad => s_imDrawer.Quad;
        internal static RectangleDrawer<Vertex> Rect => s_imDrawer.Rect;
        internal static NGonDrawer<Vertex> NGon => s_imDrawer.NGon;
        internal static PolyLineDrawer<Vertex> NLine => s_imDrawer.NLine;
        internal static ArcDrawer<Vertex> Arc => s_imDrawer.Arc;
        internal static CircleDrawer<Vertex> Circle => s_imDrawer.Circle;
        internal static LineDrawer<Vertex> Line => s_imDrawer.Line;
        internal static TextDrawer<Vertex> Text => s_textDrawer;
        internal static TextureManager Texture => s_textureManager;
        internal static FramebufferManager Framebuffer => s_framebufferManager;
        public static ShaderManager Shader => s_shaderManager;

        private static TextDrawer<Vertex> s_textDrawer;
        private static MeshOutputStream<Vertex> s_meshOutputStream;
        private static ImmediateMode2DDrawer<Vertex> s_imDrawer;
        private static readonly VertexCreator s_vertexCreator = new VertexCreator();

        private static TextureManager s_textureManager;
        private static FramebufferManager s_framebufferManager;
        private static ShaderManager s_shaderManager;
        private static InternalShader s_internalShader;

        internal static Texture InternalFontTexture => s_textDrawer.ActiveFont.FontTexture;

        public static int TimesVertexThresholdReached {
            get => s_meshOutputStream.TimesVertexThresholdReached;
            set => s_meshOutputStream.TimesVertexThresholdReached = value;
        }

        public static int TimesIndexThresholdReached {
            get => s_meshOutputStream.TimesIndexThresholdReached;
            set => s_meshOutputStream.TimesIndexThresholdReached = value;
        }

        public static float VertexToIndexRatio => (float)TimesVertexThresholdReached / (float)TimesIndexThresholdReached;

        internal static void SetScreenWidth(int width, int height) {
            contextWidth = ScreenWidth = width;
            contextHeight = ScreenHeight = height;
        }

        internal static void Init(IGLFWGraphicsContext context) {
            s_meshOutputStream = new MeshOutputStream<Vertex>(8 * 4096, 8 * 4096);
            s_imDrawer = new ImmediateMode2DDrawer<Vertex>(s_meshOutputStream);
            s_textDrawer = new TextDrawer<Vertex>();

            glContext = context;

            s_internalShader = new InternalShader();
            s_shaderManager = new ShaderManager();
            s_shaderManager.UseShader(s_internalShader);

            s_textureManager = new TextureManager();
            s_framebufferManager = new FramebufferManager();

            ClearColor = Color.VA(0, 0);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Equal, 0, 0xFF);

            GL.Enable(EnableCap.DepthTest);

            Console.WriteLine("Context initialized. OpenGL info: ");

            string version = GL.GetString(StringName.Version);
            string vendor = GL.GetString(StringName.Vendor);
            Console.WriteLine("Vendor: " + vendor + ", Version: " + version);
        }

        internal static Color GetClearColor() {
            return ClearColor;
        }

        internal static void SetClearColor(Color color) {
            ClearColor = color;
        }

        internal static void Clear() {
            GL.StencilMask(1);
            GL.ClearColor(ClearColor.R, ClearColor.G, ClearColor.B, ClearColor.A);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        internal static void Clear(Color col) {
            Color prev = ClearColor;
            ClearColor = col;

            Clear();

            ClearColor = prev;
        }


        internal static void Flush() {
            s_meshOutputStream.Flush();
        }

        internal static void SwapBuffers() {
            Flush();
            s_framebufferManager.Use(null);
            s_shaderManager.SetModelMatrix(Matrix4.Identity);

            glContext.SwapBuffers();


#if DEBUG
            TimesVertexThresholdReached = 0;
            TimesIndexThresholdReached = 0;
#endif

        }


        internal static void SetViewport(Rect screenRect) {
            screenRect = screenRect.Rectified();

            GL.Viewport((int)screenRect.X0, (int)screenRect.Y0, (int)screenRect.Width, (int)screenRect.Height);
        }

        /// <summary>
        /// <para>
        /// Intitializes a 2D coordinate system with (x, y) in virtual coordinates mapping to (offsetX + scaleX * x, offsetY + scaleY * y) in screen coordinates.
        /// </para>
        /// <para>
        /// The horizontal axis is rightwards, and the vertical axis is upwards
        /// </para>
        /// <para>
        /// This method sets the projection and vew matrices.
        /// </para>
        /// </summary>
        internal static void Cartesian2D(float scaleX = 1, float scaleY = 1, float offsetX = 0, float offsetY = 0) {
            Flush();

            float width = scaleX * ContextWidth;
            float height = scaleY * ContextHeight;

            Matrix4 viewMatrix = Matrix4.CreateTranslation(offsetX - width / 2, offsetY - height / 2, 0);
            Matrix4 projectionMatrix = Matrix4.CreateScale(2.0f / width, 2.0f / height, 1);

            s_shaderManager.SetViewMatrix(viewMatrix);
            SetProjection(projectionMatrix);

            GL.DepthFunc(DepthFunction.Lequal);
        }


        internal static void ViewLookAt(Vector3 position, Vector3 target, Vector3 up) {
            Matrix4 lookAt = Matrix4.LookAt(position, target, up);

            s_shaderManager.SetViewMatrix(lookAt);
        }

        internal static void ViewOrientation(Vector3 position, Quaternion rotation) {
            Matrix4 orienation = Matrix4.CreateTranslation(-position);
            orienation.Transpose();
            orienation *= Matrix4.CreateFromQuaternion(rotation.Inverted());

            s_shaderManager.SetViewMatrix(orienation);
        }


        /// <summary>
        /// <inheritdoc cref="Matrix4.CreatePerspectiveFieldOfView(float, float, float, float)"/>
        /// <para>
        /// And then assign it to the shader's projection matrix.
        /// </para>
        /// </summary>
        internal static void Perspective(float fovy, float aspect, float depthNear, float depthFar, float centerX = 0, float centerY = 0) {
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, depthNear, depthFar);
            perspective = perspective * Matrix4.CreateTranslation(centerX / ContextWidth, centerY / ContextHeight, 0);

            SetProjection(perspective);
        }

        /// <summary>
        /// <inheritdoc cref="Matrix4.CreateOrthographic(float, float, float, float)"/>
        /// <para>
        /// And then assign it to the shader's projection matrix.
        /// </para>
        /// </summary>
        internal static void Orthographic(float width, float height, float depthNear, float depthFar, float centerX = 0, float centerY = 0) {
            Matrix4 ortho = Matrix4.CreateOrthographic(width, height, depthNear, depthFar);
            ortho = ortho * Matrix4.CreateTranslation(centerX / ContextWidth, centerY / ContextHeight, 0);

            SetProjection(ortho);
        }

        internal static void SetProjection(Matrix4 matrix) {
            s_shaderManager.SetProjectionMatrix(matrix);
        }

        internal static void SetBackfaceCulling(bool onOrOff) {
            if(onOrOff) {
                GL.Enable(EnableCap.CullFace);
            } else {
                GL.Disable(EnableCap.CullFace);
            }
        }

        internal static void SetModel(Matrix4 matrix) {
            s_shaderManager.SetModelMatrix(matrix);
        }

        internal static void SetDrawColor(float r, float g, float b, float a) {
            Color col = Color.RGBA(r, g, b, a);
            SetDrawColor(col);
        }

        internal static void SetDrawColor(Color col) {
            if (s_internalShader.Color == col)
                return;

            Flush();

            s_internalShader.Color = col;
        }


        /// <summary>
        /// Clears and enables the stencil buffer, doesn't disable colour writes.
        /// </summary>
        /// <param name="inverseStencil">If this parameter is set to true, the buffer will be cleared to 1s and
        /// we will be drawing 0s to the stencil buffer, and vice versa.</param>
        internal static void StartStencillingWhileDrawing(bool inverseStencil = false) {
            StartStencilling(true, inverseStencil);
        }

        /// <summary>
        /// Clears and enables the stencil buffer, and then disables colour writes
        /// </summary>
        /// <param name="inverseStencil">If this parameter is set to true, the buffer will be cleared to 1s and
        /// we will be drawing 0s to the stencil buffer, and vice versa.</param>
        internal static void StartStencillingWithoutDrawing(bool inverseStencil = false) {
            StartStencilling(false, inverseStencil);
        }

        private static void StartStencilling(bool canDraw, bool inverseStencil) {
            Flush();

            if (!canDraw) {
                GL.ColorMask(false, false, false, false);
            }

            if (inverseStencil) {
                GL.ClearStencil(1);
            } else {
                GL.ClearStencil(0);
            }

            GL.StencilMask(1);
            GL.Clear(ClearBufferMask.StencilBufferBit);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

            if (inverseStencil) {
                GL.StencilFunc(StencilFunction.Always, 0, 0);
            } else {
                GL.StencilFunc(StencilFunction.Always, 1, 1);
            }
        }


        /// <summary>
        /// Disables writing to the stencil buffer, enables writing to the color buffer.
        /// 
        /// Pixels in the stencil buffer that were set to 1 will not be drawn to.
        /// </summary>
        internal static void StartUsingStencil() {
            Flush();

            GL.ColorMask(true, true, true, true);
            GL.StencilFunc(StencilFunction.Notequal, 1, 1);
            GL.StencilMask(0);
        }

        internal static void LiftStencil() {
            Flush();

            GL.Disable(EnableCap.StencilTest);
        }


        /// <summary>
        /// I hate this part of C# tbh.
        /// </summary>
        #region IDisposable Support
        private static bool disposed = false;
        public static void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                s_meshOutputStream.Dispose();
                s_internalShader.Dispose();
                s_textDrawer.Dispose();
                s_textureManager.Dispose();

                TextureMap.UnloadAll();

                s_framebufferManager.Dispose();

                Texture.Use(null);
                // TODO: set large fields to null.

                Console.WriteLine("Context disposed");
                disposed = true;
            }
        }
        #endregion
    }
}

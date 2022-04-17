using MinimalAF.Rendering.ImmediateMode;
using MinimalAF.Rendering.Text;
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
        public const int MODEL_MATRIX = 0;
        public const int VIEW_MATRIX = 1;
        public const int PROJECTION_MATRIX = 2;
        public const int NUM_MATRICES = 3;

        //Here solely for the SwapBuffers function
        private static IGLFWGraphicsContext glContext;

        private static int contextWidth, contextHeight;

        public static float Current2DDepth = 0;

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

        internal static int ContextWidth {
            get => contextWidth;
            set => contextWidth = value;
        }

        internal static int ContextHeight {
            get => contextHeight;
            set => contextHeight = value;
        }

        private static bool disposed; // To detect redundant calls to Dispose()
        internal static Color4 ClearColor;

        //Composition
        internal static TriangleDrawer Triangle => triangle;
        internal static QuadDrawer Quad => quad;
        internal static RectangleDrawer Rect => rect;
        internal static NGonDrawer NGon => nGon;
        internal static PolyLineDrawer NLine => nLine;
        internal static ArcDrawer Arc => arc;
        internal static CircleDrawer Circle => circle;
        internal static LineDrawer Line => line;
        internal static TextDrawer Text => textDrawer;
        internal static TextureManager Texture => textureManager;
        internal static FramebufferManager Framebuffer => framebufferManager;

        internal static TriangleDrawer triangle;
        internal static QuadDrawer quad;
        internal static RectangleDrawer rect;
        internal static NGonDrawer nGon;
        internal static PolyLineDrawer nLine;
        internal static ArcDrawer arc;
        internal static CircleDrawer circle;
        internal static LineDrawer line;
        private static TextDrawer textDrawer;
        private static MeshOutputStream meshOutputStream;
        private static ImmediateModeShader solidShader;
        private static TextureManager textureManager;
        private static FramebufferManager framebufferManager;

        public static FontAtlasTexture InternalFontAtlas {
            get => textDrawer.ActiveFont;
        }

        internal static Matrix4 GetMatrix(int matrix) {
            return solidShader.GetMatrix(matrix);
        }

        internal static void SetMatrix(int matrix, Matrix4 value) {
            Flush();
            solidShader.SetMatrix(matrix, value);
        }

        internal struct MatrixPush : IDisposable {
            Matrix4 oldValue;
            int matrix;

            internal MatrixPush(int matrix, Matrix4 value) {
                this.oldValue = GetMatrix(matrix);
                this.matrix = matrix;

                SetMatrix(matrix, value);
            }

            public void Dispose() {
                SetMatrix(matrix, oldValue);
            }
        }

        /// <summary>
        /// There is no corresponding PopMatrix method.
        /// Instead, put this inside a using statement.
        ///
        /// </summary>
        internal static IDisposable PushMatrix(int matrix, Matrix4 value) {
            return new MatrixPush(matrix, value);
        }


        internal static void Init(IGLFWGraphicsContext context) {
            InitDrawers();

            glContext = context;
            solidShader = new ImmediateModeShader();
            solidShader.Use();
            textureManager = new TextureManager();
            framebufferManager = new FramebufferManager();

            ClearColor = Color4.VA(0, 0);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Equal, 0, 0xFF);

            GL.Enable(EnableCap.DepthTest);
            //TODO: change this to DepthFunction.Less for viewport3D
            GL.DepthFunc(DepthFunction.Lequal);

            disposed = false; // To detect redundant calls to Dispose()


            Console.WriteLine("Context initialized");
        }

        private static void InitDrawers() {
            meshOutputStream = new MeshOutputStream(4096, 4 * 4096);

            triangle = new TriangleDrawer(meshOutputStream);
            nGon = new NGonDrawer(meshOutputStream);
            quad = new QuadDrawer(meshOutputStream);
            nLine = new PolyLineDrawer(meshOutputStream);

            line = new LineDrawer();
            arc = new ArcDrawer(circleEdgeLength: 5, maxCircleEdgeCount: 32);
            rect = new RectangleDrawer();
            circle = new CircleDrawer();
            textDrawer = new TextDrawer();
        }

        internal static Color4 GetClearColor() {
            return ClearColor;
        }

        internal static void SetClearColor(Color4 color) {
            ClearColor = color;
        }

        internal static void Clear() {
            GL.StencilMask(1);
            GL.ClearColor(ClearColor.R, ClearColor.G, ClearColor.B, ClearColor.A);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        internal static void Clear(Color4 col) {
            ClearColor = col;
            Clear();
        }


        internal static void Flush() {
            meshOutputStream.Flush();
        }

        internal static void SwapBuffers() {
            Flush();
            framebufferManager.StopUsing();
            SetMatrix(MODEL_MATRIX, Matrix4.Identity);
            glContext.SwapBuffers();
        }


        /// <summary>
        /// Initializes a rectangle within the window where UI can draw things. 
        /// Usefull when you need a particular portion of the screen to render a scene or something.
        /// Think 3D mesh editing view in Blender, or the game view in Unity game engine.
        /// 
        /// Follow this up with a camera initialization method to  initialize a coordinate system to draw in.
        /// 
        /// <para>
        /// Existing coordinate system methods:
        /// </para>
        /// 
        /// <code>
        ///		<see cref="Cartesian2D"/> <br/>
        /// </code>
        /// 
        /// <para>
        /// I will eventually be making:
        /// </para>
        /// <code>
        ///		void Orthographic3D(Matrix4 cameraPosition, ...cameraSettings) {}
        ///		void Perspective3D(Matrix4 cameraPosition, ...cameraSettings) {}
        /// </code>
        /// </summary>
        internal static void SetViewport(Rect screenRect) {
            screenRect = screenRect.Rectify();

            GL.Viewport((int)screenRect.X0, (int)screenRect.Y0, (int)screenRect.Width, (int)screenRect.Height);
            CurrentClippingRect = screenRect;
        }

        internal static void SetScreenRect(Rect screenRect) {
            Cartesian2D(ContextWidth, ContextHeight, screenRect.X0, screenRect.Y0);
        }

        internal static void Cartesian2D(float width, float height, float offsetX = 0, float offsetY = 0) {
            Matrix4 projectionMatrix = Matrix4.Identity;

            Matrix4 viewMatrix = Matrix4.Identity;

            Matrix4 translation = Matrix4.CreateTranslation(offsetX - width / 2, offsetY - height / 2, 0);
            translation.Transpose();

            Matrix4 scale = Matrix4.CreateScale(2.0f / width, 2.0f / height, 1);

            viewMatrix *= scale;
            viewMatrix *= translation;

            SetMatrix(VIEW_MATRIX, viewMatrix);
            SetMatrix(PROJECTION_MATRIX, projectionMatrix);
        }


        internal static void SetDrawColor(float r, float g, float b, float a) {
            Color4 col = Color4.RGBA(r, g, b, a);
            SetDrawColor(col);
        }

        internal static void SetDrawColor(Color4 col) {
            if (solidShader.Color.Equals(col))
                return;

            Flush();

            solidShader.Color = col;
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


        #region IDisposable Support

        public static void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                meshOutputStream.Dispose();
                solidShader.Dispose();
                textDrawer.Dispose();
                textureManager.Dispose();

                TextureMap.UnloadTextures();

                framebufferManager.Dispose();

                Texture.Set(null);
                // TODO: set large fields to null.

                Console.WriteLine("Context disposed");
                disposed = true;
            }
        }
        #endregion
    }
}

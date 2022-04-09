using MinimalAF.Rendering.ImmediateMode;
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
    internal static class CTX {
        public const int MODEL_MATRIX = 0;
        public const int VIEW_MATRIX = 1;
        public const int PROJECTION_MATRIX = 2;
        public const int NUM_MATRICES = 3;

        //Here solely for the SwapBuffers function
        private static IGLFWGraphicsContext _glContext;

        private static int _contextWidth, _contextHeight;
        private static Rect _currentScreenRect;

        internal static Rect CurrentScreenRect {
            get => _currentScreenRect;
        }

        internal static int ContextWidth {
            get => _contextWidth;
            set => _contextWidth = value;
        }

        internal static int ContextHeight {
            get => _contextHeight;
            set => _contextHeight = value;
        }

        private static bool _disposed; // To detect redundant calls to Dispose()
        internal static Color4 ClearColor;

        //Composition
        internal static TriangleDrawer Triangle => _triangle;
        internal static QuadDrawer Quad => _quad;
        internal static RectangleDrawer Rect => _rect;
        internal static NGonDrawer NGon => _nGon;
        internal static PolyLineDrawer NLine => _nLine;
        internal static ArcDrawer Arc => _arc;
        internal static CircleDrawer Circle => _circle;
        internal static LineDrawer Line => _line;
        internal static TextDrawer Text => _textDrawer;
        internal static TextureManager Texture => _textureManager;
        internal static FramebufferManager Framebuffer => _framebufferManager;

        internal static TriangleDrawer _triangle;
        internal static QuadDrawer _quad;
        internal static RectangleDrawer _rect;
        internal static NGonDrawer _nGon;
        internal static PolyLineDrawer _nLine;
        internal static ArcDrawer _arc;
        internal static CircleDrawer _circle;
        internal static LineDrawer _line;
        private static TextDrawer _textDrawer;
        private static MeshOutputStream _meshOutputStream;
        private static ImmediateModeShader _solidShader;
        private static TextureManager _textureManager;
        private static FramebufferManager _framebufferManager;

        internal static Matrix4 GetMatrix(int matrix) {
            return _solidShader.GetMatrix(matrix);
        }

        internal static void SetMatrix(int matrix, Matrix4 value) {
            Flush();
            _solidShader.SetMatrix(matrix, value);
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

            _glContext = context;
            _solidShader = new ImmediateModeShader();
            _solidShader.Use();
            _textureManager = new TextureManager();
            _framebufferManager = new FramebufferManager();

            ClearColor = Color4.VA(0, 0);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Equal, 0, 0xFF);

            GL.Enable(EnableCap.DepthTest);
            //TODO: change this to DepthFunction.Less for viewport3D
            GL.DepthFunc(DepthFunction.Lequal);

            _disposed = false; // To detect redundant calls to Dispose()


            Console.WriteLine("Context initialized");
        }

        private static void InitDrawers() {
            _meshOutputStream = new MeshOutputStream(4096, 4 * 4096);

            _triangle = new TriangleDrawer(_meshOutputStream);
            _nGon = new NGonDrawer(_meshOutputStream);
            _quad = new QuadDrawer(_meshOutputStream);
            _nLine = new PolyLineDrawer(_meshOutputStream);

            _line = new LineDrawer();
            _arc = new ArcDrawer(circleEdgeLength: 5, maxCircleEdgeCount: 32);
            _rect = new RectangleDrawer();
            _circle = new CircleDrawer();
            _textDrawer = new TextDrawer();
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
            _meshOutputStream.Flush();
        }

        internal static void SwapBuffers() {
            Flush();
            _framebufferManager.StopUsing();
            SetMatrix(MODEL_MATRIX, Matrix4.Identity);
            _glContext.SwapBuffers();
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
            SetScissor(screenRect);
        }

        internal static void SetScissor(Rect screenRect) {
            GL.Scissor((int)screenRect.X0, (int)screenRect.Y0, (int)screenRect.Width, (int)screenRect.Height);
            GL.Enable(EnableCap.ScissorTest);
        }

        internal static void SetScreenRect(Rect screenRect, bool clipping) {
            if (clipping) {
                Flush();
                SetScissor(screenRect);
            }

            Cartesian2D(ContextWidth, ContextHeight, screenRect.X0, screenRect.Y0);
            _currentScreenRect = screenRect;
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
            if (_solidShader.Color.Equals(col))
                return;

            Flush();

            _solidShader.Color = col;
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
            if (!_disposed) {
                if (disposing) {
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                _meshOutputStream.Dispose();
                _solidShader.Dispose();
                _textDrawer.Dispose();
                _textureManager.Dispose();

                TextureMap.UnloadTextures();

                _framebufferManager.Dispose();

                Texture.Set(null);
                // TODO: set large fields to null.

                Console.WriteLine("Context disposed");
                _disposed = true;
            }
        }
        #endregion
    }
}

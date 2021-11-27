using MinimalAF.Rendering.ImmediateMode;
using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MinimalAF.Rendering
{
	//TODO: Extend this to draw meshes and other retained geometry
	//TODO: Full refactor

	/// <summary>
	/// CTX is short for RenderContext. Because this will be typed a LOT, I have opted for
	/// a shorter name even though it is contrary to principles of 'clean code'
	/// </summary>
	public static class CTX
    {
		//Composition
		public static TriangleDrawer Triangle => _triangle;
		public static QuadDrawer Quad => _quad;
		public static RectangleDrawer Rect => _rect;
		public static NGonDrawer NGon => _nGon;
		public static PolyLineDrawer NLine => _nLine;
		public static ArcDrawer Arc => _arc;
		public static CircleDrawer Circle => _circle;
		public static LineDrawer Line => _line;
        public static TextDrawer Text => _textDrawer;

        public static TriangleDrawer _triangle;
        public static QuadDrawer _quad;
        public static RectangleDrawer _rect;
        public static NGonDrawer _nGon;
        public static PolyLineDrawer _nLine;
        public static ArcDrawer _arc;
        public static CircleDrawer _circle;
        public static LineDrawer _line;
		private static TextDrawer _textDrawer;

        private static MeshOutputStream _meshOutputStream;

        private static ImmediateModeShader _solidShader;
        private static TextureManager _textureManager;

		//Here solely for the SwapBuffers function
		private static IGLContext _glContext;
		public static GL GL => _gl;

		private static GL _gl;

		// Actual state info
		private static int _width, _height;
        private static bool _disposed; // To detect redundant calls to Dispose()
        private static Matrix4x4 _viewMatrix;
        private static Matrix4x4 _projectionMatrix;
        private static List<Matrix4x4> _modelMatrices;
        private static Color4 _clearColor;
        private static Dictionary<int, Framebuffer> _framebufferList;


		static CTX()
		{
			// TODO: Split further into rectangle and quad

		}

        internal static void Init(IGLContextSource context)
		{
			InitDrawers();

			_glContext = context.GLContext;
			_gl = GL.GetApi(_glContext);

			_solidShader = new ImmediateModeShader();
			_solidShader.Use();
			_textureManager = new TextureManager();
			_framebufferList = new Dictionary<int, Framebuffer>();

			_viewMatrix = Matrix4x4.Identity;
			_projectionMatrix = Matrix4x4.Identity;
			_modelMatrices = new List<Matrix4x4>();
			_clearColor = Color4.VA(0, 0);


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

		private static void InitDrawers()
		{
			// Order matters
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

		public static Color4 GetClearColor()
        {
            return _clearColor;
        }

        public static void SetClearColor(float r, float g, float b, float a)
        {
            _clearColor = Color4.RGBA(r, g, b, a);
        }


        public static void Clear()
        {
            GL.StencilMask(1);
            GL.ClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
		}


        internal static void Flush()
        {
            _meshOutputStream.Flush();
        }

        internal static void SwapBuffers()
        {
            Flush();
            StopUsingFramebuffer();
            FlushTransformMatrices();

            _glContext.SwapBuffers();
        }

        private static void FlushTransformMatrices()
        {
            _modelMatrices.Clear();
            _solidShader.ModelMatrix = Matrix4x4.Identity;
            _solidShader.ViewMatrix = _viewMatrix;
            _solidShader.ProjectionMatrix = _projectionMatrix;
            _solidShader.UpdateTransformUniforms();
        }

		/// <summary>
		/// Initializes the viewport to 2D mode, and generates a coordinate system with 
		/// width and height as the width and height, and bottom-left being zero.
		/// 
		/// It also enables transparency when drawing, which may not be desireable in 3D
		/// </summary>
		public static void Viewport2D(Rect2D screenRect)
        {
            _width = (int)screenRect.Width;
            _height = (int)screenRect.Height;


			//GL.Enable(EnableCap.ScissorTest);
			//GL.Scissor((int)screenRect.X0, (int)screenRect.Y0, _width, _height);
			GL.Viewport((int)screenRect.X0, (int)screenRect.Y0, (uint)screenRect.X1, (uint)screenRect.Y1);

			_projectionMatrix = Matrix4x4.Identity;
            _viewMatrix = Matrix4x4.Identity;

            Matrix4x4 translation = Matrix4x4.CreateTranslation(-1, -1, 0);
            translation = Matrix4x4.Transpose(translation);
            _viewMatrix *= translation;

            _viewMatrix *= Matrix4x4.CreateScale(2.0f / _width, 2.0f / _height, 1);
        }

        public static void SetDrawColor(float r, float g, float b, float a)
        {
            Color4 col = Color4.RGBA(r, g, b, a);
            SetDrawColor(col);
        }

        public static void SetDrawColor(Color4 col)
        {
            if (_solidShader.CurrentColor.Equals(col))
                return;

            Flush();

            _solidShader.CurrentColor = col;
        }

		public static Texture GetTexture()
		{
			return _textureManager.CurrentTexture();
		}


		public static void SetTexture(Texture texture)
        {
            if (_textureManager.IsSameTexture(texture))
                return;

            Flush();

            _textureManager.SetTexture(texture);
        }

        public static void PushMatrix(Matrix4x4 mat)
        {
            Flush();

            if (_modelMatrices.Count == 0)
            {
                _modelMatrices.Add(mat);
            }
            else
            {
                _modelMatrices.Add(_modelMatrices[_modelMatrices.Count - 1] * mat);
            }

            _solidShader.ModelMatrix = _modelMatrices[_modelMatrices.Count - 1];
            _solidShader.UpdateModel();
        }

        public static void PopMatrix()
        {
            Flush();

            if (_modelMatrices.Count > 0)
            {
                _modelMatrices.RemoveAt(_modelMatrices.Count - 1);
            }

            if (_modelMatrices.Count == 0)
            {
                _solidShader.ModelMatrix = Matrix4x4.Identity;
            }
            else
            {
                _solidShader.ModelMatrix = _modelMatrices[_modelMatrices.Count - 1];
            }

            _solidShader.UpdateModel();
        }

        /// <summary>
        /// Clears and enables the stencil buffer, doesn't disable colour writes.
        /// </summary>
        /// <param name="inverseStencil">If this parameter is set to true, the buffer will be cleared to 1s and
        /// we will be drawing 0s to the stencil buffer, and vice versa.</param>
        public static void StartStencillingWhileDrawing(bool inverseStencil = false)
        {
            StartStencilling(true, inverseStencil);
        }

        /// <summary>
        /// Clears and enables the stencil buffer, and then disables colour writes
        /// </summary>
        /// <param name="inverseStencil">If this parameter is set to true, the buffer will be cleared to 1s and
        /// we will be drawing 0s to the stencil buffer, and vice versa.</param>
        public static void StartStencillingWithoutDrawing(bool inverseStencil = false)
        {
            StartStencilling(false, inverseStencil);
        }

        private static void StartStencilling(bool canDraw, bool inverseStencil)
        {
            Flush();

            if (!canDraw)
            {
                GL.ColorMask(false, false, false, false);
            }

            if (inverseStencil)
            {
                GL.ClearStencil(1);
            }
            else
            {
                GL.ClearStencil(0);
            }

            GL.StencilMask(1);
            GL.Clear(ClearBufferMask.StencilBufferBit);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

            if (inverseStencil)
            {
                GL.StencilFunc(StencilFunction.Always, 0, 0);
            }
            else
            {
                GL.StencilFunc(StencilFunction.Always, 1, 1);
            }
        }


        /// <summary>
        /// Disables writing to the stencil buffer, enables writing to the color buffer.
        /// 
        /// Pixels in the stencil buffer that were set to 1 will not be drawn to.
        /// </summary>
        public static void StartUsingStencil()
        {
            Flush();

            GL.ColorMask(true, true, true, true);
            GL.StencilFunc(StencilFunction.Notequal, 1, 1);
            GL.StencilMask(0);
        }

        public static void LiftStencil()
        {
            Flush();

            GL.Disable(EnableCap.StencilTest);
        }


        /// <summary>
        /// Creates a new framebuffer with a depth buffer and a stencil buffer for the given index
        /// that is the same size as the window (Specified with Viewport2D (or Viewport3D when it exists) )
        /// if it does not yet exist, and tells the rendering API to start using it.
        /// 
        /// If you are using MinimalAF, the window size thing should be taken care of automatically.
        /// As of now, drawing to a framebuffer of any size other than the window size does not work
        /// </summary>
        public static void UseFramebuffer(int index)
        {
            if (!_framebufferList.ContainsKey(index))
            {
                _framebufferList[index] = new Framebuffer(_width, _height,
                    new TextureImportSettings
                    {
                        Filtering = FilteringType.NearestNeighbour
                    });
            }


            _framebufferList[index].ResizeIfRequired(_width, _height);

            Flush();


            _framebufferList[index].Use();
            Clear();
        }

        public static void UseFramebufferTransparent(int index)
        {
            Color4 prevClearColor = _clearColor;
            _clearColor = Color4.RGBA(0, 0, 0, 0);

            UseFramebuffer(index);

            _clearColor = prevClearColor;
        }

        public static void StopUsingFramebuffer()
        {
            Flush();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public static Texture GetFramebufferTextureHandle(int index)
        {
            if (!_framebufferList.ContainsKey(index))
            {
                return null;
            }

            return _framebufferList[index].Texture;
        }

        /// <summary>
        /// Exact same as
        /// SetTexture(GetFramebufferTextureHandle(index));
        /// but with a null check on result of GetFramebufferTextureHandle
        /// </summary>
        /// <param name="index"></param>
        public static void SetTextureToFramebuffer(int index)
        {
            Texture t = GetFramebufferTextureHandle(index);
            if (t == null)
                return;

            SetTexture(t);
        }

        /// <summary>
        /// This only works in a 2D context
        /// </summary>
        /// <param name="index"></param>
        public static void DrawFramebufferToScreen2D(int index)
        {
            Texture prevTexture = _textureManager.CurrentTexture();
            SetTextureToFramebuffer(index);
            Rect.Draw(0, 0, _width, _height);
            SetTexture(prevTexture);
        }


        #region IDisposable Support

        internal static void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                _meshOutputStream.Dispose();
                _solidShader.Dispose();
                _textDrawer.Dispose();
                _textureManager.Dispose();

                TextureMap.UnloadTextures();

                foreach(var intFramebufferPair in _framebufferList)
                {
                    intFramebufferPair.Value.Dispose();
                }

                _framebufferList.Clear();

                SetTexture(null);
                // TODO: set large fields to null.

                Console.WriteLine("Context disposed");
                _disposed = true;
            }
        }
        #endregion
    }
}

using MinimalAF.Rendering.ImmediateMode;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;

namespace MinimalAF.Rendering
{
	/// <summary>
	/// CTX is short for RenderContext. Because this will be typed a LOT, I have opted for
	/// a shorter name even though a static class like this is contrary to principles of 'clean code'
	/// </summary>
	public static class CTX
	{
		//Here solely for the SwapBuffers function
		private static IGLFWGraphicsContext _glContext;

		private static int _contextWidth, _contextHeight;
		private static Rect2D _currentScreenRect;

		public static Rect2D CurrentScreenRect {
			get => _currentScreenRect;
		}

		public static int ContextWidth {
			get => _contextWidth;
			set => _contextWidth = value;
		}

		public static int ContextHeight {
			get => _contextHeight;
			internal set => _contextHeight = value;
		}

		private static bool _disposed; // To detect redundant calls to Dispose()
		public static Color4 ClearColor;


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
		public static TextureManager Texture => _textureManager;
		public static FramebufferManager Framebuffer => _framebufferManager;

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
		private static FramebufferManager _framebufferManager;

		public static Matrix4 ViewMatrix {
			get => _solidShader.ViewMatrix;
			set {
				Flush();
				_solidShader.ViewMatrix = value;
			}
		}

		public static Matrix4 ModelMatrix {
			get => _solidShader.ModelMatrix;
			set {
				Flush();
				_solidShader.ModelMatrix = value;
			}
		}

		public static Matrix4 ProjectionMatrix {
			get => _solidShader.ProjectionMatrix;
			set {
				Flush();
				_solidShader.ProjectionMatrix = value;
			}
		}

		public struct ViewMatrixPush : IDisposable
		{
			Matrix4 oldMatrix;

			public ViewMatrixPush(Matrix4 matrix)
			{
				this.oldMatrix = ModelMatrix;
				ViewMatrix = matrix;
			}

			public void Dispose()
			{
				ViewMatrix = oldMatrix;
			}
		}

		private struct ModelMatrixPush : IDisposable
		{
			Matrix4 oldMatrix;

			public ModelMatrixPush(Matrix4 matrix)
			{
				this.oldMatrix = ModelMatrix;
				ModelMatrix = matrix;
			}

			public void Dispose()
			{
				ModelMatrix = oldMatrix;
			}
		}

		private struct ProjectionMatrixPush : IDisposable
		{
			Matrix4 oldMatrix;

			public ProjectionMatrixPush(Matrix4 matrix)
			{
				this.oldMatrix = ModelMatrix;
				ProjectionMatrix = matrix;
			}

			public void Dispose()
			{
				ProjectionMatrix = oldMatrix;
			}
		}

		/// <summary>
		/// There is no corresponding PopMatrix method.
		/// Instead, put this inside a using statement.
        ///
        /// <para>
        /// If you want to multiply the previous matrix, you will need to do that yourself.
        /// </para>
		/// </summary>
		public static IDisposable PushMatrix(Matrix4 matrix)
		{
			return new ModelMatrixPush(matrix);
		}


		internal static void Init(IGLFWGraphicsContext context)
		{
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
			return ClearColor;
		}

		public static void SetClearColor(float r, float g, float b, float a)
		{
			ClearColor = Color4.RGBA(r, g, b, a);
		}


		public static void Clear()
		{
			GL.StencilMask(1);
			GL.ClearColor(ClearColor.R, ClearColor.G, ClearColor.B, ClearColor.A);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
		}

		public static void Clear(Color4 col)
		{
			ClearColor = col;
			Clear();
		}


		internal static void Flush()
		{
			_meshOutputStream.Flush();
		}

		internal static void SwapBuffers()
		{
			Flush();
			_framebufferManager.StopUsing();
			ModelMatrix = Matrix4.Identity;
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
		public static void SetViewport(Rect2D screenRect)
		{
			screenRect = screenRect.Rectify();

			GL.Viewport((int)screenRect.X0, (int)screenRect.Y0, (int)screenRect.Width, (int)screenRect.Height);
			SetScissor(screenRect);
		}

		public static void SetScissor(Rect2D screenRect)
		{
			GL.Scissor((int)screenRect.X0, (int)screenRect.Y0, (int)screenRect.Width, (int)screenRect.Height);
			GL.Enable(EnableCap.ScissorTest);
		}

		public static void SetRect(Rect2D screenRect)
		{
			Flush();
			SetScissor(screenRect);
			Cartesian2D(ContextWidth, ContextHeight, screenRect.X0, screenRect.Y0);
			_currentScreenRect = screenRect;
		}

		public static void Cartesian2D(float width, float height, float offsetX = 0, float offsetY = 0)
		{
			Matrix4 projectionMatrix = Matrix4.Identity;

			Matrix4 viewMatrix = Matrix4.Identity;

			Matrix4 translation = Matrix4.CreateTranslation(offsetX -width/2, offsetY -height/2, 0);
			translation.Transpose();

			Matrix4 scale = Matrix4.CreateScale(2.0f / width, 2.0f / height, 1);

			viewMatrix *= scale;
			viewMatrix *= translation;

			ViewMatrix = viewMatrix;
			ProjectionMatrix = projectionMatrix;
		}


		public static void SetDrawColor(float r, float g, float b, float a)
		{
			Color4 col = Color4.RGBA(r, g, b, a);
			SetDrawColor(col);
		}

		public static void SetDrawColor(Color4 col)
		{
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

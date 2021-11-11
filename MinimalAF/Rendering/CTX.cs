using MinimalAF.Datatypes;
using MinimalAF.Rendering.ImmediateMode;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using Color4 = MinimalAF.Datatypes.Color4;

namespace MinimalAF.Rendering
{
    //TODO: Extend this to draw meshes and other retained geometry

    /// <summary>
    /// CTX is short for RenderContext. Because this will be typed a LOT, I have opted for
    /// a shorter name even though it is contrary to principles of 'clean code'
    /// </summary>
    public static class CTX
    {
        //Here solely for the SwapBuffers function
        private static IGLFWGraphicsContext _glContext;
        private static ImmediateModeShader _solidShader;
        private static MeshOutputStream _meshOutputStream;
        private static CombinedDrawer _geometryDrawer;
        private static TextDrawer _textDrawer;
        private static TextureManager _textureManager;

        private static int _width, _height;
        private static bool _drawingText;
        private static bool _disposed; // To detect redundant calls to Dispose()
        private static bool _textDrawingCodeCalledSetTexture;
        private static Texture _previousNonTextTexture;
        private static Matrix4 _viewMatrix;
        private static Matrix4 _projectionMatrix;
        private static List<Matrix4> _modelMatrices;
        private static Color4 _clearColor;
        private static Dictionary<int, Framebuffer> _framebufferList;

        internal static void Init(IGLFWGraphicsContext context)
        {
            //Composition
            _glContext = context;

            int bufferSize = 4096;
            _meshOutputStream = new MeshOutputStream(bufferSize, 4 * bufferSize);

            _geometryDrawer = new CombinedDrawer(_meshOutputStream);
            _textDrawer = new TextDrawer(_geometryDrawer.QuadDrawer);

            _solidShader = new ImmediateModeShader();
            _solidShader.Use();

            _textureManager = new TextureManager();

            //State
            _drawingText = false;
            _disposed = false; // To detect redundant calls to Dispose()
            _textDrawingCodeCalledSetTexture = false;
            _previousNonTextTexture = null;
            _viewMatrix = Matrix4.Identity;
            _projectionMatrix = Matrix4.Identity;
            _modelMatrices = new List<Matrix4>();
            _clearColor = new Color4(0, 0);
            _framebufferList = new Dictionary<int, Framebuffer>();

            Console.WriteLine("Context initialized");
        }


        public static Color4 GetClearColor()
        {
            return _clearColor;
        }

        public static void SetClearColor(float r, float g, float b, float a)
        {
            _clearColor = new Color4(r, g, b, a);
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

            _glContext.SwapBuffers();
            
            FlushTransformMatrices();

            StopUsingFramebuffer();
        }

        private static void FlushTransformMatrices()
        {
            _modelMatrices.Clear();
            _solidShader.ModelMatrix = Matrix4.Identity;
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
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void Viewport2D(int width, int height)
        {
            _width = width;
            _height = height;

            _projectionMatrix = Matrix4.Identity;
            _viewMatrix = Matrix4.Identity;

            Matrix4 translation = Matrix4.CreateTranslation(-1, -1, 0);
            translation.Transpose();
            _viewMatrix *= translation;

            _viewMatrix *= Matrix4.CreateScale(2.0f / width, 2.0f / height, 1);


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Equal, 0, 0xFF);

            GL.Enable(EnableCap.DepthTest);
            //TODO: change this to DepthFunction.Less for viewport3D
            GL.DepthFunc(DepthFunction.Lequal);
        }

        public static void SetDrawColor(float r, float g, float b, float a)
        {
            Color4 col = new Color4(r, g, b, a);
            SetDrawColor(col);
        }

        public static void SetDrawColor(Color4 col)
        {
            if (_solidShader.CurrentColor.Equals(col))
                return;

            Flush();

            _solidShader.CurrentColor = col;
        }

        public static void SetTexture(Texture texture)
        {
            if (_textureManager.CurrentTexture() == texture && (!TextureManager.GlobalTextureHasChanged()))
                return;

            if (!_drawingText)
            {
                _previousNonTextTexture = _textureManager.CurrentTexture();
            }
            else
            {
                if (!_textDrawingCodeCalledSetTexture)
                {
                    _previousNonTextTexture = _textureManager.CurrentTexture();
                    _textDrawingCodeCalledSetTexture = true;
                }
            }

            Flush();

            _textureManager.SetTexture(texture);
        }

        private static void StopUsingTextTexture()
        {
            if (!_drawingText)
                return;

            SetTexture(_previousNonTextTexture);

            //Important that this is set after SetTexture
            _drawingText = false;
            _textDrawingCodeCalledSetTexture = false;
        }

        public static void PushMatrix(Matrix4 mat)
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
                _solidShader.ModelMatrix = Matrix4.Identity;
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
            _clearColor = new Color4(0, 0, 0, 0);

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
            DrawRect(0, 0, _width, _height);
            SetTexture(prevTexture);
        }

        #region _textDrawer Wrappers 

        public static void SetCurrentFont(string name, int size)
        {
            _drawingText = true;
            _textDrawer.SetCurrentFont(name, size);
            
            //SetCurrentFont creates a new OpenGL texture, which also sets it globally.
            //We need to set it back to what it was before. 
            _textureManager.SetTexture(_textureManager.CurrentTexture());
        }

        public static PointF DrawText(string text, float x, float y)
        {
            _drawingText = true;
            return _textDrawer.DrawText(text, x, y);
        }

        public static PointF DrawText(string text, float x, float y, float scale)
        {
            _drawingText = true;
            return _textDrawer.DrawText(text, x, y, scale);
        }

        public static PointF DrawText(string text, int start, int end, float x, float y, float scale)
        {
            _drawingText = true;
            return _textDrawer.DrawText(text, start, end, x, y, scale);
        }

        public static PointF DrawTextAligned(string text, float x, float y, HorizontalAlignment hAlign, VerticalAlignment vAlign, float scale = 1)
        {
            _drawingText = true;
            return _textDrawer.DrawTextAligned(text, x, y, hAlign, vAlign, scale);
        }

        public static float GetCharHeight()
        {
            return _textDrawer.CharHeight;
        }

        public static float GetCharHeight(char c)
        {
            return _textDrawer.GetCharHeight(c);
        }

        public static float GetCharWidth()
        {
            return _textDrawer.CharWidth;
        }

        public static float GetCharWidth(char c)
        {
            return _textDrawer.GetCharWidth(c);
        }

        public static float GetStringWidth(string s)
        {
            return _textDrawer.GetStringWidth(s);
        }

        public static float GetStringWidth(string s, int start, int end)
        {
            return _textDrawer.GetStringWidth(s, start, end);
        }

        public static float GetStringHeight(string text)
        {
            return _textDrawer.GetStringHeight(text);
        }
        public static float GetStringHeight(string text, int start, int end)
        {
            return _textDrawer.GetStringHeight(text, start, end);
        }

        #endregion

        #region _geometryDrawer Wrappers
        public static void DrawTriangle(
                float x0, float y0, float x1, float y1, float x2, float y2,
                float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.5f, float v1 = 1f, float u2 = 1, float v2 = 0)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawTriangle(x0, y0, x1, y1, x2, y2, u0, v0, u1, v1, u2, v2);

        }

        /// <summary>
        /// This overload can be used to draw regular polygons
        /// </summary>
        public static void DrawCircle(float x0, float y0, float r, int edges)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawCircle(x0, y0, r, edges);
        }

        public static void DrawCircle(float x0, float y0, float r)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawCircle(x0, y0, r);
        }

        public static void DrawCircleOutline(float thickness, float x0, float y0, float r, int edges)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawCircleOutline(thickness, x0, y0, r, edges);
        }

        public static void DrawCircleOutline(float thickness, float x0, float y0, float r)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawCircleOutline(thickness, x0, y0, r);
        }

        public static void DrawQuad(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3,
            float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.0f, float v1 = 1f, float u2 = 1, float v2 = 1, float u3 = 1, float v3 = 0)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawQuad(x0, y0, x1, y1, x2, y2, x3, y3, u0, v0, u1, v1, u2, v2, u3, v3);

        }

        public static void DrawRect(Rect2D rect)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRect(rect);
        }


        public static void DrawRect(Rect2D rect, Rect2D uvs)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRect(rect, uvs);
        }

        public static void DrawRect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 0, float u1 = 1, float v1 = 1)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRect(x0, y0, x1, y1, u0, v0, u1, v1);
        }

        public static void DrawRectOutline(float thickness, Rect2D rect)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRectOutline(thickness, rect);
        }

        public static void DrawRectOutline(float thickness, float x0, float y0, float x1, float y1)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRectOutline(thickness, x0, y0, x1, y1);
        }


        public static void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawArc(xCenter, yCenter, radius, startAngle, endAngle);
        }


        public static void DrawArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawArc(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public static void DrawArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawArcOutline(thickness, xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public static void DrawArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawArcOutline(thickness, xCenter, yCenter, radius, startAngle, endAngle);
        }

        public static void DrawLine(float x0, float y0, float x1, float y1, float thickness, CapType cap)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawLine(x0, y0, x1, y1, thickness, cap);
        }

        public static void DrawLineOutline(float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawLineOutline(outlineThickness, x0, y0, x1, y1, thickness, cap);
        }

        public static void BeginPolyLine(float x0, float y0, float thickness, CapType cap)
        {
            StopUsingTextTexture();
            _geometryDrawer.BeginPolyLine(x0, y0, thickness, cap);
        }

        public static void AppendToPolyLine(float x0, float y0)
        {
            StopUsingTextTexture();
            _geometryDrawer.AppendToPolyLine(x0, y0);
        }

        public static void EndPolyLine(float x0, float y0)
        {
            StopUsingTextTexture();
            _geometryDrawer.EndPolyLine(x0, y0);
        }

        #endregion

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

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using RenderingEngine.Logic;
using RenderingEngine.Rendering.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering.ImmediateMode;
using Color4 = RenderingEngine.Datatypes.Color4;

namespace RenderingEngine.Rendering
{
    //TODO: Extend this to draw meshes and other retained geometry
    //Was initially called RenderingContext, and was not a static class.
    //Instead, each class that needed to be drawn held a reference to an instance of this class
    //and each instance was usually named _ctx 
    //but if every single instance needs to have a reference to this in order do draw, then wouldn't it be
    //better off as a static class?
    //so that is what I ended up doing
    public static class CTX
    {
        //Here solely for the SwapBuffers function
        private static IGLFWGraphicsContext _glContext;

        private static ImmediateModeShader _solidShader;
        private static MeshOutputStream _meshOutputStream;
        private static CombinedDrawer _geometryDrawer;
        private static TextDrawer _textDrawer;
        private static TextureManager _textureManager;

        private static bool _drawingText = false;

        private static bool _textDrawingCodeCalledSetTexture = false;
        private static Texture _previousNonTextTexture = null;

        private static Matrix4 _viewMatrix = Matrix4.Identity;
        private static Matrix4 _projectionMatrix = Matrix4.Identity;

        private static List<Matrix4> _modelMatrices = new List<Matrix4>();

        internal static void Init(IGLFWGraphicsContext context){
            int bufferSize = 4096;
            _meshOutputStream = new MeshOutputStream(bufferSize, 4 * bufferSize);

            _geometryDrawer = new CombinedDrawer(_meshOutputStream);
            _textDrawer = new TextDrawer(_geometryDrawer.QuadDrawer);

            _solidShader = new ImmediateModeShader();
            _solidShader.Use();

            _textureManager = new TextureManager();

            _glContext = context;
            _disposed = false;
		}

        public static void SetClearColor(float r, float g, float b, float a)
        {
            GL.ClearColor(r, g, b, a);
        }


        public static void Clear()
        {
            GL.StencilMask(~0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }


        public static void Flush()
        {
            _meshOutputStream.Flush();
        }

        public static void SwapBuffers()
        {
            Flush();
            _glContext.SwapBuffers();

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
        public static void Viewport2D(float width, float height)
        {
            _viewMatrix = Matrix4.Identity;
            _projectionMatrix = Matrix4.Identity;

            Matrix4 translation = Matrix4.CreateTranslation(-1, -1, 0);
            translation.Transpose();
            _viewMatrix *= translation;

            _viewMatrix *= Matrix4.CreateScale(2.0f / width, 2.0f / height, 1);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(StencilFunction.Equal, 0, 0xFF);

            
            GL.Enable(EnableCap.DepthTest);
            //TODO: change this for viewport3D
            GL.DepthFunc(DepthFunction.Always);
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
                _modelMatrices.RemoveAt(_modelMatrices.Count-1);
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

        public static void StartStencillingWhileDrawing()
        {
            StartStencilling(true);
        }

        public static void StartStencillingWithoutDrawing()
        {
            StartStencilling(false);
        }

        private static void StartStencilling(bool canDraw)
        {
            Flush();

            if (!canDraw)
            {
                GL.ColorMask(false, false, false, false);
            }

            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            GL.StencilFunc(StencilFunction.Always, 1, ~0);
            GL.StencilMask(~0);
        }

        public static void StartUsingStencil()
        {
            Flush();

            GL.ColorMask(true,true,true,true);
            GL.StencilFunc(StencilFunction.Notequal, 1, ~0);
            GL.StencilMask(0);
        }

        public static void LiftStencil()
        {
            Flush();

            GL.Disable(EnableCap.StencilTest);
            GL.StencilMask(~0);
            GL.Clear(ClearBufferMask.StencilBufferBit);
        }

        #region _textDrawer Wrappers 

        public static void SetCurrentFont(string name, int size)
        {
            _drawingText = true;
            _textDrawer.SetCurrentFont(name, size);
            _textureManager.SetTexture(_textureManager.CurrentTexture());
        }

        public static void DrawText(string text, float x, float y)
        {
            _drawingText = true;
            _textDrawer.DrawText(text, x, y);
        }

        public static void DrawText(string text, float x, float y, float scale)
        {
            _drawingText = true;
            _textDrawer.DrawText(text, x, y, scale);
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

        public static float GetStringHeight(string text)
        {
            return _textDrawer.GetStringHeight(text);
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

        public static void DrawRect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 1, float u1 = 1, float v1 = 0)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRect(x0, y0, x1, y1, u0, v0, u1, v1);
        }

        public static void DrawRectOutline(float thickness, Rect2D rect)
        {
            _geometryDrawer.DrawRectOutline(thickness, rect);
        }

        public static void DrawRectOutline(float thickness, float x0, float y0, float x1, float y1)
        {
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
        private static bool _disposed = false; // To detect redundant calls

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

                // TODO: set large fields to null.

                _disposed = true;
            }
        }
        #endregion
    }
}

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

namespace RenderingEngine.Rendering.ImmediateMode
{
    public class RenderingContext : IDisposable
    {
        //Here solely for the SwapBuffers function
        private IGLFWGraphicsContext _ctx;

        ImmediateModeShader _solidShader;
        MeshOutputStream _meshOutputStream;
        GeometryDrawer _geometryDrawer;
        TextDrawer _textDrawer;
        TextureManager _textureManager;

        bool _drawingText = false;
        bool _textDrawingCodeCalledSetTexture = false;
        Texture _previousNonTextTexture = null;

        public RenderingContext(IGLFWGraphicsContext context)
        {
            _ctx = context;

            int bufferSize = 4096;
            _meshOutputStream = new MeshOutputStream(bufferSize, 4 * bufferSize);

            _geometryDrawer = new GeometryDrawer(_meshOutputStream);
            _textDrawer = new TextDrawer(_geometryDrawer, this);

            _solidShader = new ImmediateModeShader();
            _solidShader.Use();

            _textureManager = new TextureManager();
        }

        public void SetClearColor(float r, float g, float b, float a)
        {
            GL.ClearColor(r, g, b, a);
        }


        public void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Flush()
        {
            _meshOutputStream.Flush();
            _ctx.SwapBuffers();
        }

        /// <summary>
        /// Initializes the viewport to 2D mode, and generates a coordinate system with 
        /// width and height as the width and height, and bottom-left being zero.
        /// 
        /// It also enables transparency when drawing, which may not be desireable in 3D
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Viewport2D(float width, float height)
        {
            _solidShader.ProjectionMatrix = Matrix4.Identity;

            Matrix4 translation = Matrix4.CreateTranslation(-1, -1, 0);
            translation.Transpose();
            _solidShader.ProjectionMatrix *= translation;

            _solidShader.ProjectionMatrix *= Matrix4.CreateScale(2.0f / width, 2.0f / height, 1);

            _solidShader.UpdateTransformUniforms();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void SetDrawColor(float r, float g, float b, float a)
        {
            Color4 col = new Color4(r, g, b, a);
            SetDrawColor(col);
        }

        public void SetDrawColor(Color4 col)
        {
            if (_solidShader.CurrentColor.Equals(col))
                return;

            _meshOutputStream.Flush();

            _solidShader.CurrentColor = col;
        }

        public void SetTexture(Texture texture)
        {
            if (_textureManager.CurrentTexture() == texture)
                return;

            if(!_drawingText)
            {
                _previousNonTextTexture = _textureManager.CurrentTexture();
            } else
            {
                if (!_textDrawingCodeCalledSetTexture)
                {
                    _previousNonTextTexture = _textureManager.CurrentTexture();
                    _textDrawingCodeCalledSetTexture = true;
                }
            }

            _meshOutputStream.Flush();

            _textureManager.SetTexture(texture);
        }

        private void StopUsingTextTexture()
        {
            if (!_drawingText)
                return;

            SetTexture(_previousNonTextTexture);

            //Important that this is set after SetTexture
            _drawingText = false;
            _textDrawingCodeCalledSetTexture = false;
        }


        // ------------------- _textDrawer Wrappers  -------------------

        public void SetCurrentFont(string name, int size)
        {
            _drawingText = true;
            _textDrawer.SetCurrentFont(name, size);
            _textureManager.SetTexture(_textureManager.CurrentTexture());
        }

        public void DrawText(string text, float x, float y)
        {
            _drawingText = true;
            _textDrawer.DrawText(text, x, y);
        }

        public float GetCharHeight()
        {
            return _textDrawer.CharHeight;
        }

        public float GetCharHeight(char c)
        {
            return _textDrawer.GetCharHeight(c);
        }

        public float GetCharWidth()
        {
            return _textDrawer.CharWidth;
        }

        public float GetCharWidth(char c)
        {
            return _textDrawer.GetCharWidth(c);
        }


        // ------------------- _geometryDrawer Wrappers  -------------------

        public void DrawTriangle(
                float x0, float y0, float x1, float y1, float x2, float y2,
                float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.5f, float v1 = 1f, float u2 = 1, float v2 = 0)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawTriangle(x0, y0, x1, y1, x2, y2, u0, v0, u1, v1, u2, v2);

        }

        public void DrawFilledCircle(float x0, float y0, float r, int edges)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawFilledCircle(x0, y0, r, edges);
        }

        public void DrawQuad(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3,
            float u0 = 0.0f, float v0 = 0.0f, float u1 = 0.0f, float v1 = 1f, float u2 = 1, float v2 = 1, float u3 = 1, float v3 = 0)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawQuad(x0, y0, x1, y1, x2, y2, x3, y3, u0, v0, u1, v1, u2, v2, u3, v3);

        }

        public void DrawRect(Rect2D rect)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRect(rect);
        }

        public void DrawRect(Rect2D rect, Rect2D uvs)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRect(rect, uvs);

        }

        public void DrawRect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 1, float u1 = 1, float v1 = 0)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawRect(x0, y0, x1, y1, u0, v0, u1, v1);
        }

        public void DrawFilledArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawFilledArc(xCenter, yCenter, radius, startAngle, endAngle);
        }


        public void DrawFilledArc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawFilledArc(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
        }

        public void DrawLine(float x0, float y0, float x1, float y1, float thickness, CapType cap)
        {
            StopUsingTextTexture();
            _geometryDrawer.DrawLine(x0, y0, x1, y1, thickness, cap);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
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

                disposedValue = true;
            }
        }

        ~RenderingContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

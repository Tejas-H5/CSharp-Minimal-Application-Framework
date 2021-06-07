using OpenTK.Mathematics;

namespace MinimalAF.Rendering.Uncomplete
{
    /// <summary>
    /// Used for drawing 2D rectangles that have stuff inside them that cant overflow to the outside
    /// </summary>
    public class ResizingFramebufferRect2D
    {
        Framebuffer _frameBuffer;
        Vector2 _upperCorner;

        public ResizingFramebufferRect2D(int width, int height)
        {
            _frameBuffer = new Framebuffer(width, height);
            _upperCorner = new Vector2(1, 1);
        }

        public void Resize(int width, int height)
        {
            if (width < _frameBuffer.Width)
            {
                float nX = width / (float)_frameBuffer.Width;
                float nY = height / (float)_frameBuffer.Height;
                _upperCorner = new Vector2(nX, nY);
            }
        }

        public void Bind()
        {
            _frameBuffer.Bind();
        }

        public void Draw()
        {

        }
    }
}

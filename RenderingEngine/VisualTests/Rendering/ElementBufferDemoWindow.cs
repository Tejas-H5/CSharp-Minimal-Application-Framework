using OpenTK.Graphics.OpenGL;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.Rendering.ImmediateMode;
using System;

namespace RenderingEngine.VisualTests.Rendering
{
    class ElementBufferDemoWindow : EntryPoint
    {
        // We modify the vertex array to include four vertices for our rectangle.
        private readonly Vertex[] _vertices =
        {
            //0.5f,  0.5f, 0.0f,0.0f,0.0f, // top right
            //0.5f, -0.5f, 0.0f,0.0f,0.0f, // bottom right
            //-0.5f, -0.5f, 0.0f,0.0f,0.0f, // bottom left
            //-0.5f,  0.5f, 0.0f,0.0f,0.0f, // top left
            new Vertex(0.5f,  0.5f, 0.0f), // top right
            new Vertex(0.5f, -0.5f, 0.0f), // bottom right
            new Vertex(-0.5f, -0.5f, 0.0f), // bottom left
            new Vertex(-0.5f,  0.5f, 0.0f), // top left
        };

        // Then, we create a new array: indices.
        // This array controls how the EBO will use those vertices to create triangles
        private readonly uint[] _indices =
        {
            // Note that indices start at 0!
            0, 1, 3, // The first triangle will be the bottom-right half of the triangle
            1, 2, 3  // Then the second will be the top-right half of the triangle
        };

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private ImmediateModeShader _shader;

        // Add a handle for the EBO
        private int _elementBufferObject;

        protected void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }

        public override void Start()
        {

            Window.Size = (800, 600);
            Window.Title = "Element buffer demo";

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * 5 * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // We create/bind the Element Buffer Object EBO the same way as the VBO, except there is a major difference here which can be REALLY confusing.
            // The binding spot for ElementArrayBuffer is not actually a global binding spot like ArrayBuffer is. 
            // Instead it's actually a property of the currently bound VertexArrayObject, and binding an EBO with no VAO is undefined behaviour.
            // This also means that if you bind another VAO, the current ElementArrayBuffer is going to change with it.
            // Another sneaky part is that you don't need to unbind the buffer in ElementArrayBuffer as unbinding the VAO is going to do this,
            // and unbinding the EBO will remove it from the VAO instead of unbinding it like you would for VBOs or VAOs.
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            // We also upload data to the EBO the same way as we did with VBOs.
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            // The EBO has now been properly setup. Go to the Render function to see how we draw our rectangle now!

            _shader = new ImmediateModeShader();
            _shader.Use();
        }

        public override void Update(double deltaTime)
        {

        }

        public override void Render(double deltaTime)
        {

            // Because ElementArrayObject is a property of the currently bound VAO
            // the buffer you will find in the ElementArrayBuffer will change with the currently bound VAO.
            GL.BindVertexArray(_vertexArrayObject);

            // Then replace your call to DrawTriangles with one to DrawElements
            // Arguments:
            //   Primitive type to draw. Triangles in this case.
            //   How many indices should be drawn. Six in this case.
            //   Data type of the indices. The indices are an unsigned int, so we want that here too.
            //   Offset in the EBO. Set this to 0 because we want to draw the whole thing.
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, (IntPtr)0);


        }

        public override void Resize()
        {
        }
    }
}

using OpenTK.Mathematics;
using System;

namespace MinimalAF.Rendering.ImmediateMode
{
    public class ImmediateModeShader : IDisposable
    {
        Shader _shader;
        public Shader Shader { get { return _shader; } }

        public Matrix4 ProjectionMatrix { get => _projectionMatrix; set => _projectionMatrix = value; }
        public Matrix4 ViewMatrix { get => _viewMatrix; set => _viewMatrix = value; }
        public Matrix4 ModelMatrix { get => _modelMatrix; set => _modelMatrix = value; }
        public Color4 CurrentColor {
            get {
                return _currentColor;
            }
            set {
                _currentColor = value;
                _shader.SetVector4(_colorUniformLocation, _currentColor);
            }
        }

        int _projectionUniformLocation;
        int _viewUniformLocation;
        int _modelUniformLocation;
        int _colorUniformLocation;

        Matrix4 _projectionMatrix = Matrix4.Identity;
        Matrix4 _viewMatrix = Matrix4.Identity;
        Matrix4 _modelMatrix = Matrix4.Identity;
        Color4 _currentColor;

        public ImmediateModeShader()
        {
            const string vertSource =
                "#version 330\n" +
                "layout(location = 0) in vec3 position;" +
                "layout(location = 1) in vec2 uv;" +
                "out vec2 uv0;" +
                "uniform mat4 model;" +
                "uniform mat4 projection;" +
                "uniform mat4 view;" +
                "" +
                "void main(){" +
                "   gl_Position = projection * view * model * vec4(position, 1);" +
                "   uv0 = uv;" +
                //"   gl_Position =  vec4(position, 1);" +
                "}" +
                "";

            const string fragSource =
                "#version 330\n" +
                "uniform vec4 color;" +
                "uniform sampler2D sampler;" +
                "in vec2 uv0;" +
                "" +
                "void main(){" +
                "   vec4 texColor = texture2D(sampler, uv0.xy);" +
                "   gl_FragColor = color * texColor;" +
                "}" +
                "";

            _shader = new Shader(vertSource, fragSource);

            _colorUniformLocation = _shader.Loc("color");

            _modelUniformLocation = _shader.Loc("model");
            _projectionUniformLocation = _shader.Loc("projection");
            _viewUniformLocation = _shader.Loc("view");

            _projectionMatrix = Matrix4.Identity;
            _viewMatrix = Matrix4.Identity;
            _modelMatrix = Matrix4.Identity;

            UpdateTransformUniforms();
        }

        public void UpdateTransformUniforms()
        {
            _shader.SetMatrix4(_projectionUniformLocation, _projectionMatrix);
            _shader.SetMatrix4(_viewUniformLocation, _viewMatrix);
            _shader.SetMatrix4(_modelUniformLocation, _modelMatrix);
        }

        public void UpdateModel()
        {
            _shader.SetMatrix4(_modelUniformLocation, _modelMatrix);
        }

        public void Use()
        {
            _shader.Use();
        }

        #region IDisposable Support
        ~ImmediateModeShader()
        {
            _shader.Dispose(false);
        }

        public void Dispose()
        {
            _shader.Dispose(true);
        }
        #endregion
    }
}

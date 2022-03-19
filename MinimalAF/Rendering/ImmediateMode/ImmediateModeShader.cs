using OpenTK.Mathematics;
using System;
using MinimalAF.Rendering;

namespace MinimalAF.Rendering.ImmediateMode
{
	public class ImmediateModeShader : Shader
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

		Color4 _color;
		int _colorLoc;

		public ImmediateModeShader()
			: base(vertSource, fragSource)
        {
            UpdateTransformUniforms();
        }

		public Color4 Color { get => _color; set {
				_color = value;
				SetVector4(_colorLoc, _color);
			} 
		}

		protected override void InitShader()
		{
			_colorLoc = Loc("color");
		}
	}
}

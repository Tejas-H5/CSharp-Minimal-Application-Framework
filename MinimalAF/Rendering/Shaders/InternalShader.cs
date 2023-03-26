﻿namespace MinimalAF.Rendering {
    public class InternalShader : Shader {
        const string vertSource =
@"#version 330
{{globals}}
out vec2 uv0;
void main(){
    gl_Position =  vec4(position, 1) * model * view * projection;
    uv0 = uv;
}";
        const string fragSource =
@"#version 330
uniform vec4 color;
uniform sampler2D sampler;
in vec2 uv0;
void main(){
    vec4 texColor = texture2D(sampler, uv0.xy);
    gl_FragColor = color * texColor;
}";

        Color color;
        int colorLoc;
        public InternalShader()
            : base(vertSource, fragSource) {
            colorLoc = UniformLocation("color");
        }

        public Color Color {
            get => color; set {
                color = value;
                SetVector4(colorLoc, color);
            }
        }
    }
}

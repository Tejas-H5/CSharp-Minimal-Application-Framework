using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace MinimalAF.Rendering {
    public readonly struct ShaderSources {
        public readonly string VertexSource;
        public readonly string FragmentSource;

        public ShaderSources(string vertexSource, string fragmentSource) {
            VertexSource = vertexSource;
            FragmentSource = fragmentSource;
        }
    }

    /// <summary>
    /// TODO: learn abt compute shaders
    /// </summary>
    public abstract class Shader {
        public readonly int Handle;
        internal int ModelLoc, ViewLoc, ProjectionLoc;
        readonly Dictionary<string, int> uniformLocations;
        public readonly string VertexSource, FragSource;

        protected Shader(string vertexSource, string fragSource) {
            int vertexShader = CompileShader(vertexSource, ShaderType.VertexShader);
            int fragmentShader = CompileShader(fragSource, ShaderType.FragmentShader);
            int programHandle = LinkProgram(vertexShader, fragmentShader);

            this.VertexSource = vertexSource;
            this.FragSource = fragSource;
            Handle = programHandle;

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int numberOfUniforms);
            uniformLocations = new Dictionary<string, int>();

            for (int i = 0; i < numberOfUniforms; i++) {
                string key = GL.GetActiveUniform(Handle, i, out _, out _);
                int location = GL.GetUniformLocation(Handle, key);

                uniformLocations.Add(key, location);
            }

            ModelLoc = UniformLocation("model");
            ViewLoc = UniformLocation("view");
            ProjectionLoc = UniformLocation("projection");
        }


        private static int CompileShader(string code, ShaderType type) {
            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, code);

            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var errorCode);
            if (errorCode != (int)All.True) {
                var infoLog = GL.GetShaderInfoLog(shader);
                // hahaha 'whilst'
                throw new Exception("Error occurred whilst compiling Shader(" + shader + ").\n\n" + infoLog);
            }

            return shader;
        }


        private static int LinkProgram(int vertexShader, int fragmentShader) {
            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);

            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);

            if (code != (int)All.True) {
                throw new Exception("Error occurred whilst linking Program(" + program + ")");
            }

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            return program;
        }

        protected int UniformLocation(string name) {
            if (!uniformLocations.ContainsKey(name)) {
                throw new Exception("Could not find the " + name + " uniform in the shader. If your program has it"
                    + "but is not using it, it will get optimized out.");
            }

            return uniformLocations[name];
        }

        public void SetInt(int uniformLocation, int data) {
            CTX.Shader.UseShader(this);
            GL.Uniform1(uniformLocation, data);
        }

        public void SetFloat(int uniformLocation, float data) {
            CTX.Shader.UseShader(this);
            GL.Uniform1(uniformLocation, data);
        }

        public void SetMatrix4(int uniformLocation, Matrix4 data) {
            CTX.Shader.UseShader(this);
            GL.UniformMatrix4(uniformLocation, true, ref data);
        }

        public void SetVector2(int uniformLocation, Vector2 data) {
            CTX.Shader.UseShader(this);
            GL.Uniform2(uniformLocation, data);
        }

        public void SetVector3(int uniformLocation, Vector3 data) {
            CTX.Shader.UseShader(this);
            GL.Uniform3(uniformLocation, data);
        }

        public  void SetVector4(int uniformLocation, Vector4 data) {
            CTX.Shader.UseShader(this);
            GL.Uniform4(uniformLocation, data);
        }

        public void SetVector4(int uniformLocation, Color4 data) {
            CTX.Shader.UseShader(this);
            GL.Uniform4(uniformLocation, new Vector4(data.R, data.G, data.B, data.A));
        }


        private bool disposed = false;
        public void Dispose(bool disposing) {
            if (disposed)
                return;

            GL.DeleteProgram(Handle);
            Console.WriteLine("Shader destructed");

            disposed = true;
        }

        ~Shader() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

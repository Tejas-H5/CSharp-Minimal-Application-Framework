using System;
using System.Collections.Generic;
using System.IO;
using Silk.NET.OpenGL;
using System.Numerics;

namespace MinimalAF.Rendering
{
	//Taken from https://github.com/opentk/LearnOpenTK/blob/master/Common/Shader.cs
	//The destructor code was modified to be more like https://docs.microsoft.com/en-us/dotnet/api/system.idisposable.dispose?view=net-5.0
	public class Shader
    {
        public readonly uint Handle;

        private readonly Dictionary<string, int> _uniformLocations;

        public static Shader FromFile(string vertPath, string fragPath)
        {
            // Load vertex shader and compile
            var vertSource = File.ReadAllText(vertPath);
            // We do the same for the fragment shader
            var fragSource = File.ReadAllText(fragPath);

            return new Shader(vertSource, fragSource);
        }

        // This is how you create a simple shader.
        // Shaders are written in GLSL, which is a language very similar to C in its semantics.
        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        // A commented example of GLSL can be found in shader.vert
        public Shader(string vertexSource, string fragSource)
        {
            // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
            // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
            //   The vertex shader won't be too important here, but they'll be more important later.
            // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
            //   The fragment shader is what we'll be using the most here.

            var shaderSource = vertexSource;

            // CTX.GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
            var vertexShader = CTX.GL.CreateShader(ShaderType.VertexShader);

			// Now, bind the GLSL source code
			CTX.GL.ShaderSource(vertexShader, shaderSource);

            // And then compile
            CompileShader(vertexShader);


            shaderSource = fragSource;

            var fragmentShader = CTX.GL.CreateShader(ShaderType.FragmentShader);
            CTX.GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);

            // These two shaders must then be merged into a shader program, which can then be used by OpenCTX.GL.
            // To do this, create a program...
            Handle = CTX.GL.CreateProgram();

            // Attach both shaders...
            CTX.GL.AttachShader(Handle, vertexShader);
            CTX.GL.AttachShader(Handle, fragmentShader);

            // And then link them together.
            LinkProgram(Handle);

            // When the shader program is linked, it no longer needs the individual shaders attacked to it; the compiled code is copied into the shader program.
            // Detach them, and then delete them.
            CTX.GL.DetachShader(Handle, vertexShader);
            CTX.GL.DetachShader(Handle, fragmentShader);
            CTX.GL.DeleteShader(fragmentShader);
            CTX.GL.DeleteShader(vertexShader);

            // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

            // First, we have to get the number of active uniforms in the shader.
            CTX.GL.GetProgram(Handle, ProgramPropertyARB.ActiveUniforms, out var numberOfUniforms);

            // Next, allocate the dictionary to hold the locations.
            _uniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (uint i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = CTX.GL.GetActiveUniform(Handle, i, out _, out _);

                // get the location,
                var location = CTX.GL.GetUniformLocation(Handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }
        }

        private static void CompileShader(uint shader)
        {
            // Try to compile the shader
            CTX.GL.CompileShader(shader);

            // Check for compilation errors
            CTX.GL.GetShader(shader, ShaderParameterName.CompileStatus, out var code);
            if (code != (int)GLEnum.True)
            {
                // We can use `CTX.GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = CTX.GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(uint program)
        {
            // We link the program
            CTX.GL.LinkProgram(program);

            // Check for linking errors
            CTX.GL.GetProgram(program, ProgramPropertyARB.LinkStatus, out var code);
            if (code != (int)GLEnum.True)
            {
                // We can use `CTX.GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        // A wrapper function that enables the shader program.
        public void Use()
        {
            CTX.GL.UseProgram(Handle);
        }

        // The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
        // you can omit the layout(location=X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        public int GetAttribLocation(string attribName)
        {
            return CTX.GL.GetAttribLocation(Handle, attribName);
        }


        /// <summary>
        /// A micro-optimization that can be used to reduce hashmap lookups by getting the location just once
        /// </summary>
        public int Loc(string name)
        {
            return _uniformLocations[name];
        }

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(int location, int data)
        {
            CTX.GL.UseProgram(Handle);
            CTX.GL.Uniform1(location, data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(int location, float data)
        {
            CTX.GL.UseProgram(Handle);
            CTX.GL.Uniform1(location, data);
        }

        /// <summary>
        /// Set a uniform Matrix4x4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public unsafe void SetMatrix4x4(int location, Matrix4x4 data)
        {
            CTX.GL.UseProgram(Handle);
            CTX.GL.UniformMatrix4(location, 1, false, (float*) &data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(int location, Vector3 data)
        {
            CTX.GL.UseProgram(Handle);
            CTX.GL.Uniform3(location, data);
        }

        /// <summary>
        /// Set a uniform Vector4 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector4(int location, Vector4 data)
        {
            CTX.GL.UseProgram(Handle);
            CTX.GL.Uniform4(location, data);
        }


        /// <summary>
        /// Set a uniform Color4 on this shader.
        /// Same as SetVector4 under the hood
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector4(int location, Color4 data)
        {
            CTX.GL.UseProgram(Handle);
            CTX.GL.Uniform4(location, new Vector4(data.R, data.G, data.B, data.A));
        }


        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            CTX.GL.DeleteProgram(Handle);
            Console.WriteLine("Shader destructed");

            disposed = true;
        }

        ~Shader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

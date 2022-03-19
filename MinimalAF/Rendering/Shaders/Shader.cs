using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace MinimalAF.Rendering
{
	public struct ShaderFiles
	{
		public string VertPath, FragPath;

		public ShaderFiles(string vertPath, string fragPath)
		{
			VertPath = vertPath;
			FragPath = fragPath;
		}
	}


	//Taken from https://github.com/opentk/LearnOpenTK/blob/master/Common/Shader.cs
	//The destructor code was modified to be more like https://docs.microsoft.com/en-us/dotnet/api/system.idisposable.dispose?view=net-5.0
	public abstract class Shader
	{
		public readonly int Handle;

		private readonly Dictionary<string, int> _uniformLocations;
		private static Matrix4 _viewMatrix = Matrix4.Identity;
		private static Matrix4 _projectionMatrix = Matrix4.Identity;
		private static Matrix4 _modelMatrix = Matrix4.Identity;

		int _projectionUniformLocation;
		int _viewUniformLocation;
		int _modelUniformLocation;


		public Matrix4 ProjectionMatrix {
			get => _projectionMatrix;
			set {
				_projectionMatrix = value;
				SetMatrix4(_projectionUniformLocation, _projectionMatrix);
			}
		}
		public Matrix4 ViewMatrix {
			get => _viewMatrix;
			set {
				_viewMatrix = value;
				SetMatrix4(_viewUniformLocation, _viewMatrix);
			}
		}

		public Matrix4 ModelMatrix {
			get => _modelMatrix; 
			set {
				_modelMatrix = value;
				SetMatrix4(_modelUniformLocation, _modelMatrix);
			}
		}

		public Shader(ShaderFiles files)
			: this(File.ReadAllText(files.VertPath), File.ReadAllText(files.FragPath))
		{
		}

		public Shader(string vertexSource, string fragSource)
		{
			var shaderSource = vertexSource;

			var vertexShader = GL.CreateShader(ShaderType.VertexShader);

			GL.ShaderSource(vertexShader, shaderSource);

			CompileShader(vertexShader);

			shaderSource = fragSource;

			var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, shaderSource);
			CompileShader(fragmentShader);

			Handle = GL.CreateProgram();

			GL.AttachShader(Handle, vertexShader);
			GL.AttachShader(Handle, fragmentShader);

			LinkProgram(Handle);

			GL.DetachShader(Handle, vertexShader);
			GL.DetachShader(Handle, fragmentShader);
			GL.DeleteShader(fragmentShader);
			GL.DeleteShader(vertexShader);

			GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

			_uniformLocations = new Dictionary<string, int>();

			for (var i = 0; i < numberOfUniforms; i++)
			{
				var key = GL.GetActiveUniform(Handle, i, out _, out _);

				var location = GL.GetUniformLocation(Handle, key);

				_uniformLocations.Add(key, location);
			}

			_modelUniformLocation = Loc("model");
			_projectionUniformLocation = Loc("projection");
			_viewUniformLocation = Loc("view");

			InitShader();
		}

		protected abstract void InitShader();

		public void UpdateTransformUniforms()
		{
			SetMatrix4(_projectionUniformLocation, _projectionMatrix);
			SetMatrix4(_viewUniformLocation, _viewMatrix);
			SetMatrix4(_modelUniformLocation, _modelMatrix);
		}

		public void UpdateModel()
		{
			SetMatrix4(_modelUniformLocation, _modelMatrix);
		}

		private static void CompileShader(int shader)
		{
			GL.CompileShader(shader);

			GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
			if (code != (int)All.True)
			{
				var infoLog = GL.GetShaderInfoLog(shader);
				throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
			}
		}

		private static void LinkProgram(int program)
		{
			GL.LinkProgram(program);

			GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
			if (code != (int)All.True)
			{
				throw new Exception($"Error occurred whilst linking Program({program})");
			}
		}

		public void Use()
		{
			GL.UseProgram(Handle);
		}

		public int GetAttribLocation(string attribName)
		{
			return GL.GetAttribLocation(Handle, attribName);
		}


		/// <summary>
		/// A micro-optimization that can be used to reduce hashmap lookups by getting the location just once
		/// </summary>
		protected int Loc(string name)
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
			GL.UseProgram(Handle);
			GL.Uniform1(location, data);
		}

		/// <summary>
		/// Set a uniform float on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetFloat(int location, float data)
		{
			GL.UseProgram(Handle);
			GL.Uniform1(location, data);
		}

		/// <summary>
		/// Set a uniform Matrix4 on this shader
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		/// <remarks>
		///   <para>
		///   The matrix is transposed before being sent to the shader.
		///   </para>
		/// </remarks>
		public void SetMatrix4(int location, Matrix4 data)
		{
			GL.UseProgram(Handle);
			GL.UniformMatrix4(location, true, ref data);
		}

		/// <summary>
		/// Set a uniform Vector3 on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetVector3(int location, Vector3 data)
		{
			GL.UseProgram(Handle);
			GL.Uniform3(location, data);
		}

		/// <summary>
		/// Set a uniform Vector4 on this shader.
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetVector4(int location, Vector4 data)
		{
			GL.UseProgram(Handle);
			GL.Uniform4(location, data);
		}


		/// <summary>
		/// Set a uniform Color4 on this shader.
		/// Same as SetVector4 under the hood
		/// </summary>
		/// <param name="name">The name of the uniform</param>
		/// <param name="data">The data to set</param>
		public void SetVector4(int location, Color4 data)
		{
			GL.UseProgram(Handle);
			GL.Uniform4(location, new Vector4(data.R, data.G, data.B, data.A));
		}


		private bool disposed = false;
		public virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			GL.DeleteProgram(Handle);
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

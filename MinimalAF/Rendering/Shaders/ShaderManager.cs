using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace MinimalAF.Rendering {
    public class ShaderManager {
        Shader currentProgram;

        public Shader CurrentProgram => currentProgram;

        Matrix4 model, view, projection;

        public Matrix4 Projection => projection;
        public Matrix4 View => view;
        public Matrix4 Model => model;


        internal void SetModelMatrix(Matrix4 matrix) {
            CTX.Flush();

            model = matrix;
            currentProgram.SetMatrix4(currentProgram.ModelLoc, matrix);
        }

        internal void SetProjectionMatrix(Matrix4 matrix) {
            CTX.Flush();

            projection = matrix;
            currentProgram.SetMatrix4(currentProgram.ProjectionLoc, matrix);
        }

        internal void SetViewMatrix(Matrix4 matrix) {
            CTX.Flush();

            view = matrix;
            currentProgram.SetMatrix4(currentProgram.ViewLoc, matrix);
        }

        public void UseShader(Shader s, bool updateUniforms = true) {
            if (s == currentProgram)
                return;

            currentProgram = s;
            GL.UseProgram(currentProgram.Handle);

            if(updateUniforms) {
                SetModelMatrix(model);
                SetViewMatrix(view);
                SetModelMatrix(projection);
            }
        }
    }
}

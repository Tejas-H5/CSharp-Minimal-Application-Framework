using System;
using System.Collections.Concurrent;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.ResourceManagement {
    internal class GLDeletionQueue {
        enum DeleteThing {
            Buffer,
            VertexArray,
            ShaderProgram,
            Texture
        }

        static ConcurrentQueue<(int, DeleteThing)> glBuffersToDelete = new ConcurrentQueue<(int, DeleteThing)>();


        public static void QueueBufferForDeletion(int handle) {
            glBuffersToDelete.Enqueue((handle, DeleteThing.Buffer));
        }
        public static void QueueVertexArrayForDeletion(int handle) {
            glBuffersToDelete.Enqueue((handle, DeleteThing.VertexArray));
        }
        public static void QueueShaderProgramForDeletion(int handle) {
            glBuffersToDelete.Enqueue((handle, DeleteThing.ShaderProgram));
        }
        public static void QueueTextureForDeletion(int handle) {
            glBuffersToDelete.Enqueue((handle, DeleteThing.Texture));
        }


        public static void DeleteResources() {
            if (glBuffersToDelete.Count == 0)
                return;

            while(glBuffersToDelete.TryDequeue(out (int, DeleteThing) data)) {
                (int resource, DeleteThing resType) = data;

                switch (resType) {
                    case DeleteThing.Buffer:  break;
                    case DeleteThing.VertexArray: GL.DeleteVertexArray(resource); break;
                    case DeleteThing.ShaderProgram: GL.DeleteProgram(resource); break;
                    case DeleteThing.Texture: GL.DeleteTexture(resource); break;
                    default: throw new Exception("What the heck kinda resource is this??!?!?");
                }

                Console.WriteLine("GL " + resType.ToString() + " handle " + resource + " deleted");
            }
        }
    }
}

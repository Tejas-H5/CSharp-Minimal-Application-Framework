using MinimalAF.ResourceManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MinimalAF.Rendering {
    public static class MeshMap {
        public static Mesh LoadMesh(string name, string path) {
            var mesh = ResourceMap<Mesh>.Get(name);

            if(mesh == null) {
                string text = File.ReadAllText(path);
                var meshData = MeshData.FromOBJ(text);
                mesh = meshData.ToDrawableMesh();

                Put(name, mesh);
            }

            return mesh;
        }

        public static void Put(string name, Mesh mesh) {
            ResourceMap<Mesh>.Put(name, mesh);
        }

        public static Mesh Get(string name) {
            return ResourceMap<Mesh>.Get(name);
        }

        public static void UnloadAll() {
            ResourceMap<Mesh>.UnloadAll();
        }

        public static void Unload(string name) {
            ResourceMap<Mesh>.Delete(name);
        }
    }
}

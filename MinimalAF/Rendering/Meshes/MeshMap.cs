using MinimalAF.ResourceManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MinimalAF.Rendering {
    public static class MeshMap {
        public static void Put(string name, IMesh mesh) {
            ResourceMap<IMesh>.Put(name, mesh);
        }

        public static IMesh Get(string name) {
            return ResourceMap<IMesh>.Get(name);
        }

        public static void UnloadAll() {
            ResourceMap<IMesh>.UnloadAll();
        }

        public static void Unload(string name) {
            ResourceMap<IMesh>.Delete(name);
        }
    }
}

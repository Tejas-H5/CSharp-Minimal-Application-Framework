using System;
using System.Collections.Generic;

namespace MinimalAF.ResourceManagement {
    /// <summary>
    /// Used to manage the lifetimes of dynamically allocated OpenGL resources that need to be freed.
    /// 
    /// This should be used instead of relying on the garbage collector to trigger deletion of OpenGL resources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class ResourceMap<T> where T : class, IDisposable {
        private static Dictionary<string, T> resourceCache = new Dictionary<string, T>();

        internal static T Get(string name) {
            if (resourceCache.ContainsKey(name))
                return resourceCache[name];

            return null;
        }

        internal static void Put(string name, T resource) {
#if DEBUG
            if(resourceCache.ContainsKey(name)) {
                throw new Exception("Cant overwrite existing resources");
            }
#endif

            resourceCache[name] = resource;
        }

        internal static bool Has(string name) {
            return resourceCache.ContainsKey(name);
        }

        internal static void Delete(string name) {
#if DEBUG
            if(!resourceCache.ContainsKey(name)) {
                throw new Exception("Resource " + name + " doesn't exist");
            }
#endif

            resourceCache[name].Dispose();
            resourceCache.Remove(name);
        }

        internal static void UnloadAll() {
            foreach (T item in resourceCache.Values) {
                item.Dispose();
            }

            resourceCache.Clear();
        }
    }
}

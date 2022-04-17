using System;
using System.Collections.Generic;

namespace MinimalAF.ResourceManagement {
    public static class ResourceMap<T> {
        private static Dictionary<string, T> resourceCache = new Dictionary<string, T>();

        public static void LoadResource<TLoadSettings>(string name, string path, TLoadSettings loadSettings, Func<string, TLoadSettings, T> loadingFunction) {
            if (resourceCache.ContainsKey(name))
                return;

            var resource = loadingFunction(path, loadSettings);
            if (resource != null)
                resourceCache[name] = resource;
        }

        //TODO: return a pink texture or similar
        public static T GetResource(string name) {
            if (!resourceCache.ContainsKey(name))
                return default;

            return resourceCache[name];
        }

        public static void UnloadResource(string name) {
            if (!resourceCache.ContainsKey(name))
                return;

            UnloadResource(resourceCache[name]);
            resourceCache.Remove(name);
        }

        public static void UnloadResources() {
            foreach (var item in resourceCache) {
                UnloadResource(item.Value);
            }

            resourceCache.Clear();
        }

        private static void UnloadResource(T resource) {
            IDisposable unmanagedResource = resource as IDisposable;

            if (unmanagedResource != null) {
                unmanagedResource.Dispose();
            }
        }
    }
}

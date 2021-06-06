using System;
using System.Collections.Generic;

namespace MinimalAF.ResourceManagement
{
    public static class ResourceMap<T>
    {
        private static Dictionary<string, T> _resourceCache = new Dictionary<string, T>();

        public static void RegisterResource<TLoadSettings>(string name, string path, TLoadSettings loadSettings, Func<string, TLoadSettings, T> loadingFunction)
        {
            if (_resourceCache.ContainsKey(name))
                return;

            var resource = loadingFunction(path, loadSettings);
            if (resource != null)
                _resourceCache[name] = resource;
        }

        //TODO: return a pink texture or similar
        public static T GetCached(string name)
        {
            if (!_resourceCache.ContainsKey(name))
                return default;

            return _resourceCache[name];
        }

        public static void UnloadResource(string name)
        {
            if (!_resourceCache.ContainsKey(name))
                return;

            UnloadResource(_resourceCache[name]);
            _resourceCache.Remove(name);
        }

        public static void UnloadResources()
        {
            foreach (var item in _resourceCache)
            {
                UnloadResource(item.Value);
            }

            _resourceCache.Clear();
        }

        private static void UnloadResource(T resource)
        {
            IDisposable unmanagedResource = resource as IDisposable;

            if (unmanagedResource != null)
            {
                unmanagedResource.Dispose();
            }
        }
    }
}

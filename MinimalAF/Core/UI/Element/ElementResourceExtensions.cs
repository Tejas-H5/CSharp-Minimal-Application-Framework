using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF {
    public partial class Element {
		private List<object> _resources = new List<object>();

		public void AddResource(object res) {
			if (GetResourceAtElement(res.GetType()) != null) {
				throw new Exception("This resource is already present on this object");
			}

			_resources.Add(res);
		}

		public bool AddResourceIfNotPresent(object res) {
			if (GetResourceAtElement(res.GetType()) != null) {
				return false;
			}

			_resources.Add(res);
			return true;
		}

		private object GetResourceAtElement(Type tType) {

			foreach (var r in _resources) {
				if (tType.IsAssignableFrom(r.GetType()))
					return r;
			}

			return null;
		}

		private T GetResourceAtElement<T>() where T : class {
			object res = GetResourceAtElement(typeof(T));

			if (res != null) {
				return (T)res;
			}

			return null;
		}

		/// <summary>
        /// Similar to GetAncestor but will search each parent node incuding this one for a particular resource, and
        /// will return it if it is found.
        /// </summary>
		public T GetResource<T>() where T : class {
			Element next = this;
			while (next != null) {
				var res = next.GetResourceAtElement<T>();
				if (res != null)
					return res;
			}

			return null;
		}
	}
}

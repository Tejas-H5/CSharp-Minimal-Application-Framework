using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.Rendering {
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class VertexComponentAttribute : Attribute {
        public readonly string ComponentName;
        internal int FieldSize;
        internal int FieldCount;

        public VertexComponentAttribute(string componentName) {
            ComponentName = componentName;
        }
    }
}

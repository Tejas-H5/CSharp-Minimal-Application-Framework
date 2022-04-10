using System;

namespace MinimalAF.Testing {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class VisualTestAttribute : Attribute {
        public VisualTestAttribute() {
            
        }
    }
}

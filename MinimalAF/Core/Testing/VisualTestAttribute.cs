using System;

namespace MinimalAF {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class VisualTestAttribute : Attribute {
        public VisualTestAttribute() {
            
        }
    }
}

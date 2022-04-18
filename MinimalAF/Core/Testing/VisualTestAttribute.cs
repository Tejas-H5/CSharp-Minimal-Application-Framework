using System;

namespace MinimalAF {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class VisualTestAttribute : Attribute {
        public string Description;
        public string Tags;

        public VisualTestAttribute(string description, string tags) {
            Description = description;
            Tags = tags;
        }
    }
}

using System;

namespace MinimalAF {

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class IgnorePropertyTestingAttribute : Attribute {
        public IgnorePropertyTestingAttribute() {
        }
    }


    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class VisualTestAttribute : Attribute {
        public readonly string Description;
        public readonly string Tags;

        public VisualTestAttribute(string description, string tags) {
            Description = description;
            Tags = tags;
        }
    }
}

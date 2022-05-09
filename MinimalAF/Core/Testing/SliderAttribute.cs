using System;

namespace MinimalAF {
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class SliderAttribute : Attribute {
        public readonly double Value, Min, Max;

        public SliderAttribute(double min, double max, double value) {
            Value = value;
            Min = min;
            Max = max;
        }
    }
}

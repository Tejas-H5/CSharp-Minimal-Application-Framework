using RenderingEngine.Datatypes;
using RenderingEngine.Util;
using System;

namespace RenderingEngine.UI.Property
{
    public class StringProperty : Property<string>
    {
        public StringProperty(string value)
            : base(value)
        {
        }
    }
}

using RenderingEngine.Datatypes;
using RenderingEngine.Util;
using System;

namespace RenderingEngine.UI.Properties
{
    public class StringProperty : Property<string>
    {
        public StringProperty(string value) 
            : base(value)
        {
        }
    }
}

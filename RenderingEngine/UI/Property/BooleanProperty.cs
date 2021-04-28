using RenderingEngine.Datatypes;
using RenderingEngine.Util;
using System;

namespace RenderingEngine.UI.Property
{
    public class BooleanProperty : Property<bool>
    {
        public BooleanProperty(bool value)
            : base(value)
        {
        }

        public override Property<bool> Copy()
        {
            return new BooleanProperty(_value);
        }
    }
}

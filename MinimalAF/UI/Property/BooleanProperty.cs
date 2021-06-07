using MinimalAF.Datatypes;
using MinimalAF.Util;
using System;

namespace MinimalAF.UI
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

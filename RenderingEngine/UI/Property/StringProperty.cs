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

        protected override void SetValueInternal(string val)
        {
            if (val == null)
                val = "";

            base.SetValueInternal(val);
        }

        public override Property<string> Copy()
        {
            return new StringProperty(_value);
        }
    }
}

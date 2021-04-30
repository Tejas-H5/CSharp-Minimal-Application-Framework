using RenderingEngine.Datatypes;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace UICodeGenerator.CustomEditors
{
    public class ColorProperty : Property<Color4>
    {
        public ColorProperty(Color4 value) 
            : base(value)
        {

        }

        protected override void SetValueInternal(Color4 val)
        {
            val.R = MathF.Min(MathF.Max(0, val.R), 1f);
            val.G = MathF.Min(MathF.Max(0, val.G), 1f);
            val.B = MathF.Min(MathF.Max(0, val.B), 1f);
            val.A = MathF.Min(MathF.Max(0, val.A), 1f);

            base.SetValueInternal(val);
        }

        public override Property<Color4> Copy()
        {
            return new ColorProperty(_value);
        }
    }
}

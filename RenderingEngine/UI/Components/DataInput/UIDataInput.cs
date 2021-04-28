using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.DataInput
{
    public abstract class UIDataInput<T> : UIComponent
    {
        protected Property<T> _property;
        public Property<T> Property { get { return _property; } }
    }
}

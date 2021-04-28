using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Property
{
    public interface IProperty
    {
        Type InnerType { get; }
        void SetValue(object obj);

        void AddCallback(object callback);
        void RemoveCallback(object callback);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.UI
{
    public interface IProperty
    {
        Type InnerType { get; }

        void SetValue(object obj);

        /// <summary>
        /// Opt for adding a Typesafe callback directly to the OnDataChanged event whenever possible.
        /// 
        /// This may not be possible if you only have access to this object via the interface, for example
        /// </summary>
        /// <param name="a"></param>
        void AddCallback(Action<object> a);

        /// <summary>
        /// Opt for adding a Typesafe callback directly to the OnDataChanged event whenever possible.
        /// 
        /// This may not be possible if you only have access to this object via the interface, for example
        /// </summary>
        /// <param name="a"></param>
        void RemoveCallback(Action<object> a);
    }
}

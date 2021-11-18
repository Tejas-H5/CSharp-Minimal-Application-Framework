using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF
{
    public class Property<T> 
    {
        T _value;
        Func<T, T> _validator;

        /// <summary>
        /// The validation function may constrain the value of T by returning something, or throw an exception, 
        /// in which case no changes will be made to the value.
        /// </summary>
        public Property(T defaultValue, Func<T,T> validator = null)
        {
            _value = defaultValue;
            _validator = validator;
        }

        public T Value
        {
            get {
                return _value;
            }
            set {
                _value = value;

                try
                {
                    if (_validator != null)
                    {
                        _value = _validator(value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Setting property failed: " + e);
                }
            }
        }
    }
}

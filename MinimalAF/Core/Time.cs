using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF
{
    public static class Time
    {
        internal static float _deltaTime;
        public static float DeltaTime {
            get {
                return _deltaTime;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components.MouseInput
{
    public class MouseEventArgs
    {
        public bool Handled { get; set; } = false;

        public void Reset()
        {
            Handled = false;
        }
    }
}

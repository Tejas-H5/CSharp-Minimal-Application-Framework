using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF
{
	public class WindowKeyboardInput
	{
		Element _currentlyFocused = null;

		public void FocusElement(Element el)
		{
			_currentlyFocused = el;
		}

		public bool IsFocused(Element el)
		{
			return el == _currentlyFocused;
		}
	}
}

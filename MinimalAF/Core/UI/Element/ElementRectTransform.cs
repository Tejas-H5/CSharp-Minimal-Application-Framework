using System;
using System.Drawing;

namespace MinimalAF
{
	public partial class Element
	{
		public RectTransform RectTransform { get { return _rectTransform; } }

		public virtual Rect2D GetParentRect()
		{
			if (Parent != null)
			{
				return Parent.RectTransform.Rect;
			}

			throw new NotImplementedException("Parent wasn't hooked up properly. " +
				"Or you called GetParentRect on a 'Window' element. which shouldn't have any parents");
		}

		public float Width { get { return RectTransform.Rect.Width; } }
		public float Height { get { return RectTransform.Rect.Height; } }
		public float Left { get { return RectTransform.Rect.Left; } }
		public float Bottom { get { return RectTransform.Rect.Bottom; } }
		public float Right { get { return RectTransform.Rect.Right; } }
		public float Top { get { return RectTransform.Rect.Top; } }
	}
}

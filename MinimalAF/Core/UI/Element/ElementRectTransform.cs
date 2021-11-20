using System;
using System.Drawing;

namespace MinimalAF
{
	public partial class Element
	{
		public RectTransform RectTransform { get { return _rectTransform; } }

		public Rect2D Rect {
			get { return _rectTransform.Rect; }
			set {
				_rectTransform.Rect = value;
			}
		}

		public virtual Rect2D GetParentRect()
		{
			if (Parent != null)
			{
				return Parent.Rect;
			}

			throw new NotImplementedException("Parent wasn't hooked up properly. " +
				"Or you called GetParentRect on a 'Window' element. which shouldn't have any parents");
		}

		public float Width { get { return Rect.Width; } }
		public float Height { get { return Rect.Height; } }
		public float Left { get { return Rect.Left; } }
		public float Bottom { get { return Rect.Bottom; } }
		public float Right { get { return Rect.Right; } }
		public float Top { get { return Rect.Top; } }


		public PointF AnchoredPositionAbs {
			get {
				return _rectTransform.AnchoredPositionAbs;
			}
		}

		public PointF NormalizedCenter {
			get { return _rectTransform.NormalizedCenter; }
			set { _rectTransform.NormalizedCenter = value; }
		}

		public Rect2D AbsoluteOffset {
			get { return _rectTransform.AbsoluteOffset; }
		}

		public Rect2D NormalizedAnchoring {
			get { return _rectTransform.NormalizedAnchoring; }
		}

		public float AbsCenterX {
			get {
				return Rect.X0 + _rectTransform.NormalizedCenter.X * Rect.Width;
			}
		}

		public float AbsCenterY {
			get {
				return Rect.Y0 + _rectTransform.NormalizedCenter.Y * Rect.Height;
			}
		}
	}
}

namespace MinimalAF
{
	/// <summary>
	/// The reason why I am using extension methods is so that if we have a class T that inherits from Element,
	/// and we call OffsetsX(whatever) on it, it will still return a type of T and not Element
	/// </summary>
	public static class ElementTransformExtensions
	{
		public static T OffsetsX<T>(this T element, float left, float right) where T : Element
		{
			element.RectTransform.OffsetsX(left, right);
			return element;
		}

		public static T OffsetsY<T>(this T element, float bottom, float top) where T : Element
		{
			element.RectTransform.OffsetsY(bottom, top);
			return element;
		}

		public static T Offsets<T>(this T element, float offset) where T : Element
		{
			return Offsets(element, new Rect2D(offset, offset, offset, offset));
		}

		public static T Offsets<T>(this T element, Rect2D pos) where T : Element
		{
			element.RectTransform.Offsets(pos);
			return element;
		}

		public static T Offsets<T>(this T element, float left, float bottom, float right, float top) where T : Element
		{
			return Offsets(element, new Rect2D(left, bottom, right, top));
		}

		public static T PosSizeX<T>(this T element, float x, float width) where T : Element
		{
			element.RectTransform.PosSizeX(x, width);
			return element;
		}

		public static T PosSizeY<T>(this T element, float y, float height) where T : Element
		{
			element.RectTransform.PosSizeY(y, height);
			return element;
		}

		public static T Pos<T>(this T element, float x, float y) where T : Element
		{
			element.RectTransform.PosSize(x, y, element.RectTransform.Width, element.RectTransform.Height);
			return element;
		}

		public static T PosX<T>(this T element, float x) where T : Element
		{
			element.RectTransform.PosSizeX(x, element.RectTransform.Width);
			return element;
		}

		public static T PosY<T>(this T element, float y) where T : Element
		{
			element.RectTransform.PosSizeY(y, element.RectTransform.Height);
			return element;
		}

		public static T SetWidth<T>(this T element, float width) where T : Element
		{
			element.RectTransform.SetWidth(width);
			return element;
		}

		public static T SetHeight<T>(this T element, float height) where T : Element
		{
			element.RectTransform.SetWidth(height);
			return element;
		}


		public static T PosSize<T>(this T element, float x, float y, float width, float height) where T : Element
		{
			element.RectTransform.PosSize(x, y, width, height);
			return element;
		}

		public static T AnchorsX<T>(this T element, float left, float right) where T : Element
		{
			element.RectTransform.AnchorsX(left, right);
			return element;
		}

		public static T AnchorsY<T>(this T element, float bottom, float top) where T : Element
		{
			element.RectTransform.AnchorsY(bottom, top);
			return element;
		}

		public static T Anchors<T>(this T element, Rect2D anchor) where T : Element
		{
			element.RectTransform.Anchors(anchor);
			return element;
		}

		public static T Anchors<T>(this T element, float left, float bottom, float right, float top) where T : Element
		{
			element.RectTransform.Anchors(new Rect2D(left, bottom, right, top));
			return element;
		}

		public static T AnchoredPosX<T>(this T element, float x) where T : Element
		{
			element.RectTransform.AnchoredPosX(x);
			return element;
		}

		public static T AnchoredPosY<T>(this T element, float y) where T : Element
		{
			element.RectTransform.AnchoredPosY(y);
			return element;
		}

		public static T AnchoredCenterX<T>(this T element, float x) where T : Element
		{
			element.RectTransform.AnchoredCenterX(x);
			return element;
		}

		public static T AnchoredCenterY<T>(this T element, float y) where T : Element
		{
			element.RectTransform.AnchoredCenterY(y);
			return element;
		}

		/// <summary>
		///  Set the anchoredCenter before you set this.
		/// </summary>
		public static T AnchoredPos<T>(this T element, float x, float y) where T : Element
		{
			element.RectTransform.AnchoredPos(x, y);
			return element;
		}

		/// <summary>
		/// Set this before you set the anchored position
		/// </summary>
		public static T AnchoredCenter<T>(this T element, float x = 0.5f, float y = 0.5f) where T : Element
		{
			element.RectTransform.AnchoredCenter(x, y);
			return element;
		}

		public static T AnchoredPosCenter<T>(this T element, float x, float y, float centerX = 0.5f, float centerY = 0.5f) where T : Element
		{
			AnchoredCenter(element, centerX, centerY);
			AnchoredPos(element, x, y);
			return element;
		}

		public static T AnchoredPosCenterX<T>(this T element, float x, float centerX) where T : Element
		{
			AnchoredCenterX(element, centerX);
			AnchoredPosX(element, x);
			return element;
		}

		public static T AnchoredPosCenterY<T>(this T element, float y, float centerY) where T : Element
		{
			AnchoredCenterY(element, centerY);
			AnchoredPosY(element, y);
			return element;
		}
	}
}

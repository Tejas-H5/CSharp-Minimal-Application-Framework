namespace MinimalAF
{
	public class AspectRatioElement : Element
    {
        float _widthToHeight;

        public AspectRatioElement(float widthToHeight)
        {
            _widthToHeight = widthToHeight;
        }

        public override void OnResize()
        {
            Rect2D parentRect = _parent.Rect;

            float wantedWidth = parentRect.Height * _widthToHeight;
            bool shouldDriveHeight = wantedWidth > parentRect.Width;

            if (shouldDriveHeight)
            {
                float wantedHeight = parentRect.Width * (1.0f / _widthToHeight);

                RectTransform.SetHeight(wantedHeight);
            }
            else
            {
                RectTransform.SetWidth(wantedWidth);
            }
        }
    }
}

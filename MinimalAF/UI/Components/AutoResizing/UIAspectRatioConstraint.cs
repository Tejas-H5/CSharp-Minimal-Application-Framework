using MinimalAF.Datatypes;

namespace MinimalAF.UI
{
    public class UIAspectRatioConstraint : UIComponent
    {
        float _widthToHeight;

        public UIAspectRatioConstraint(float aspectRatio)
        {
            _widthToHeight = aspectRatio;
        }


        public override void OnResize()
        {
            Rect2D parentRect = _parent.Rect;

            float wantedWidth = parentRect.Height * _widthToHeight;
            bool shouldDriveHeight = wantedWidth > parentRect.Width;

            if (shouldDriveHeight)
            {
                float wantedHeight = parentRect.Width * (1.0f / _widthToHeight);

                _parent.RectTransform.SetHeight(wantedHeight);
            }
            else
            {
                _parent.RectTransform.SetWidth(wantedWidth);
            }
        }

        public override UIComponent Copy()
        {
            return new UIAspectRatioConstraint(_widthToHeight);
        }
    }
}

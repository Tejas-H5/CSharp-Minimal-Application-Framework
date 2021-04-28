using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components.AutoResizing
{
    public class UIAspectRatioConstraint : UIComponent
    {
        float _widthToHeight;

        public UIAspectRatioConstraint(float aspectRatio)
        {
            _widthToHeight = aspectRatio;
            _parent.RectTransform.OnDataChanged += OnResize;
        }

        public override UIComponent Copy()
        {
            return new UIAspectRatioConstraint(_widthToHeight);
        }

        private void OnResize(UIRectTransform obj)
        {
            Rect2D parentRect = _parent.Rect;
            Rect2D wantedRect = _parent.Rect;

            if (_widthToHeight * parentRect.Height < parentRect.Width)
            {
                //The height is fine, the width needs to be changed
                float wantedWidth = parentRect.Height * _widthToHeight;
                float centerX = _parent.RectTransform.NormalizedCenter.X;

                float deltaW = parentRect.Width - wantedWidth;
                wantedRect.X0 += deltaW * centerX;
                wantedRect.X1 -= deltaW * (1.0f - centerX);
            }
            else
            {
                //The width is fine, the height needs to be changed
                float wantedHeight = parentRect.Width * (1.0f / _widthToHeight);
                float centerY = _parent.RectTransform.NormalizedCenter.Y;

                float deltaH = parentRect.Height - wantedHeight;

                wantedRect.Y0 += deltaH * centerY;
                wantedRect.Y1 -= deltaH * (1.0f - centerY);
            }

            _parent.RectTransform.Rect = wantedRect;
        }


    }
}

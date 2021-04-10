using RenderingEngine.Datatypes;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Components
{
    public class UIAspectRatioPreserver : UIComponent
    {
        float _widthToHeight;

        public UIAspectRatioPreserver(float aspectRatio)
        {
            _widthToHeight = aspectRatio;
        }

        public override void OnResize()
        {
            Rect2D parentRect = _parent.Rect;
            Rect2D wantedRect = _parent.Rect;

            if ((4.0f / 3.0f) * parentRect.Height < parentRect.Width)
            {
                //The height is fine, the width needs to be changed
                float wantedWidth = (parentRect.Height) * _widthToHeight;
                float centerX = _parent.RectTransform.Center.X;

                float deltaW = parentRect.Width - wantedWidth;
                wantedRect.X0 += deltaW * (centerX);
                wantedRect.X1 -= deltaW * (1.0f - centerX);
            }
            else
            {
                //The width is fine, the height needs to be changed
                float wantedHeight = (parentRect.Width) * (1.0f / _widthToHeight);
                float centerY = _parent.RectTransform.Center.Y;

                float deltaH = parentRect.Height - wantedHeight;

                wantedRect.Y0 += deltaH * (centerY);
                wantedRect.Y1 -= deltaH * (1.0f - centerY);
            }

            _parent.RectTransform.Rect = wantedRect;
        }
    }
}

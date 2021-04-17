using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components.AutoResizing.Uncompleted
{
    public class UIDimensionConstraint : UIComponent
    {
        public UIDimensionConstraint(float minWidth, float minHeight, float maxWidth, float maxHeight)
            : this(new Rect2D(minWidth, minHeight, maxWidth, maxHeight))
        { }

        Rect2D _boundsRect;

        public UIDimensionConstraint(Rect2D boundsRect)
        {
            _boundsRect = boundsRect;
        }


        public override void OnResize()
        {
            Rect2D wantedRect = _parent.Rect;

            float width = wantedRect.X1 - wantedRect.X0;
            float minWidth = _boundsRect.X0;
            float maxWidth = _boundsRect.X1;
            float centerX = _parent.RectTransform.NormalizedCenter.X;

            if (minWidth > 0 && width < minWidth)
            {
                wantedRect.X1 = wantedRect.X0 + (1f - centerX) * minWidth;
                wantedRect.X0 = wantedRect.X1 - centerX * minWidth;
            }
            else if (maxWidth > 0 && width > maxWidth)
            {
                wantedRect.X1 = wantedRect.X0 + (1f - centerX) * maxWidth;
                wantedRect.X0 = wantedRect.X1 - centerX * maxWidth;
            }

            float height = wantedRect.Y1 - wantedRect.Y0;
            float minHeight = _boundsRect.Y0;
            float maxHeight = _boundsRect.Y1;
            float centerY = _parent.RectTransform.NormalizedCenter.Y;

            if (minHeight > 0 && height < minHeight)
            {
                wantedRect.Y1 = wantedRect.Y0 + (1f - centerY) * minHeight;
                wantedRect.Y0 = wantedRect.Y1 - centerY * minHeight;
            }
            else if (maxHeight > 0 && height > maxHeight)
            {
                wantedRect.Y1 = wantedRect.Y0 + (1f - centerY) * maxHeight;
                wantedRect.Y0 = wantedRect.Y1 - centerY * maxHeight;
            }

            _parent.RectTransform.Rect = wantedRect;
        }
    }
}

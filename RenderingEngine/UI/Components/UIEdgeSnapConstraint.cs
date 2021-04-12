using RenderingEngine.Datatypes;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components
{
    public enum UIRectEdgeSnapEdge
    {
        Bottom,
        Left,
        Top,
        Right
    }

    public class UIEdgeSnapConstraint : UIComponent
    {
        UIElement _other;
        UIRectEdgeSnapEdge _mine;
        UIRectEdgeSnapEdge _theirs;

        //Assumes that the UIElement it is assigned to 
        // is using RectOffset and not PositionSize
        public UIEdgeSnapConstraint(UIElement other, UIRectEdgeSnapEdge mine, UIRectEdgeSnapEdge theirs)
        {
            _other = other;
            _mine = mine;
            _theirs = theirs;
        }


        public override void OnResize()
        {
            Rect2D wantedRect = _parent.Rect;
            Rect2D otherRect = _other.Rect;

            float newValue;

            switch (_theirs)
            {
                case UIRectEdgeSnapEdge.Bottom:
                    newValue = otherRect.Y0;
                    break;
                case UIRectEdgeSnapEdge.Left:
                    newValue = otherRect.X0;
                    break;
                case UIRectEdgeSnapEdge.Top:
                    newValue = otherRect.Y1;
                    break;
                case UIRectEdgeSnapEdge.Right:
                    newValue = otherRect.X1;
                    break;
                default:
                    newValue = 0;
                    break;
            }

            switch (_mine)
            {
                case UIRectEdgeSnapEdge.Bottom:
                    wantedRect.Y0 = newValue + _parent.RectOffset.Y0;
                    break;
                case UIRectEdgeSnapEdge.Left:
                    wantedRect.X0 = newValue + _parent.RectOffset.X0;
                    break;
                case UIRectEdgeSnapEdge.Top:
                    wantedRect.Y1 = newValue - _parent.RectOffset.Y1;
                    break;
                case UIRectEdgeSnapEdge.Right:
                    wantedRect.X1 = newValue - _parent.RectOffset.X1;
                    break;
                default:
                    break;
            }

            _parent.RectTransform.Rect = wantedRect;
        }
    }
}

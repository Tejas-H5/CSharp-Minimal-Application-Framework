using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI.Components.AutoResizing
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
            _other.RectTransform.OnDataChanged += OnOtherResized;
            _mine = mine;
            _theirs = theirs;
        }

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);
        }

        protected override void OnRectTransformResize(UIRectTransform rtf)
        {
            OnOtherResized(_other.RectTransform);
        }

        public void OnOtherResized(UIRectTransform rtf)
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
                    wantedRect.Y0 = newValue;
                    break;
                case UIRectEdgeSnapEdge.Left:
                    wantedRect.X0 = newValue;
                    break;
                case UIRectEdgeSnapEdge.Top:
                    wantedRect.Y1 = newValue;
                    break;
                case UIRectEdgeSnapEdge.Right:
                    wantedRect.X1 = newValue;
                    break;
                default:
                    break;
            }

            _parent.Rect = wantedRect;
        }
    }
}

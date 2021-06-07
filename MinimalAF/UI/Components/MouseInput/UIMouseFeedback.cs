using MinimalAF.Datatypes;
using MinimalAF.Logic;
using MinimalAF.UI;
using MinimalAF.UI;

namespace MinimalAF.UI
{
    [RequiredComponents(typeof(UIRect), typeof(UIMouseListener))]
    public class UIMouseFeedback : UIComponent
    {
        public Color4 HoverColor { get; set; }
        public Color4 ClickedColor { get; set; }

        public UIMouseFeedback(Color4 hoverColor, Color4 clickedColor)
        {
            HoverColor = hoverColor;
            ClickedColor = clickedColor;
        }

        private UIMouseListener _mouseListner;
        private UIRect _bgRect;

        public override void SetParent(UIElement parent)
        {
            if (_mouseListner != null)
            {
                _mouseListner.OnMouseEnter -= _mouseListner_OnMouseOver;
                _mouseListner.OnMouseOver -= _mouseListner_OnMouseOver;
                _mouseListner.OnMouseLeave -= _mouseListner_OnMouseLeave;
            }

            base.SetParent(parent);

            _mouseListner = _parent.GetComponentOfType<UIMouseListener>();
            _bgRect = _parent.GetComponentOfType<UIRect>();


            _mouseListner.OnMouseEnter += _mouseListner_OnMouseOver;
            _mouseListner.OnMouseOver += _mouseListner_OnMouseOver;
            _mouseListner.OnMouseLeave += _mouseListner_OnMouseLeave;
        }

        private void _mouseListner_OnMouseOver(MouseEventArgs e)
        {
            if (Input.IsMouseHeld(MouseButton.Left))
            {
                _bgRect.Color = ClickedColor;
            }
            else
            {
                _bgRect.Color = HoverColor;
            }

            e.Handled = true;
        }

        private void _mouseListner_OnMouseLeave(MouseEventArgs e)
        {
            _bgRect.Color = _bgRect.InitialColor;
        }

        public override UIComponent Copy()
        {
            return new UIMouseFeedback(HoverColor, ClickedColor);
        }
    }
}

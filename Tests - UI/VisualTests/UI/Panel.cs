using MinimalAF.Datatypes;
using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.UI
{
    public class Panel : Element
    {
        Color4 _color, _hoverColor, _clickColor;

        public Panel(Color4 color, Color4 hoverColor, Color4 clickColor)
        {
            _color = color;
            _drawColor = color;

            _hoverColor = hoverColor;
            _clickColor = clickColor;
        }

        Color4 _drawColor;

        public override void OnRender()
        {
            CTX.SetDrawColor(new Color4(0,1));
            CTX.DrawRectOutline(1, Rect);

            CTX.SetDrawColor(_drawColor);
            CTX.DrawRect(Rect);

            base.OnRender();
        }


        public override bool OnProcessEvents()
        {
            if (Input.Mouse.IsOver(Rect))
            {
                if (Input.Mouse.IsAnyDown)
                {
                    _drawColor = _clickColor;
                }
                else
                {
                    _drawColor = _hoverColor;
                }

                return true;
            }


            _drawColor = _color;
            return false;
        }
    }
}

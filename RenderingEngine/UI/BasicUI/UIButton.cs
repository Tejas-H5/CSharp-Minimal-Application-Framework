using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.BasicUI
{
    public class UIButton : UIElement
    {
        Action _onClickEvent;
        Action _onReleaseEvent;
        string _text;

        UIBackgroundRect _backgroundRect;
        public UIBackgroundRect BackgroundRect { get { return _backgroundRect; } }


        public event Action OnClicked {
            add {
                lock (this)
                {
                    _onClickEvent += value;
                }
            }
            remove {
                lock (this)
                {
                    _onClickEvent -= value;
                }
            }
        }

        public event Action OnReleased {
            add {
                lock (this)
                {
                    _onReleaseEvent += value;
                }
            }
            remove {
                lock (this)
                {
                    _onReleaseEvent -= value;
                }
            }
        }

        public UIButton(string text)
        {
            _backgroundRect = new UIBackgroundRect(this);
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

        }


        public override void Draw(double deltaTime)
        {
        }

    }
}

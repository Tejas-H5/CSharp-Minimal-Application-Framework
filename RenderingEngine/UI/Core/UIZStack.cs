namespace MinimalAF.UI.Core
{
    public class UIZStack : UIElement
    {
        public UIZStack()
        {
        }


        public override void UpdateChildren(double deltaTime)
        {
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                if (_children[i].IsVisible)
                {
                    _children[i].Update(deltaTime);
                }
            }
        }

        internal override bool ProcessChildEvents()
        {
            bool res = false;
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                if (_children[i].IsVisible)
                {
                    res = _children[i].ProcessChildEvents();
                    break;
                }
            }

            if (res)
                return true;

            return ProcessComponentEvents();
        }
    }
}

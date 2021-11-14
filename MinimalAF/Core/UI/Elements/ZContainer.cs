using System.Collections.Generic;

namespace MinimalAF
{
    public class ZContainer : Element
    {
        private List<Element> _children = new List<Element>();

        public void Push(Element el)
        {
            _children.Add(el);
        }

        public void Pop(Element el)
        {
            if (_children.Count == 0)
                return;

            _children.RemoveAt(_children.Count - 1);
        }

        public override bool ProcessEvents()
        {
            if (_children.Count < 1)
                return false;

            return _children[_children.Count - 1].ProcessEvents();
        }
    }
}

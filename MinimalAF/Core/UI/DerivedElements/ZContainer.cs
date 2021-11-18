using System.Collections.Generic;

namespace MinimalAF
{
    public class ZContainer : Element
    {
        private List<Element> _stack = new List<Element>();
		public override bool SingleChild => true;

		public ZContainer(Element child)
		{
			this.SetChildren(child);
		}

        public void Push(Element el)
        {
            _stack.Add(el);
        }

        public void Pop(Element el)
        {
			// Don't pop all elements
            if (_stack.Count == 1)
                return;

            _stack.RemoveAt(_stack.Count - 1);
        }

        public override void OnUpdate()
        {
            if (_stack.Count < 1)
                return;

            _stack[_stack.Count - 1].Update();
        }
    }
}

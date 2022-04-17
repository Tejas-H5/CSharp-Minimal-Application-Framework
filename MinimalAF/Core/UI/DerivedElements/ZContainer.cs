using System.Collections.Generic;

namespace MinimalAF {
    public class ZContainer : Element {
        private List<Element> stack = new List<Element>();
        public override bool SingleChild => true;

        public ZContainer(Element child) {
            this.SetChildren(child);
        }

        public void Push(Element el) {
            stack.Add(el);
        }

        public void Pop(Element el) {
            // Don't pop all elements
            if (stack.Count == 1)
                return;

            stack.RemoveAt(stack.Count - 1);
        }

        public override void OnUpdate() {
            if (stack.Count < 1)
                return;

            stack[stack.Count - 1].UpdateSelfAndChildren(screenRect);
        }
    }
}

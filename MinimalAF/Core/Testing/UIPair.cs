// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    class UIPair : Element {
        public UIPair(Element el1, Element el2) {
            SetChildren(el1, el2);
        }

        public override void OnRender() {
            SetDrawColor(Color4.Black);

            DrawRectOutline(1, this[0].RelativeRect);
            DrawRectOutline(1, this[1].RelativeRect);
        }

        public override void OnLayout() {
            LayoutTwoSplit(this[0], this[1], Direction.Right, VW(0.5f));
            LayoutChildren();
        }
    }
}

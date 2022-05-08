namespace MinimalAF {
    class UIPair : Element {
        public UIPair(Element el1, Element el2) {
            SetChildren(el1, el2);
        }

        public override void OnRender() {
            SetDrawColor(Color4.Black);
            DrawRectOutline(1, 0, 0, Width, Height);

            var e = this[1];
            e.ResetCoordinates();
            DrawRectOutline(1, 0, 0, e.Width, e.Height);
        }

        public override void OnLayout() {
            LayoutX0X1(Children, 0, VW(1));
            float height = LayoutLinear(Children, Direction.Down);
            RelativeRect = RelativeRect
                .ResizedHeight(height, 1);
        }
    }
}

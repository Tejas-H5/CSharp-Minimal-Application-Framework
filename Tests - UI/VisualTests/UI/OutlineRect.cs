namespace UIVisualTests {
    public class OutlineRect : Element {
        Color4 col;
        float thickness;
        public OutlineRect(Color4 col, float thickness) {
            this.thickness = thickness;
            this.col = col;
        }

        public override void OnRender() {
            SetDrawColor(col);
            DrawRectOutline(thickness, 0, 0, VW(1), VH(1));
        }
    }
}

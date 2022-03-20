namespace MinimalAF.VisualTests.UI {
    public class OutlineRect : Element {
        Color4 _col;
        float _thickness;
        public OutlineRect(Color4 col, float thickness) {
            _thickness = thickness;
            _col = col;
        }

        public override void OnRender() {
            SetDrawColor(_col);
            RectOutline(_thickness, 0, 0, VW(1), VH(1));
        }
    }
}

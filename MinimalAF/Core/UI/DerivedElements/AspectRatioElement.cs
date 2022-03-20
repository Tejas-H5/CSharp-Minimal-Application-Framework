namespace MinimalAF {
    public class AspectRatioElement : Element {
        float _widthToHeight;

        public AspectRatioElement(float widthToHeight) {
            _widthToHeight = widthToHeight;
        }

        public override void OnLayout() {
			LayoutAspectRatio(_widthToHeight);
        }
    }
}

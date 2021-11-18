namespace MinimalAF.Rendering.ImmediateMode
{
	//TODO: remove if not necessary
	public abstract class GeometryDrawer
    {
        //Will be used by child classes to draw their outlines
        protected PolyLineDrawer _outlineDrawer;
        public void SetPolylineDrawer(PolyLineDrawer polyLineDrawer)
        {
            _outlineDrawer = polyLineDrawer;
        }
    }
}

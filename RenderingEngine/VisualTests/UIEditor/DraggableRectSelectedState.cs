namespace RenderingEngine.VisualTests.UIEditor
{
    class DraggableRectSelectedState
    {
        public UIDraggableRect SelectedRect {
            get {
                return _selRect;
            }

            set {
                _selRect = value;
                selectionChanged = true;
            }
        }

        UIDraggableRect _selRect;
        public bool selectionChanged = false;
        public float DimensionSnap = 5f;

    }
}

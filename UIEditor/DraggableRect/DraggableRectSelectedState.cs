using System;

namespace UICodeGenerator.DraggableRect
{
    public class DraggableRectSelectedState
    {
        public event Action<UIDraggableRect> OnSelectionChanged;

        public UIDraggableRect SelectedRect {
            get {
                return _selRect;
            }
            set {
                if (value != _selRect)
                {
                    _selRect = value;
                    InvokeChangeEvent();
                }
            }
        }

        bool _isInvoking = false;

        public void InvokeChangeEvent()
        {
            if (_isInvoking)
                return;

            _isInvoking = true;
            OnSelectionChanged?.Invoke(_selRect);
            _isInvoking = false;
        }


        UIDraggableRect _selRect;

        public float DimensionSnap = 5f;
        public float AnchorSnap = 0.5f;
    }
}

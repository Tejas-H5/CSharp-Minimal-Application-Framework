using System;

namespace RenderingEngine.VisualTests.UIEditor
{
    class DraggableRectSelectedState
    {
        public event Action<UIDraggableRect> OnSelectionChanged;

        public UIDraggableRect SelectedRect {
            get {
                return _selRect;
            }

            set {
                _selRect = value;
                selectionChanged = true;
            }
        }

        bool _isInvoking = false;

        void InvokeChangeEvent()
        {
            if (_isInvoking)
                return;
            
            if (selectionChanged && _selRect != null)
            {
                _isInvoking = true;
                OnSelectionChanged?.Invoke(_selRect);
                _isInvoking = false;
            }
        }

        public bool SelectionChanged {
            get {
                return selectionChanged;
            }

            set {
                selectionChanged = value;
                InvokeChangeEvent();
            }
        }

        UIDraggableRect _selRect;

        public bool selectionChanged = false;
        public float DimensionSnap = 5f;
    }
}

using System;

namespace UICodeGenerator.DraggableRect
{
    public class DraggableRectSelectedState
    {
        public event Action<UIElementEditor> OnSelectionChanged;

        public UIElementEditor SelectedEditorRect {
            get {
                return _selRect;
            }
            set {
                Console.WriteLine(LockSelection);

                if (LockSelection)
                    return;

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


        private UIElementEditor _selRect;

        public float DimensionSnap = 5f;
        public float AnchorSnap = 0.5f;
        public bool LockSelection = false;
    }
}

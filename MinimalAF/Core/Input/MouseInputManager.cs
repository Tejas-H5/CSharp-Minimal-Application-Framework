using System;

namespace MinimalAF {
    internal class MouseInputManager {
        OpenTKWindowWrapper _window;

        float _incomingWheelNotches = 0;
        float _wheelNotches = 0;

        bool[] _prevMouseButtonStates = new bool[3];
        bool[] _mouseButtonStates = new bool[3];

        bool _dragCancelled;
        bool _wasAnyDown = false;
        bool _anyHeld = false;
        bool _wasAnyHeld = false;
        bool _isAnyHeld = false;
        bool _wasDragging = false;
        bool _anyPressed = false;
        bool _anyReleased = false;

        float _dragStartX;
        float _dragStartY;
        float _dragDeltaX = 0;
        float _dragDeltaY = 0;
        //Mainly used to tell if we started dragging or not, and 
        //not meant to be an accurate representation of total distance dragged

        internal float WheelNotches {
            get {
                return _wheelNotches;
            }
        }

        internal bool[] ButtonStates {
            get {
                return _mouseButtonStates;
            }
        }
        internal bool[] PrevButtonStates {
            get {
                return _prevMouseButtonStates;
            }
        }

        internal bool IsAnyDown {
            get {
                return _anyHeld;
            }
        }
        internal bool IsAnyPressed {
            get {
                return _anyPressed;
            }
        }
        internal bool IsAnyReleased {
            get {
                return _anyReleased;
            }
        }

        // TODO low priority: Think of a better name for this private field (HAHAHA)
        private bool CurrentlyDragging {
            get {
                return _isAnyHeld && !_dragCancelled && ((MathF.Abs(DragDeltaX) + MathF.Abs(DragDeltaX)) > 1);
            }
        }

        internal bool StartedDragging => !_wasAnyHeld && _isAnyHeld;
        internal bool IsDragging => _wasAnyHeld && CurrentlyDragging;
        internal bool FinishedDragging => _wasAnyHeld && !_isAnyHeld;

        internal float X {
            get {
                return _window.MouseState.Position.X;
            }
        }
        internal float Y {
            get {
                return _window.Height - _window.MouseState.Position.Y;
            }
        }

        internal float XDelta {
            get {
                return _window.MouseState.Delta.X;
            }
        }
        internal float YDelta {
            get {
                return _window.MouseState.Delta.Y;
            }
        }

        internal float DragStartX {
            get {
                return _dragStartX;
            }
        }
        internal float DragStartY {
            get {
                return _dragStartY;
            }
        }
        internal float DragDeltaX {
            get {
                return _dragDeltaX;
            }
        }
        internal float DragDeltaY {
            get {
                return _dragDeltaY;
            }
        }

        internal MouseInputManager() {
        }

        private void OnWindowMouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj) {
            _incomingWheelNotches += obj.OffsetY;
        }

        internal bool IsOver(Rect rect) {
            return Intersections.IsInsideRect(X, Y, rect);
        }

        internal void Hook(OpenTKWindowWrapper window) {
            _window = window;
            _window.MouseWheel += OnWindowMouseWheel;
        }

        internal void Unhook() {
            if (_window != null)
                _window.MouseWheel -= OnWindowMouseWheel;
        }


        private void SwapInputBuffers() {
            bool[] temp = _prevMouseButtonStates;
            _prevMouseButtonStates = _mouseButtonStates;
            _mouseButtonStates = temp;
        }

        internal bool IsPressed(MouseButton b) {
            if (b == MouseButton.Any)
                return _anyPressed;

            return (!_prevMouseButtonStates[(int)b]) && _mouseButtonStates[(int)b];
        }

        internal bool IsReleased(MouseButton b) {
            if (b == MouseButton.Any)
                return _anyReleased;

            return _prevMouseButtonStates[(int)b] && (!_mouseButtonStates[(int)b]);
        }

        internal bool IsHeld(MouseButton b) {
            if (b == MouseButton.Any)
                return _anyHeld;

            return _mouseButtonStates[(int)b];
        }

        internal void CancelDrag() {
            _dragCancelled = true;
            SetDragDeltas(_dragStartX, _dragStartY);
        }

        private void SetDragDeltas(float x, float y) {
            _dragStartX = x;
            _dragStartY = y;
            _dragDeltaX = 0;
            _dragDeltaY = 0;
        }

        private void CalculateDragDeltas(float x, float y) {
            _dragDeltaY = y - _dragStartY;
            _dragDeltaX = x - _dragStartX;
        }

        internal void Update() {
            SwapInputBuffers();

            UpdateMousewheelNotches();

            UpdatePressedStates();

            UpdateDragDeltas();
        }

        private void UpdateDragDeltas() {
            if (!_isAnyHeld) {
                _dragCancelled = false;
            }

            if (StartedDragging) {
                SetDragDeltas(X, Y);
            } else if (_wasAnyHeld && _isAnyHeld && !_dragCancelled) {
                CalculateDragDeltas(X, Y);
            }
        }

        private void UpdatePressedStates() {
            _wasDragging = CurrentlyDragging;
            _wasAnyHeld = _isAnyHeld;
            _wasAnyDown = _anyHeld;

            _anyHeld = false;
            _anyPressed = false;
            _anyReleased = false;

            for (int i = 0; i < _mouseButtonStates.Length; i++) {
                _mouseButtonStates[i] = _window.MouseState.IsButtonDown(
                        (OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)i
                    );

                _anyHeld = _anyHeld || _mouseButtonStates[i];

                _anyPressed = _anyPressed || (!_prevMouseButtonStates[i] && _mouseButtonStates[i]);
                _anyReleased = _anyReleased || (_prevMouseButtonStates[i] && !_mouseButtonStates[i]);
            }

            _isAnyHeld = _wasAnyDown && _anyHeld;
        }

        private void UpdateMousewheelNotches() {
            _wheelNotches = _incomingWheelNotches;
            _incomingWheelNotches = 0;
        }
    }
}

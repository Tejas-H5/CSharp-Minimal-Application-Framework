using MinimalAF.Util;
using System;

namespace MinimalAF.Logic
{
    class MouseInputManager
    {
        WindowInstance _window;

        private void OnWindowMouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj)
        {
            if (obj.OffsetY < 0)
            {
                _incomingWheelNotches -= 1;
            }
            else if (obj.OffsetY > 0)
            {
                _incomingWheelNotches += 1;
            }
        }

        internal void Hook(WindowInstance window)
        {
            _window = window;
            _window.MouseWheel += OnWindowMouseWheel;
        }

        internal void Unhook()
        {
            if (_window != null)
                _window.MouseWheel -= OnWindowMouseWheel;
        }

        int _incomingWheelNotches = 0;
        int _wheelNotches = 0;

        public int WheelNotches {
            get { return _wheelNotches; }
        }

        bool[] _prevMouseButtonStates = new bool[3];
        bool[] _mouseButtonStates = new bool[3];
        public bool[] MouseButtonStates { get { return _mouseButtonStates; } }
        public bool[] PrevMouseButtonStates { get { return _prevMouseButtonStates; } }

        private void SwapInputBuffers()
        {
            bool[] temp = _prevMouseButtonStates;
            _prevMouseButtonStates = _mouseButtonStates;
            _mouseButtonStates = temp;
        }

        bool _dragCancelled;

        bool _wasAnyDown = false;
        bool _anyDown = false;
        bool _wasAnyHeld = false;
        bool _isAnyHeld = false;
        bool _wasDragging = false;
        bool _startedDragging = false;

        bool _anyClicked = false;
        bool _anyReleased = false;

        public bool IsMouseDownAny { get { return _anyDown; } }
        public bool IsMouseClickedAny { get { return _anyClicked; } }
        public bool IsMouseReleasedAny { get { return _anyReleased; } }

        public bool IsDragging {
            get {
                return (!_dragCancelled) && (_startedDragging || (_isAnyHeld && _wasAnyHeld && (_dragDisplacement > 1)));
            }
        }

        public float MouseX { get { return _window.MouseState.Position.X; } }
        public float MouseY { get { return _window.Height - _window.MouseState.Position.Y; } }

        public float MouseXDelta { get { return _window.MouseState.Delta.X; } }
        public float MouseYDelta { get { return _window.MouseState.Delta.Y; } }

        public float DragStartX { get { return _dragStartX; } }
        public float DragStartY { get { return _dragStartY; } }
        public float DragDeltaX { get { return _dragDeltaX; } }
        public float DragDeltaY { get { return _dragDeltaY; } }
        public float DragDisplacement { get { return _dragDisplacement; } }
        public bool WasDragging { get { return _wasDragging; } }
        public bool StartedDragging { get { return _startedDragging; } }

        float _dragStartX;
        float _dragStartY;
        float _dragDeltaX = 0;
        float _dragDeltaY = 0;
        //Mainly used to tell if we started dragging or not, and 
        //not meant to be an accurate representation of total distance dragged
        float _displacementStartX = 0;
        float _displacementStartY = 0;
        float _dragDisplacement = 0;

        public bool IsMouseClicked(MouseButton b)
        {
            return (!_prevMouseButtonStates[(int)b]) && _mouseButtonStates[(int)b];
        }

        public bool IsMouseReleased(MouseButton b)
        {
            return _prevMouseButtonStates[(int)b] && (!_mouseButtonStates[(int)b]);
        }

        public bool IsMouseDown(MouseButton b)
        {
            return _mouseButtonStates[(int)b];
        }

        public bool IsMouseHeld(MouseButton b)
        {
            return _prevMouseButtonStates[(int)b] && _mouseButtonStates[(int)b];
        }

        public void CancelDrag()
        {
            _dragCancelled = true;
            SetDragDeltas(_dragStartX, _dragStartY);
        }

        private void SetDragDeltas(float x, float y)
        {
            _dragStartX = x;
            _dragStartY = y;
            _dragDeltaX = 0;
            _dragDeltaY = 0;
            _dragDisplacement = 0;
            _displacementStartX = x;
            _displacementStartY = y;
            _startedDragging = true;
        }

        private void CalculateDragDeltas(float x, float y)
        {
            _dragDeltaY = y - _dragStartY;
            _dragDeltaX = x - _dragStartX;
            _dragDisplacement = MathF.Min(_dragDisplacement + MathUtilF.Mag(x - _displacementStartX, y - _displacementStartY), 100);
            _displacementStartX = x;
            _displacementStartY = y;
        }


        public void StartDrag(float x, float y)
        {
            _anyDown = true;
            _wasAnyDown = true;
            _isAnyHeld = true;
            _wasAnyHeld = true;
            _wasDragging = true;
            SetDragDeltas(x, y);
            CalculateDragDeltas(x, y);
        }

        public void Update()
        {
            SwapInputBuffers();

            _wheelNotches = _incomingWheelNotches;
            _incomingWheelNotches = 0;

            _wasDragging = IsDragging;
            _wasAnyHeld = _isAnyHeld;
            _wasAnyDown = _anyDown;
            _anyDown = false;
            _startedDragging = false;

            _anyClicked = false;
            _anyReleased = false;

            for (int i = 0; i < _mouseButtonStates.Length; i++)
            {
                _mouseButtonStates[i] = _window.MouseState.IsButtonDown(
                    (OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)i);
                _anyDown = _anyDown || _mouseButtonStates[i];

                _anyClicked = _anyClicked || (!_prevMouseButtonStates[i] && _mouseButtonStates[i]);
                _anyReleased = _anyReleased || (_prevMouseButtonStates[i] && !_mouseButtonStates[i]);
            }

            _isAnyHeld = _wasAnyDown && _anyDown;

            if (!_isAnyHeld)
            {
                _dragCancelled = false;
            }

            if (!_wasAnyHeld && _isAnyHeld)
            {
                SetDragDeltas(MouseX, MouseY);
            }
            else if (_wasAnyHeld && _isAnyHeld && !_dragCancelled)
            {
                CalculateDragDeltas(MouseX, MouseY);
            }
        }
    }
}

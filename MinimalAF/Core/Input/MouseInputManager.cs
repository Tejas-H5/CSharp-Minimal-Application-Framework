using MinimalAF.Util;
using System;

namespace MinimalAF
{
	public class MouseInputManager
    {
        OpenTKWindowWrapper _window;

        float _incomingWheelNotches = 0;
        float _wheelNotches = 0;

        bool[] _prevMouseButtonStates = new bool[3];
        bool[] _mouseButtonStates = new bool[3];

        bool _dragCancelled;
        bool _wasAnyDown = false;
        bool _anyDown = false;
        bool _wasAnyHeld = false;
        bool _isAnyHeld = false;
        bool _wasDragging = false;
        bool _startedDragging = false;
        bool _anyClicked = false;
        bool _anyReleased = false;

        float _dragStartX;
        float _dragStartY;
        float _dragDeltaX = 0;
        float _dragDeltaY = 0;
        //Mainly used to tell if we started dragging or not, and 
        //not meant to be an accurate representation of total distance dragged
        float _displacementStartX = 0;
        float _displacementStartY = 0;
        float _dragDisplacement = 0;

        public float WheelNotches {
            get { return _wheelNotches; }
        }

        public bool[] ButtonStates { get { return _mouseButtonStates; } }
        public bool[] PrevButtonStates { get { return _prevMouseButtonStates; } }

        public bool IsAnyDown { get { return _anyDown; } }
        public bool IsAnyPressed { get { return _anyClicked; } }
        public bool IsAnyReleased { get { return _anyReleased; } }

        public bool IsDragging {
            get {
                return (!_dragCancelled) && (_startedDragging || (_isAnyHeld && _wasAnyHeld && (_dragDisplacement > 1)));
            }
        }

        public float X { get { return _window.MouseState.Position.X; } }
        public float Y { get { return _window.Height - _window.MouseState.Position.Y; } }

        public float XDelta { get { return _window.MouseState.Delta.X; } }
        public float YDelta { get { return _window.MouseState.Delta.Y; } }

        public float DragStartX { get { return _dragStartX; } }
        public float DragStartY { get { return _dragStartY; } }
        public float DragDeltaX { get { return _dragDeltaX; } }
        public float DragDeltaY { get { return _dragDeltaY; } }
        public float DragDisplacement { get { return _dragDisplacement; } }
        public bool WasDragging { get { return _wasDragging; } }
        public bool StartedDragging { get { return _startedDragging; } }

        internal MouseInputManager() { }

        private void OnWindowMouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj)
        {
            _incomingWheelNotches += obj.OffsetY;
        }

        public bool IsOver(Rect2D rect)
        {
            return Intersections.IsInsideRect(X, Y, rect);
        }

        internal void Hook(OpenTKWindowWrapper window)
        {
            _window = window;
            _window.MouseWheel += OnWindowMouseWheel;
        }

        internal void Unhook()
        {
            if (_window != null)
                _window.MouseWheel -= OnWindowMouseWheel;
        }


        private void SwapInputBuffers()
        {
            bool[] temp = _prevMouseButtonStates;
            _prevMouseButtonStates = _mouseButtonStates;
            _mouseButtonStates = temp;
        }

        public bool IsPressed(MouseButton b)
        {
            return (!_prevMouseButtonStates[(int)b]) && _mouseButtonStates[(int)b];
        }

        public bool IsReleased(MouseButton b)
        {
            return _prevMouseButtonStates[(int)b] && (!_mouseButtonStates[(int)b]);
        }

        public bool IsDown(MouseButton b)
        {
            return _mouseButtonStates[(int)b];
        }

        public bool IsDownFor2PlusFrames(MouseButton b)
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

            UpdateMousewheelNotches();

            UpdatePressedStates();

            UpdateDragDeltas();
        }

        private void UpdateDragDeltas()
        {
            if (!_isAnyHeld)
            {
                _dragCancelled = false;
            }

            if (!_wasAnyHeld && _isAnyHeld)
            {
                SetDragDeltas(X, Y);
            }
            else if (_wasAnyHeld && _isAnyHeld && !_dragCancelled)
            {
                CalculateDragDeltas(X, Y);
            }
        }

        private void UpdatePressedStates()
        {
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
                        (OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)i
                    );

                _anyDown = _anyDown || _mouseButtonStates[i];

                _anyClicked = _anyClicked || (!_prevMouseButtonStates[i] && _mouseButtonStates[i]);
                _anyReleased = _anyReleased || (_prevMouseButtonStates[i] && !_mouseButtonStates[i]);
            }

            _isAnyHeld = _wasAnyDown && _anyDown;
        }

        private void UpdateMousewheelNotches()
        {
            _wheelNotches = _incomingWheelNotches;
            _incomingWheelNotches = 0;
        }
    }
}

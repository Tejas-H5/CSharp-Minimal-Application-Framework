using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderingEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Logic
{
    class MouseInputManager
    {
        WindowInstance _window;
        public MouseInputManager(WindowInstance window)
        {
            _window = window;
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

        bool _wasAnyClicked = false;
        bool _anyClicked = false;
        bool _wasAnyHeld = false;
        bool _isAnyHeld = false;
        bool _wasDragging = false;

        public bool AnyClicked { get { return _anyClicked; } }
        public bool IsDragging {
            get {
                return _isAnyHeld && _wasAnyHeld && (_dragDisplacement > 5);
            }
        }

        public float MouseX { get { return _mouseX; } }
        public float MouseY { get { return _mouseY; } }

        float _mouseX;
        float _mouseY;
        float _prevMouseX;
        float _prevMouseY;
        public float MouseXDelta { get { return MouseX - _prevMouseX; } }
        public float MouseYDelta { get { return MouseY - _prevMouseY; } }

        public float DragStartX { get { return _dragStartX; } }
        public float DragStartY { get { return _dragStartY; } }
        public float DragDeltaX { get { return _dragDeltaX; } }
        public float DragDeltaY { get { return _dragDeltaY; } }
        public float DragDisplacement { get { return _dragDisplacement; } }
        public bool WasDragging { get { return _wasDragging; } }

        float _dragStartX;
        float _dragStartY;
        float _dragDeltaX = 0;
        float _dragDeltaY = 0;
        //Mainly used to tell if we started dragging or not, and 
        //not meant to be an accurate representation of total distance dragged
        float _displacementStartX = 0;
        float _displacementStartY = 0;
        float _dragDisplacement = 0;

        public bool MouseClicked(MouseButton b)
        {
            return (!_prevMouseButtonStates[(int)b]) && _mouseButtonStates[(int)b];
        }

        public bool MouseReleased(MouseButton b)
        {
            return _prevMouseButtonStates[(int)b] && (!_mouseButtonStates[(int)b]);
        }

        public bool MouseHeld(MouseButton b)
        {
            return _prevMouseButtonStates[(int)b] && _mouseButtonStates[(int)b];
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
        }

        private void CalculateDragDeltas(float x, float y)
        {
            _dragDeltaY = y - _dragStartY;
            _dragDeltaX = x - _dragStartX;
            _dragDisplacement = MathF.Min(_dragDisplacement + MathUtil.Mag(x - _displacementStartX, y - _displacementStartY), 100);
            _displacementStartX = x;
            _displacementStartY = y;
        }


        public void StartDrag(float x, float y)
        {
            _anyClicked = true;
            _wasAnyClicked = true;
            _isAnyHeld = true;
            _wasAnyHeld = true;
            _wasDragging = true;
            SetDragDeltas(x, y);
            CalculateDragDeltas(x, y);
        }

        public void Update()
        {
            SwapInputBuffers();
            _wasDragging = IsDragging;
            _wasAnyHeld = _isAnyHeld;
            _wasAnyClicked = _anyClicked;
            _anyClicked = false;

            for (int i = 0; i < 3; i++)
            {
                _mouseButtonStates[i] = _window.MouseState.IsButtonDown(
                    (OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)i);
                _anyClicked = _anyClicked || _mouseButtonStates[i];
            }

            _isAnyHeld = _wasAnyClicked && _anyClicked;

            _mouseX = _window.MousePosition.X;
            _mouseY = _window.Height - _window.MousePosition.Y;

            if (!_wasAnyHeld && _isAnyHeld)
            {
                SetDragDeltas(_mouseX, _mouseY);
            }
            else if (_wasAnyHeld && _isAnyHeld)
            {
                CalculateDragDeltas(_mouseX, _mouseY);
            }
        }
    }
}

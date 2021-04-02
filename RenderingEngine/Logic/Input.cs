using RenderingEngine.Datatypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Logic
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public static class Input
    {
        private static WindowInstance _window;
        internal static void Init(WindowInstance window)
        {
            _window = window;
            _mouseInputManager = new MouseInputManager(window);
        }

        internal static void Update()
        {
            _mouseInputManager.Update();
        }


        public static float MouseX { get { return _window.MousePosition.X; } }
        public static float MouseY { get { return _window.Height - _window.MousePosition.Y; } }

        // -------- mouseInputManager wrappers
        private static MouseInputManager _mouseInputManager;

        public static bool[] MouseButtonStates { get { return _mouseInputManager.MouseButtonStates; } }
        public static bool[] PrevMouseButtonStates { get { return _mouseInputManager.PrevMouseButtonStates; } }

        public static bool MouseClicked(MouseButton b)
        {
            return _mouseInputManager.MouseClicked(b);
        }

        public static bool MouseReleased(MouseButton b)
        {
            return _mouseInputManager.MouseReleased(b);
        }

        public static bool MouseHeld(MouseButton b)
        {
            return _mouseInputManager.MouseHeld(b);
        }

        public static void StartMouseDrag(float x, float y)
        {
            _mouseInputManager.StartDrag(x, y);
        }

        public static bool IsAnyClicked { get { return _mouseInputManager.AnyClicked; } }
        public static bool IsMouseDragging { get { return _mouseInputManager.IsDragging; } }
        public static bool WasMouseDragging { get { return _mouseInputManager.WasDragging; } }
        public static float DragStartX { get { return _mouseInputManager.DragStartX; } }
        public static float DragStartY { get { return _mouseInputManager.DragStartY; } }
        public static float DragDeltaX { get { return _mouseInputManager.DragDeltaX; } }
        public static float DragDeltaY { get { return _mouseInputManager.DragDeltaY; } }
        public static float DragDisplacement { get { return _mouseInputManager.DragDisplacement; } }
    }
}

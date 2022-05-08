using System;

namespace MinimalAF {
    public class MouseInputManager {
        OpenTKWindowWrapper window;

        float incomingWheelNotches = 0;
        float wheelNotches = 0;

        bool[] prevMouseButtonStates = new bool[3];
        bool[] mouseButtonStates = new bool[3];

        bool dragCancelled;
        bool wasAnyDown = false;
        bool anyHeld = false;
        bool wasAnyHeld = false;
        bool isAnyHeld = false;
        bool wasDragging = false;
        bool anyPressed = false;
        bool anyReleased = false;

        float dragStartX;
        float dragStartY;
        float dragDeltaX = 0;
        float dragDeltaY = 0;
        //Mainly used to tell if we started dragging or not, and 
        //not meant to be an accurate representation of total distance dragged

        public float WheelNotches {
            get {
                return wheelNotches;
            }
        }

        internal bool[] ButtonStates {
            get {
                return mouseButtonStates;
            }
        }
        internal bool[] PrevButtonStates {
            get {
                return prevMouseButtonStates;
            }
        }

        public bool IsAnyDown {
            get {
                return anyHeld;
            }
        }
        public bool IsAnyPressed {
            get {
                return anyPressed;
            }
        }
        public bool IsAnyReleased {
            get {
                return anyReleased;
            }
        }

        // TODO low priority: Think of a better name for this private field (HAHAHA)
        public bool CurrentlyDragging {
            get {
                return isAnyHeld && !dragCancelled && ((MathF.Abs(DragDeltaX) + MathF.Abs(DragDeltaX)) > 1);
            }
        }

        public bool StartedDragging => !wasAnyHeld && isAnyHeld;
        public bool IsDragging => wasAnyHeld && CurrentlyDragging;
        public bool FinishedDragging => wasAnyHeld && !isAnyHeld;

        public float X {
            get {
                return window.MouseState.Position.X;
            }
        }
        public float Y {
            get {
                return window.Height - window.MouseState.Position.Y;
            }
        }

        public float XDelta {
            get {
                return window.MouseState.Delta.X;
            }
        }
        public float YDelta {
            get {
                return -window.MouseState.Delta.Y;
            }
        }

        public float DragStartX {
            get {
                return dragStartX;
            }
        }
        public float DragStartY {
            get {
                return dragStartY;
            }
        }
        public float DragDeltaX {
            get {
                return dragDeltaX;
            }
        }
        internal float DragDeltaY {
            get {
                return dragDeltaY;
            }
        }

        internal MouseInputManager() {
        }

        private void OnWindowMouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj) {
            incomingWheelNotches += obj.OffsetY;
        }

        public bool IsOver(Rect rect) {
            return Intersections.IsInsideRect(X, Y, rect);
        }

        internal void Hook(OpenTKWindowWrapper window) {
            this.window = window;
            window.MouseWheel += OnWindowMouseWheel;
        }

        internal void Unhook() {
            if (window != null)
                window.MouseWheel -= OnWindowMouseWheel;
        }


        private void SwapInputBuffers() {
            bool[] temp = prevMouseButtonStates;
            prevMouseButtonStates = mouseButtonStates;
            mouseButtonStates = temp;
        }

        public bool ButtonPressed(MouseButton b) {
            if (b == MouseButton.Any)
                return anyPressed;

            return (!prevMouseButtonStates[(int)b]) && mouseButtonStates[(int)b];
        }

        public bool ButtonReleased(MouseButton b) {
            if (b == MouseButton.Any)
                return anyReleased;

            return prevMouseButtonStates[(int)b] && (!mouseButtonStates[(int)b]);
        }

        public bool ButtonHeld(MouseButton b) {
            if (b == MouseButton.Any)
                return anyHeld;

            return mouseButtonStates[(int)b];
        }

        public void CancelDrag() {
            dragCancelled = true;
            SetDragDeltas(dragStartX, dragStartY);
        }

        private void SetDragDeltas(float x, float y) {
            dragStartX = x;
            dragStartY = y;
            dragDeltaX = 0;
            dragDeltaY = 0;
        }

        private void CalculateDragDeltas(float x, float y) {
            dragDeltaY = y - dragStartY;
            dragDeltaX = x - dragStartX;
        }

        internal void Update() {
            SwapInputBuffers();

            UpdateMousewheelNotches();

            UpdatePressedStates();

            UpdateDragDeltas();
        }

        private void UpdateDragDeltas() {
            if (!isAnyHeld) {
                dragCancelled = false;
            }

            if (StartedDragging) {
                SetDragDeltas(X, Y);
            } else if (wasAnyHeld && isAnyHeld && !dragCancelled) {
                CalculateDragDeltas(X, Y);
            }
        }

        private void UpdatePressedStates() {
            wasDragging = CurrentlyDragging;
            wasAnyHeld = isAnyHeld;
            wasAnyDown = anyHeld;

            anyHeld = false;
            anyPressed = false;
            anyReleased = false;

            for (int i = 0; i < mouseButtonStates.Length; i++) {
                mouseButtonStates[i] = window.MouseState.IsButtonDown(
                        (OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)i
                    );

                anyHeld = anyHeld || mouseButtonStates[i];

                anyPressed = anyPressed || (!prevMouseButtonStates[i] && mouseButtonStates[i]);
                anyReleased = anyReleased || (prevMouseButtonStates[i] && !mouseButtonStates[i]);
            }

            isAnyHeld = wasAnyDown && anyHeld;
        }

        private void UpdateMousewheelNotches() {
            wheelNotches = incomingWheelNotches;
            incomingWheelNotches = 0;
        }
    }
}

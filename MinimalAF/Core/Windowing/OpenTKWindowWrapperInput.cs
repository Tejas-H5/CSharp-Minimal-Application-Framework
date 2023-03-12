using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace MinimalAF {
    public partial class OpenTKWindowWrapper {
        #region Keyboard input

        public const string KEYBOARD_CHARS = "\t\b\n `1234567890-=qwertyuiop[]asdfghjkl;'\\zxcvbnm,./";

        bool wasAnyHeld;
        bool isAnyHeld;

        public bool KeyJustPressed(KeyCode key) {
            return (!KeyWasDown(key)) && (KeyIsDown(key));
        }

        public bool KeyJustReleased(KeyCode key) {
            return KeyWasDown(key) && (!KeyIsDown(key));
        }

        public bool KeyWasDown(KeyCode key) {
            if (key == KeyCode.Control) {
                return KeyWasDown(KeyCode.LeftControl) || KeyWasDown(KeyCode.RightControl);
            }
            if (key == KeyCode.Shift) {
                return KeyWasDown(KeyCode.LeftShift) || KeyWasDown(KeyCode.RightShift);
            }
            if (key == KeyCode.Alt) {
                return KeyWasDown(KeyCode.LeftAlt) || KeyWasDown(KeyCode.RightAlt);
            }
            if (key == KeyCode.Any) {
                return wasAnyHeld;
            }

            return window.KeyboardState.WasKeyDown((Keys)key);
        }

        public bool KeyIsDown(KeyCode key) {
            if (key == KeyCode.Control) {
                return KeyIsDown(KeyCode.LeftControl) || KeyIsDown(KeyCode.RightControl);
            }
            if (key == KeyCode.Shift) {
                return KeyIsDown(KeyCode.LeftShift) || KeyIsDown(KeyCode.RightShift);
            }
            if (key == KeyCode.Alt) {
                return KeyIsDown(KeyCode.LeftAlt) || KeyIsDown(KeyCode.RightAlt);
            }
            if (key == KeyCode.Any) {
                return window.KeyboardState.IsAnyKeyDown;
            }

            return window.KeyboardState.IsKeyDown((Keys)key);
        }

        private void UpdateKeyInput() {
            wasAnyHeld = isAnyHeld;
            isAnyHeld = window.KeyboardState.IsAnyKeyDown;
        }

        #endregion
        #region Mouse Input


        float incomingMousewheelNotches = 0;
        float mouseWheelNotches = 0;

        bool[] prevMouseButtonStates = new bool[3];
        bool[] mouseButtonStates = new bool[3];

        bool mouseDragCancelled;
        bool mouseWasAnyDown = false;
        bool mouseAnyDown = false;
        bool mouseWasAnyHeld = false;
        bool mouseIsAnyHeld = false;
        bool mouseWasDragging = false;
        bool mouseAnyPressed = false;
        bool mouseAnyReleased = false;

        float mouseDragStartX;
        float mouseDragStartY;
        float mouseDragDeltaX = 0;
        float mouseDragDeltaY = 0;
        //Mainly used to tell if we started dragging or not, and 
        //not meant to be an accurate representation of total distance dragged

        public float MouseWheelNotches => mouseWheelNotches;

        internal bool[] MouseButtonStates => mouseButtonStates;

        internal bool[] MousePrevButtonStates => prevMouseButtonStates;

        // TOOD [priority=low] check that the many many flags relating to dragging are actually useful.
        // I know that I copied a lot of this code from things I have already made in Processing, but 
        // I wonder if I would still be using this kind of stuff ...

        // TODO: better names here
        public bool MouseIsAnyDown => mouseAnyDown;
        public bool MouseIsAnyPressed => mouseAnyPressed;
        public bool MouseIsAnyReleased => mouseAnyReleased;
        public bool MouseCurrentlyDragging => mouseIsAnyHeld && !mouseDragCancelled && ((MathF.Abs(MouseDragDeltaX) + MathF.Abs(MouseDragDeltaX)) > 1);

        public bool MouseStartedDragging => !wasAnyHeld && mouseIsAnyHeld;
        public bool MouseIsDragging => wasAnyHeld && MouseCurrentlyDragging;
        public bool MouseWasDragging => mouseWasDragging;
        public bool MouseFinishedDragging => wasAnyHeld && !mouseIsAnyHeld;

        public float MouseX => window.MouseState.Position.X;
        public float MouseY => Height - window.MouseState.Position.Y;

        public float MouseXDelta => window.MouseState.Delta.X;
        public float MouseYDelta => -window.MouseState.Delta.Y;

        public float MouseDragStartX => mouseDragStartX;

        public float MouseDragStartY => mouseDragStartY;
        public float MouseDragDeltaX => mouseDragDeltaX;
        public float MouseDragDeltaY => mouseDragDeltaY;

        // Currently needs to be manually invoked by the OpenTKWindowWrapper or whatever
        void OnWindowMouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj) {
            incomingMousewheelNotches += obj.OffsetY;
        }

        public bool MouseIsOver(Rect rect) {
            float x = MouseX, y = MouseY,
                left = rect.X0, right = rect.X1,
                bottom = rect.Y0, top = rect.Y1;

            if (x > left && x < right) {
                if (y < top && y > bottom) {
                    return true;
                }
            }

            return false;
        }

        public bool MouseButtonPressed(MouseButton b) {
            if (b == MouseButton.Any)
                return mouseAnyPressed;

            return (!prevMouseButtonStates[(int)b]) && mouseButtonStates[(int)b];
        }

        public bool MouseButtonReleased(MouseButton b) {
            if (b == MouseButton.Any)
                return mouseAnyReleased;

            return prevMouseButtonStates[(int)b] && (!mouseButtonStates[(int)b]);
        }

        public bool MouseButtonHeld(MouseButton b) {
            if (b == MouseButton.Any)
                return mouseAnyDown;

            return mouseButtonStates[(int)b];
        }

        public void MouseCancelDrag() {
            mouseDragCancelled = true;

            mouseDragDeltaX = 0;
            mouseDragDeltaY = 0;
        }

        private void MouseUpdateInput() {
            // Swap the mouse input buffers
            {
                bool[] temp = prevMouseButtonStates;
                prevMouseButtonStates = mouseButtonStates;
                mouseButtonStates = temp;
            }

            // Update mouse wheel notches
            {
                mouseWheelNotches = incomingMousewheelNotches;
                incomingMousewheelNotches = 0;
            }

            // Update mouse pressed states
            {
                mouseWasDragging = MouseCurrentlyDragging;
                mouseWasAnyHeld = mouseIsAnyHeld;
                mouseWasAnyDown = mouseAnyDown;

                mouseAnyDown = false;
                mouseAnyPressed = false;
                mouseAnyReleased = false;

                for (int i = 0; i < mouseButtonStates.Length; i++) {
                    mouseButtonStates[i] = window.MouseState.IsButtonDown(
                            (OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)i
                        );

                    mouseAnyDown = mouseAnyDown || mouseButtonStates[i];

                    mouseAnyPressed = mouseAnyPressed || (!prevMouseButtonStates[i] && mouseButtonStates[i]);
                    mouseAnyReleased = mouseAnyReleased || (prevMouseButtonStates[i] && !mouseButtonStates[i]);
                }

                mouseIsAnyHeld = mouseWasAnyDown && mouseAnyDown;
            }


            // Update mouse drag deltas and other drag related stuff
            {
                mouseWasDragging = MouseIsDragging;

                if (!mouseIsAnyHeld) {
                    mouseDragCancelled = false;
                }

                if (MouseStartedDragging) {
                    // initialize mouse drag state
                    mouseDragStartX = MouseX;
                    mouseDragStartY = MouseY;
                    mouseDragDeltaX = 0;
                    mouseDragDeltaY = 0;
                } else if (mouseWasAnyHeld && mouseIsAnyHeld && !mouseDragCancelled) {
                    // recalculate deltas
                    mouseDragDeltaX = MouseX - mouseDragStartX;
                    mouseDragDeltaY = MouseY - mouseDragStartY;
                }
            }
        }

        #endregion
    }
}
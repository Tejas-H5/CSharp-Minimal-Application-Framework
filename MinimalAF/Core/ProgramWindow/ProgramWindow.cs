using MinimalAF.Audio;
using MinimalAF.Rendering;
using MinimalAF.ResourceManagement;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace MinimalAF {
    public enum WindowState {
        Normal,
        Minimized,
        Maximized,
        Fullscreen
    }

    internal class OpenTKNativeWindowWrapper : NativeWindow {
        public OpenTKNativeWindowWrapper(NativeWindowSettings settings) : base(settings) {
        }

        public Action Refresh;
        // public Action<CancelEventArgs> Closing;

        // protected override void OnClosing(CancelEventArgs e) {
        // Closing(e);
        // }

        // Some OpenTK comment somewhere says this is ran when resizing the window  - 
        // Ideally, we would like to rerender then as well.
        protected override void OnRefresh() {
            Refresh();
        }
    }


    public sealed class ProgramWindow {
        IRenderable _renderable;
        Func<FrameworkContext, IRenderable> _renderableConstructor;

        private double _frameDuration;

        // We use this free some OpenGL resources every X frames. I forget why this was needed, but it was
        // (SMH 2 years ago me for not writing a comment)
        private int _deletionFrameCounter = 0;  

        OpenTKNativeWindowWrapper _window;
        // GameWindow _w;

        /// <summary>
        /// There is no update loop. It is dead, and we killed it
        /// </summary>
        public double RenderFrequency {
            get {
                return 1.0 / _frameDuration;
            }
            set {
                _frameDuration = 1.0 / value;
            }
        }

        public Vector2i Size {
            get => _window.Size;
            set => _window.Size = value;
        }

        public string Title {
            get => _window.Title;
            set => _window.Title = value;
        }

        public double FrameDuration => _frameDuration;

        public ProgramWindow(Func<FrameworkContext, IRenderable> renderableConstructor) {
            this._renderableConstructor = renderableConstructor;

            _window = new OpenTKNativeWindowWrapper(new NativeWindowSettings { StartVisible = false });

            _window.IsVisible = false;
            _window.MouseWheel += OnWindowMouseWheel;
            _window.KeyDown += OnKeyDown;
            _window.TextInput += OnTextInput;
            _window.Refresh = OnRefresh;
        }

        public double GetSecondsSinceStart() {
            return GLFW.GetTime();
        }

        public void SetSecondsSinceStart(double t) {
            GLFW.SetTime(t);
        }

        double dt, prevFrameEnd;
        public unsafe void Run() {
            OnLoad();

            dt = 0.0; 
            prevFrameEnd = GetSecondsSinceStart();
            while (!GLFW.WindowShouldClose(_window.WindowPtr)) {
                ProcessUpdateEvents();

                StepRenderLoop();
            }

            OnUnload();
        }

        void StepRenderLoop() {
            RenderFrame(dt);

            double frameEnd = GetSecondsSinceStart();
            dt = frameEnd - prevFrameEnd;

            // This is a power saving mechanism that will sleep the thread if we
            // have the time available to do so. It reduces overall CPU consumption.
            // It also never kicks in if _frameDuration = 0
            {
                double wantedNextFrameEnd = prevFrameEnd + _frameDuration;
                double remainingTimeTillWantedNextFrameEnd = wantedNextFrameEnd - frameEnd;
                int timeTakenToDoThisStuffMS = 1;
                int remainingTimeTillWantedNextFrameEndMS = (int)Math.Floor(
                    (remainingTimeTillWantedNextFrameEnd) * 1000
                ) - timeTakenToDoThisStuffMS;

                if (remainingTimeTillWantedNextFrameEndMS > 0) {
                    Thread.Sleep(remainingTimeTillWantedNextFrameEndMS);
                    frameEnd = GetSecondsSinceStart();
                    dt = frameEnd - prevFrameEnd;
                }
            }

            prevFrameEnd = frameEnd;
        }

        void OnRefresh() {
            StepRenderLoop();
        }

        public void Close() {
            _window.Close();
        }
       
        void OnLoad() {
            _window.Context?.MakeCurrent();

            CTX.Init(_window.Context);
            AudioCTX.Init();

            _window.IsVisible = true;

            // Usually an app will be donig OpenGL/OpenAL stuff that will
            // require those things to be initialized, so we are constructing
            // the app with a constructor rather than injecting it directly
            _renderable = _renderableConstructor(CreateFrameworkContext());
        }

        void OnUnload() {
            // clean up all our resources

            CTX.Dispose(true);
            AudioCTX.Cleanup();

            GLDeletionQueue.DeleteResources();
        }

        /// <summary>
        ///  Does things like polling inputs and updating our input/audio subsystems.
        ///  previous input states.
        /// </summary>
        private void ProcessUpdateEvents() {
            // GLFW and OpenTK Update
            {
                // important that these two are in this order:

                // propagate all current event states to the previous event state
                _window.ProcessInputEvents();
                UpdateKeyInputsBeforePoll();

                // checks all events, then populates current even state through GLFW callbacks.
                GLFW.PollEvents();  // is this thread-static?
            }

            // Update our subsystems. They all assume that previous input states have been propagated
            // and the window's input contain the current input states
            {
                UpdateKeyInput();
                MouseUpdateInput();
                AudioCTX.Update();

                _deletionFrameCounter++;
                // 100 is an arbitrary number.
                if (_deletionFrameCounter > 100) {
                    _deletionFrameCounter = 0;

                    GLDeletionQueue.DeleteResources();
                }
            }
        }

        void RenderFrame(double deltaTime) {
            Time.deltaTime = (float)deltaTime;

            // render
            {
                var ctx = CreateFrameworkContext().Use();

                CTX.SetViewport(new Rect(0, 0, Width, Height));
                CTX.Clear();

                CTX.ContextWidth = Width;
                CTX.ScreenWidth = Width;
                CTX.ContextHeight = Height;
                CTX.ScreenHeight = Height;

                _renderable.Render(ctx);

                CTX.SwapBuffers();
            }
        }

        public int Width => _window.Size.X;
        public int Height => _window.Size.Y;

        private FrameworkContext CreateFrameworkContext() {
            return new FrameworkContext(
                new Rect(0, 0, Width, Height),
                this,
                isClipping: false
            );
        }

        public void SetWindowState(WindowState state) {
            _window.WindowState = (OpenTK.Windowing.Common.WindowState)state;
        }

        #region Keyboard input

        internal const string KEYBOARD_CHARS = "\t\b\n `1234567890-=qwertyuiop[]asdfghjkl;'\\zxcvbnm,./";

        List<KeyCode> _keysJustPressedOrRepeated = new List<KeyCode>();
        List<int> _charsJustInputted = new List<int>();

        bool wasAnyHeld;
        bool isAnyHeld;

        internal List<KeyCode> KeysJustPressedOrRepeated => _keysJustPressedOrRepeated;
        internal List<int> CharsJustInputted => _charsJustInputted;

        // this thing fires on repeats as well, so it is useful for capturing inputs for
        // text inputs and such but not necessarily for games
        private void OnKeyDown(KeyboardKeyEventArgs args) {
            _keysJustPressedOrRepeated.Add((KeyCode)args.Key);
        }

        private void OnTextInput(TextInputEventArgs obj) {
            _charsJustInputted.Add(obj.Unicode);
        }


        internal bool KeyJustPressed(KeyCode key) {
            return (!KeyWasDown(key)) && (KeyIsDown(key));
        }

        internal bool KeyJustReleased(KeyCode key) {
            return KeyWasDown(key) && (!KeyIsDown(key));
        }

        internal bool KeyWasDown(KeyCode key) {
            if (key == KeyCode.Ctrl) {
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

            return _window.KeyboardState.WasKeyDown((Keys)key);
        }

        // copy-pasted from OpenTK codebase and changed IsKeyDown to Waskeydown for debugging
        //public string ToStringWas(KeyboardState state) {
        //    StringBuilder stringBuilder = new StringBuilder();
        //    stringBuilder.Append('{');
        //    bool flag = true;
        //    for (Keys keys = (Keys)0; keys <= Keys.Menu; keys++) {
        //        if (state.WasKeyDown(keys)) {
        //            stringBuilder.AppendFormat("{0}{1}", keys, (!flag) ? ", " : string.Empty);
        //            flag = false;
        //        }
        //    }

        //    stringBuilder.Append('}');
        //    return stringBuilder.ToString();
        //}

        internal bool KeyIsDown(KeyCode key) {
            if (key == KeyCode.Ctrl) {
                return KeyIsDown(KeyCode.LeftControl) || KeyIsDown(KeyCode.RightControl);
            }
            if (key == KeyCode.Shift) {
                return KeyIsDown(KeyCode.LeftShift) || KeyIsDown(KeyCode.RightShift);
            }
            if (key == KeyCode.Alt) {
                return KeyIsDown(KeyCode.LeftAlt) || KeyIsDown(KeyCode.RightAlt);
            }
            if (key == KeyCode.Any) {
                return _window.KeyboardState.IsAnyKeyDown;
            }

            return _window.KeyboardState.IsKeyDown((Keys)key);
        }

        private void UpdateKeyInputsBeforePoll() {
            _keysJustPressedOrRepeated.Clear();
            _charsJustInputted.Clear();
        }

        private void UpdateKeyInput() {
            wasAnyHeld = isAnyHeld;
            isAnyHeld = _window.KeyboardState.IsAnyKeyDown;
        }

        #endregion
        #region Mouse Input


        float incomingMousewheelNotches = 0;
        float mouseWheelNotches = 0;

        bool[] prevMouseButtonStates = new bool[3];
        bool[] mouseButtonStates = new bool[3];

        bool mouseWasAnyDown = false;
        bool mouseAnyDown = false;
        //Mainly used to tell if we started dragging or not, and 
        //not meant to be an accurate representation of total distance dragged

        internal float MouseWheelNotches => mouseWheelNotches;

        internal bool[] MouseButtonStates => mouseButtonStates;

        internal bool[] MousePrevButtonStates => prevMouseButtonStates;

        // TOOD [priority=low] check that the many many flags relating to dragging are actually useful.
        // I know that I copied a lot of this code from things I have already made in Processing, but 
        // I wonder if I would still be using this kind of stuff ...

        // TODO: better names here
        internal bool MouseIsAnyDown => mouseAnyDown;
        internal bool MouseIsAnyJustPressed => mouseAnyDown && !mouseWasAnyDown;
        internal bool MouseIsAnyJustReleased => !mouseAnyDown && mouseWasAnyDown;
        internal float MouseX => _window.MouseState.Position.X;
        internal float MouseY => Height - _window.MouseState.Position.Y;

        internal float MouseXDelta => _window.MouseState.Delta.X;
        internal float MouseYDelta => -_window.MouseState.Delta.Y;

        // Currently needs to be manually invoked by the OpenTKWindowWrapper or whatever
        void OnWindowMouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj) {
            incomingMousewheelNotches += obj.OffsetY;
        }

        internal bool MouseIsOver(Rect rect) {
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

        internal bool MouseButtonJustPressed(MouseButton b) {
            return !MouseButtonWasDown(b) && MouseButtonIsDown(b);
        }

        internal bool MouseButtonJustReleased(MouseButton b) {
            return MouseButtonWasDown(b) && !MouseButtonIsDown(b);
        }

        internal bool MouseButtonIsDown(MouseButton b) {
            if (b == MouseButton.Any)
                return mouseAnyDown;

            return mouseButtonStates[(int)b];
        }

        internal bool MouseButtonWasDown(MouseButton b) {
            if (b == MouseButton.Any)
                return mouseWasAnyDown;

            return prevMouseButtonStates[(int)b];
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
                mouseWasAnyDown = mouseAnyDown;

                mouseAnyDown = false;

                for (int i = 0; i < mouseButtonStates.Length; i++) {
                    mouseButtonStates[i] = _window.MouseState.IsButtonDown(
                            (OpenTK.Windowing.GraphicsLibraryFramework.MouseButton)i
                        );

                    mouseAnyDown = mouseAnyDown || mouseButtonStates[i];
                }
            }
        }

        #endregion
    }
}

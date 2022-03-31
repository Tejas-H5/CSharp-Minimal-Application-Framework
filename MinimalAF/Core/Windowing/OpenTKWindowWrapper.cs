using MinimalAF.Audio;
using MinimalAF.Rendering;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.ComponentModel;

namespace MinimalAF {
    internal class OpenTKWindowWrapper : GameWindow {
        Element _rootElement;

        double time = 0;
        int renderFrames = 0;
        int updateFrames = 0;
        float _fps;
        float _updateFps;

        public int Height {
            get {
                return Size.Y;
            }
        }
        public int Width {
            get {
                return Size.X;
            }
        }
        public Rect Rect {
            get {
                return new Rect(0, 0, Width, Height);
            }
        }
        public float CurrentFPS {
            get {
                return _fps;
            }
        }

        public float CurrentUpdateFPS {
            get {
                return _updateFps;
            }
        }

        public OpenTKWindowWrapper(Element rootElement)
            : base(new GameWindowSettings {
                IsMultiThreaded = false
            },
            new NativeWindowSettings {
                StartVisible = false
            }) {
            _rootElement = rootElement;
        }

        public event Action<uint> TextInputEvent;

        public event Action<float> MouseWheelVertical;

        bool _init = false;

        protected unsafe override void OnLoad() {
            base.OnLoad();

            KeyDown += ProcessPhysicalKeyPress;
            TextInput += ProcessCharTextInputs;
            MouseWheel += WindowInstance_MouseWheel;

            CTX.Init(Context);
            AudioCTX.Init();
            Input.HookToWindow(this);
            _rootElement.Mount();
            _init = true;

            ResizeAction();

            IsVisible = true;
        }

        private void WindowInstance_MouseWheel(MouseWheelEventArgs obj) {
            MouseWheelVertical?.Invoke(obj.OffsetY);
        }

        private void ProcessCharTextInputs(TextInputEventArgs obj) {
            Console.WriteLine("char input");
            for (int i = 0; i < obj.AsString.Length; i++) {
                TextInputEvent?.Invoke(obj.AsString[i]);
            }
        }

        private void ProcessPhysicalKeyPress(KeyboardKeyEventArgs obj) {
            KeyCode keyCode = (KeyCode)obj.Key;

            if ((keyCode == KeyCode.Backspace)
                || (keyCode == KeyCode.Enter)
                || (keyCode == KeyCode.NumpadEnter)
                || (keyCode == KeyCode.Tab)) {
                Console.WriteLine("non-char input");
                TextInputEvent?.Invoke(CharKeyMapping.KeyCodeToChar(keyCode));
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);

            Input.Update();
            AudioCTX.Update();

            Time._deltaTime = (float)args.Time;
            _rootElement.Update();

            TrackUpdateFPS(args);
        }

        private void TrackUpdateFPS(FrameEventArgs args) {
            TrackFPS(args);

            updateFrames++;
        }

        private void TrackFPS(FrameEventArgs args) {
            time += args.Time;

            if (time >= 1) {
                _fps = renderFrames / (float)time;
                _updateFps = updateFrames / (float)time;

                Console.WriteLine($"Render FPS: {_fps}, Update FPS: {updateFrames / time}");

                time = 0;
                renderFrames = 0;
                updateFrames = 0;
            }
        }


        protected override void OnRenderFrame(FrameEventArgs args) {
            Time._deltaTime = (float)args.Time;
            CTX.ContextWidth = Width;
            CTX.ContextHeight = Height;

            base.OnRenderFrame(args);

            CTX.SetViewport(Rect);
            CTX.Clear();

            CTX.Cartesian2D(Width, Height);

            _rootElement.Render();


            CTX.SwapBuffers();

            renderFrames++;
        }

        void ResizeAction() {
            _rootElement.UpdateLayout();

            CTX.SetViewport(Rect);
        }


        protected override void OnResize(ResizeEventArgs e) {
            if (!_init)
                return;

            base.OnResize(e);

            ResizeAction();
        }

        protected override void OnMaximized(MaximizedEventArgs e) {
            ResizeAction();
        }

        //TODO: Find out why OnUnload() wasn't working
        protected unsafe override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            Cleanup();

            e.Cancel = false;
        }

        private unsafe void Cleanup() {
            _rootElement.Dismount();

            CTX.Dispose(true);
            AudioCTX.Cleanup();
        }

        public void Maximize() {
            WindowState = WindowState.Maximized;
        }

        public void Fullscreen() {
            WindowState = WindowState.Fullscreen;
        }

        public void Unmaximize() {
            WindowState = WindowState.Normal;
        }
    }
}

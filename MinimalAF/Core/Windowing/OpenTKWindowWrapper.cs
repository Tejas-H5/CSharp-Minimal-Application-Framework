using MinimalAF.Audio;
using MinimalAF.Rendering;
using MinimalAF.ResourceManagement;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.ComponentModel;

namespace MinimalAF {
    public enum WindowState {
        Normal,
        Minimized,
        Maximized,
        Fullscreen
    }

    internal class OpenTKWindowWrapper : GameWindow {
        ApplicationWindow rootWindow;

        double time = 0;
        int renderFrames = 0;
        int updateFrames = 0;
        int deletionInterval = 0;
        float fps;
        float updateFps;

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
                return fps;
            }
        }

        public float CurrentUpdateFPS {
            get {
                return updateFps;
            }
        }

        public OpenTKWindowWrapper(ApplicationWindow rootWindow)
            : base(new GameWindowSettings {},
            new NativeWindowSettings {
                StartVisible = false
            }) {
            this.rootWindow = rootWindow;
        }

        public event Action<uint> TextInputEvent;

        public event Action<float> MouseWheelVertical;

        bool init = false;

        protected unsafe override void OnLoad() {
            base.OnLoad();

            KeyDown += ProcessPhysicalKeyPress;
            TextInput += ProcessCharTextInputs;
            MouseWheel += WindowInstance_MouseWheel;

            CTX.Init(Context);
            AudioCTX.Init();
            Input.HookToWindow(this);
            init = true;

            ResizeAction();

            rootWindow.StartMounting();

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

            Time.deltaTime = (float)args.Time;
            rootWindow.UpdateSelfAndChildren(new Rect(0, 0, Width, Height));

            TrackUpdateFPS(args);


            deletionInterval++;
            // 100 is an arbitrary number.
            if (deletionInterval > 100) {
                deletionInterval = 0;

                GLDeletionQueue.DeleteResources();
            }
        }

        private void TrackUpdateFPS(FrameEventArgs args) {
            TrackFPS(args);

            updateFrames++;
        }

        private void TrackFPS(FrameEventArgs args) {
            time += args.Time;

            if (time >= 1) {
                fps = renderFrames / (float)time;
                updateFps = updateFrames / (float)time;

                Console.WriteLine("Render FPS: " + fps + ", Update FPS: " + updateFrames / time);

                time = 0;
                renderFrames = 0;
                updateFrames = 0;
            }
        }


        protected override void OnRenderFrame(FrameEventArgs args) {
            Time.deltaTime = (float)args.Time;

            base.OnRenderFrame(args);

            CTX.SetViewport(Rect);
            CTX.Clear();

            CTX.Cartesian2D(1, 1);

            rootWindow.RenderSelfAndChildren(new Rect(0, 0, Width, Height));


            CTX.SwapBuffers();

            renderFrames++;
        }

        void ResizeAction() {
            rootWindow.RelativeRect = new Rect(0, 0, Width, Height);
            CTX.SetScreenWidth(Width, Height);

            rootWindow.Layout();
        }


        protected override void OnResize(ResizeEventArgs e) {
            if (!init)
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
            rootWindow.Dismount();

            CTX.Dispose(true);
            AudioCTX.Cleanup();

            GLDeletionQueue.DeleteResources();
        }

        public void SetWindowState(WindowState state) {
            WindowState = (OpenTK.Windowing.Common.WindowState)state;
        }
    }
}

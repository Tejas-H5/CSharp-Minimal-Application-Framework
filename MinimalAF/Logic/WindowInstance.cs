using MinimalAF.Audio;
using MinimalAF.Datatypes;
using MinimalAF.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.ComponentModel;

namespace MinimalAF.Logic
{
    internal class WindowInstance : GameWindow
    {
        EntryPoint _program;

        double time = 0;
        int renderFrames = 0;
        int updateFrames = 0;
        float _fps;

        public float Height { get { return Size.Y; } }
        public float Width { get { return Size.X; } }
        public Rect2D Rect { get { return new Rect2D(0, 0, Width, Height); } }
        public float CurrentFPS { get { return _fps; } }

        public WindowInstance(EntryPoint program)
            : base(new GameWindowSettings
            {
                IsMultiThreaded = false
            },
            new NativeWindowSettings
            {
                StartVisible = false
            })
        {
            _program = program;
        }

        public event Action<uint> TextInputEvent;

        public event Action<float> MouseWheelVertical;

        bool _init = false;

        protected unsafe override void OnLoad()
        {
            base.OnLoad();

            KeyDown += ProcessPhysicalKeyPress;
            TextInput += ProcessCharTextInputs;
            MouseWheel += WindowInstance_MouseWheel;

            CTX.Init(Context);
            AudioCTX.Init();

            Input.HookToWindow(this);

            _program.Start();

            _init = true;
            _program.Resize();

            IsVisible = true;
        }

        private void WindowInstance_MouseWheel(MouseWheelEventArgs obj)
        {
            MouseWheelVertical?.Invoke(obj.OffsetY);
        }

        private void ProcessCharTextInputs(TextInputEventArgs obj)
        {
            Console.WriteLine("char input");
            for (int i = 0; i < obj.AsString.Length; i++)
            {
                TextInputEvent?.Invoke(obj.AsString[i]);
            }
        }

        private void ProcessPhysicalKeyPress(KeyboardKeyEventArgs obj)
        {
            KeyCode keyCode = (KeyCode)obj.Key;

            if ((keyCode == KeyCode.Backspace)
                || (keyCode == KeyCode.Enter)
                || (keyCode == KeyCode.NumpadEnter)
                || (keyCode == KeyCode.Tab))
            {
                Console.WriteLine("non-char input");
                TextInputEvent?.Invoke(CharKeyMapping.KeyCodeToChar(keyCode));
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            Input.Update();

            _program.Update(args.Time);

            TrackFPS(args);

            updateFrames++;
        }

        private void TrackFPS(FrameEventArgs args)
        {
            time += args.Time;

            if (time >= 1)
            {
                _fps = renderFrames / (float)time;

                Console.WriteLine($"Render FPS: {_fps}, Update FPS: {updateFrames / time}");

                time = 0;
                renderFrames = 0;
                updateFrames = 0;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            CTX.Clear();

            _program.Render(args.Time);

            CTX.SwapBuffers();

            renderFrames++;
        }

        void OnResize()
        {
            GL.Viewport(0, 0, Size.X, Size.Y);

            _program.Resize();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            if (!_init)
                return;

            base.OnResize(e);

            OnResize();
        }

        protected override void OnMaximized(MaximizedEventArgs e)
        {
            OnResize();
        }

        //TODO: Find out why OnUnload() wasn't working
        protected unsafe override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Cleanup();

            e.Cancel = false;
        }

        private unsafe void Cleanup()
        {
            _program.Cleanup();

            CTX.Dispose(true);
            AudioCTX.Cleanup();
        }

        public void Maximize()
        {
            WindowState = WindowState.Maximized;
        }

        public void Fullscreen()
        {
            WindowState = WindowState.Fullscreen;
        }

        public void Unmaximize()
        {
            WindowState = WindowState.Normal;
        }
    }
}

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderingEngine.Datatypes;
using RenderingEngine.Rendering;
using System;
using System.ComponentModel;

namespace RenderingEngine.Logic
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

        public unsafe WindowInstance(EntryPoint program)
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

            GLFW.SetKeyCallback(WindowPtr, ProcessPhysicalKeyPress);
            GLFW.SetCharCallback(WindowPtr, ProcessCharTextInputs);

            CTX.Init(Context);
            Input.Init(this);
        }

        private unsafe void ProcessCharTextInputs(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, uint codepoint)
        {
            UnicodeTextInputEvent?.Invoke(Convert.ToChar(codepoint));
        }

        private unsafe void ProcessPhysicalKeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, Keys key, int scanCode, InputAction action, KeyModifiers mods)
        {
            KeyCode keyCode = (KeyCode)key;

            if ((keyCode == KeyCode.Backspace)
                || (keyCode == KeyCode.Enter)
                || (keyCode == KeyCode.NumpadEnter)
                || (keyCode == KeyCode.Tab))
            {
                UnicodeTextInputEvent?.Invoke(CharKeyMapping.KeyCodeToChar((KeyCode)key));
            }
        }

        public event Action<uint> UnicodeTextInputEvent;

        bool _init = false;

        protected override void OnLoad()
        {
            base.OnLoad();
            _program.Start();

            _init = true;
            _program.Resize();

            IsVisible = true;
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

            _program.RenderLoopIteration(args.Time);

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

            _program.Cleanup();

            GLFW.SetCharCallback(WindowPtr, null);
            GLFW.SetKeyCallback(WindowPtr, null);

            e.Cancel = false;
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

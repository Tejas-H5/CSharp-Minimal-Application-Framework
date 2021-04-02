using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderingEngine.UI;
using System.ComponentModel;
using RenderingEngine.Datatypes;
using System.Drawing;
using RenderingEngine.Rendering;

namespace RenderingEngine.Logic
{
    public class GraphicsWindow : GameWindow
    {
        EntryPoint _program;
        MouseInputManager _mouseInputManager;

        public float Height { get { return Size.Y; } }
        public float Width { get { return Size.X; } }
        public Rect2D Rect { get { return new Rect2D(0, 0, Width, Height); } }
        public float MouseX { get { return MousePosition.X; } }
        public float MouseY { get { return Height - MousePosition.Y; }}

        double time = 0;
        int renderFrames = 0;
        int updateFrames = 0;
        float _fps;

        public float CurrentFPS { get { return _fps; } }

        public GraphicsWindow(EntryPoint program)
            : base(GameWindowSettings.Default, new NativeWindowSettings {
                StartVisible = false
            })
        {
            _program = program;

            RenderingContext.Init(Context);

            _mouseInputManager = new MouseInputManager(this);
        }

        protected override void OnLoad()
        {
            _program.Start(this);

            IsVisible = true;

            base.OnLoad();
        }

        public bool[] MouseButtonStates { get { return _mouseInputManager.MouseButtonStates; } }
        public bool[] PrevMouseButtonStates { get { return _mouseInputManager.PrevMouseButtonStates; } }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            TrackFPS(args);

            _mouseInputManager.Update();

            _program.Update(args.Time);

            updateFrames++;

            base.OnUpdateFrame(args);
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
            _program.Render(args.Time);

            renderFrames++;

            base.OnRenderFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);

            _program.Resize();
        }

        //TODO: Find out why OnUnload() wasn't working
        protected override void OnClosing(CancelEventArgs e)
        {
            _program.Cleanup();
            e.Cancel = false;
            base.OnClosing(e);
        }

        // -------- mouseInputManager wrappers

        public bool MouseClicked(MouseButton b)
        {
            return _mouseInputManager.MouseClicked(b);
        }

        public bool MouseReleased(MouseButton b)
        {
            return _mouseInputManager.MouseReleased(b);
        }

        public bool MouseHeld(MouseButton b)
        {
            return _mouseInputManager.MouseHeld(b);
        }

        public void StartMouseDrag(float x, float y)
        {
            _mouseInputManager.StartDrag(x, y);
        }

        public bool AnyClicked { get { return _mouseInputManager.AnyClicked; } }
        public bool IsMouseDragging { get { return _mouseInputManager.IsDragging; } }
        public bool WasMouseDragging { get { return _mouseInputManager.WasDragging; } }
        public float DragStartX { get { return _mouseInputManager.DragStartX; } }
        public float DragStartY { get { return _mouseInputManager.DragStartY; } }
        public float DragDeltaX { get { return _mouseInputManager.DragDeltaX; } }
        public float DragDeltaY { get { return _mouseInputManager.DragDeltaY; } }
        public float DragDisplacement { get { return _mouseInputManager.DragDisplacement; } }
    }
}

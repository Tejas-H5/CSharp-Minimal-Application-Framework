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

        double time = 0;
        int renderFrames = 0;
        int updateFrames = 0;
        float _fps;

        public float Height { get { return Size.Y; } }
        public float Width { get { return Size.X; } }
        public Rect2D Rect { get { return new Rect2D(0, 0, Width, Height); } }

        public float CurrentFPS { get { return _fps; } }

        public GraphicsWindow(EntryPoint program)
            : base(GameWindowSettings.Default, new NativeWindowSettings {
                StartVisible = false
            })
        {
            _program = program;

            CTX.Init(Context);
            Input.Init(this);
        }

        protected override void OnLoad()
        {
            _program.Start(this);

            IsVisible = true;

            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            TrackFPS(args);

            _program.Update(args.Time);

            Input.Update();

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
    }
}

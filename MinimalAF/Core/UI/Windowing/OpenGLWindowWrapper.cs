using MinimalAF.Audio;
using MinimalAF.Rendering;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;
using Silk.NET.GLFW;
using System.ComponentModel;

namespace MinimalAF
{
	internal class OpenGLWindowWrapper
    {
		Glfw glfw;
        Element _rootElement;
		IWindow _window;

        double time = 0;
        int renderFrames = 0;
        int updateFrames = 0;
        float _fps;
        float _updateFps;

        public int Width => _window.Size.X;
        public int Height => _window.Size.Y; 

		public Vector2D<int> Size {
			get => _window.Size;
			set => _window.Size = value;
		}

		public Rect2D Rect { get { return new Rect2D(0, 0, Width, Height); } }
        public float MeasuredRenderFPS { get { return _fps; } }

        public float MeasuredUpdateFPS { get { return _updateFps; } }

		public double UpdatesPerSecond { get => _window.UpdatesPerSecond; set => _window.UpdatesPerSecond = value; }
		public double RendersPerSecond { get => _window.FramesPerSecond; set => _window.FramesPerSecond = value; }

		public OpenGLWindowWrapper(Element rootElement)
        {
            _rootElement = rootElement;

			var options = WindowOptions.Default;
			_window = Window.Create(options);

			glfw = Glfw.GetApi();

			_window.Load += OnLoad;
			_window.Update += OnUpdateFrame;
			_window.Render += OnRenderFrame;
			_window.Resize += OnResize;
			_window.StateChanged += OnStateChanged;
			_window.Closing += OnClosing;
		}

		public event Action<uint> TextInputEvent;

        public event Action<float> MouseWheelVertical;

        bool _init = false;

        private void OnLoad()
        {
			var input = _window.CreateInput();
            Input.Hook(this, input);

            CTX.Init(_window);
            AudioCTX.Init();

            _rootElement.Start();

            _init = true;
            ResizeAction();

			

            _window.IsVisible = true;
        }

        protected void OnUpdateFrame(double deltaTime)
        {
            Input.Update();
            AudioCTX.Update();

            Time._deltaTime = (float)deltaTime;
            _rootElement.Update();

            TrackUpdateFPS(deltaTime);
        }

        private void TrackUpdateFPS(double deltaTime)
        {
            TrackFPS(deltaTime);

            updateFrames++;
        }

        private void TrackFPS(double deltaTime)
        {
            time += deltaTime;

            if (time >= 1)
            {
                _fps = renderFrames / (float)time;
                _updateFps = updateFrames / (float)time;

                Console.WriteLine($"Render FPS: {_fps}, Update FPS: {updateFrames / time}");

                time = 0;
                renderFrames = 0;
                updateFrames = 0;
            }
        }


        protected void OnRenderFrame(double deltaTime)
        {
            CTX.Clear();

            Time._deltaTime = (float)deltaTime;


			CTX.Viewport2D(Rect);
            _rootElement.Render();

			CTX.SwapBuffers();

            renderFrames++;
        }

        void ResizeAction()
        {
            _rootElement.Resize();

			CTX.Viewport2D(Rect);
        }


        protected void OnResize(Silk.NET.Maths.Vector2D<int> size)
        {
            if (!_init)
                return;

            ResizeAction();
        }

		private void OnStateChanged(WindowState state)
		{
			if (state == WindowState.Maximized)
			{
				ResizeAction();
			}
		}

		//TODO: Find out why OnUnload() wasn't working
		protected unsafe void OnClosing()
        {
            Cleanup();
        }

        private unsafe void Cleanup()
        {
            _rootElement.Cleanup();

            CTX.Dispose(true);
            AudioCTX.Cleanup();
        }

        public void Maximize()
        {
            _window.WindowState = WindowState.Maximized;
        }

        public void Fullscreen()
        {
			_window.WindowState = WindowState.Fullscreen;
        }

        public void Unmaximize()
        {
			_window.WindowState = WindowState.Normal;
        }
    }
}

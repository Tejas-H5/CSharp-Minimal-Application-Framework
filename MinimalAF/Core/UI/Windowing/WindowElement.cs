using System;
using Silk.NET.Maths;

namespace MinimalAF
{
	/// <summary>
	/// Only be getting this and modifying stuff if the element is a root-level element
	/// of your program.
	/// </summary>
	public class WindowElement : Element
    {
        OpenGLWindowWrapper _window;

		private IWindowResource[] _windowResources;
		public readonly WindowMouseInput MouseInput;
		public readonly WindowKeyboardInput KeyboardInput;

		public override bool SingleChild => true;

		public WindowElement(Element child) 
        {
            this.SetChildren(
				child
			);

			_windowResources = new IWindowResource[]
			{
				MouseInput = new WindowMouseInput(),
			};

			KeyboardInput = new WindowKeyboardInput();
		}

		public override void OnStart()
		{
			base.OnStart();
		}

		public void Run()
        {
            using (OpenGLWindowWrapper window = new OpenGLWindowWrapper(this))
            {
                _window = window;
                window.Run();
            }
            _window = null;
        }

        public Vector2D<int> Size {
            get { return new Vector2D<int>(_window.Size.X, _window.Size.Y); }
            set { _window.Size = value; }
        }

        public  void Maximize() { _window.Maximize(); }
        public  void Fullscreen() { _window.Fullscreen(); }
        public  void Unmaximize() { _window.Unmaximize(); }
        public  string Title { get { return _window.Title; } set { _window.Title = value; } }
        new public int Width { get { return _window.Width; } }
        new public int Height { get { return _window.Height; } }
        public override Rect2D GetParentRect() { return Rect; }
        new public Rect2D Rect { get { return _window.Rect; } }
        public  float CurrentFPS { get { return _window.MeasuredRenderFPS; } }
        public  float CurrentUpdateFPS { get { return _window.MeasuredUpdateFPS; } }
        public  double UpdatesPerSecond { get { return _window.UpdatesPerSecond; } set { _window.UpdatesPerSecond = value; } }
        public  double RendersPerSecond { get { return _window.RendersPerSecond; } set { _window.RendersPerSecond = value; } }
        public  string ClipboardString { get { return _window.ClipboardString; } set { _window.ClipboardString = value; } }

        internal  event Action<float> MouseWheel {
            add {
                lock (_window)
                {
                    _window.MouseWheelVertical += value;
                }
            }
            remove {
                lock (_window)
                {
                    _window.MouseWheelVertical -= value;
                }
            }
        }

		public override void OnUpdate()
		{
			foreach (IWindowResource ie in _windowResources)
			{
				ie.Update();
			}

			base.OnUpdate();
		}
	}
}

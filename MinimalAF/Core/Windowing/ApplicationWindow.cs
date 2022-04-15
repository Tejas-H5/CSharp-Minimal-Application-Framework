using MinimalAF.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;

namespace MinimalAF {
    /// <summary>
    /// Mainly used to mock a window for testing
    /// </summary>
    public abstract class Window : Element {
        public abstract string Title {
            get; set;
        }
        public abstract double UpdateFrequency {
            get; set;
        }
        public abstract double RenderFrequency {
            get; set;
        }
        public abstract WindowState WindowState {
            get; set;
        }

        public abstract (int, int) Size {
            get; set;
        }
    }

    /// <summary>
    /// Only be getting this and modifying stuff if the element is a root-level element
    /// of your program.
    /// </summary>
    public class ApplicationWindow : Window, IDisposable {
        OpenTKWindowWrapper _window;
        Element _rootElement;
        private bool disposedValue;

        internal IGraphicsContext Context => _window.Context;

        public override bool SingleChild => true;

        public ApplicationWindow() {
            _window = new OpenTKWindowWrapper(this);
        }

        public void Run(Element rootElement) {
            _rootElement = rootElement;

            // This call is actually a blocking event loop
            _window.Run();

            SetChildren(null);
            _window = null;
        }


        internal void StartMounting() {
            ctx = _window.ctx;
            SetChildren(_rootElement);
            Mounted = true;
        }

        public override (int, int) Size {
            get {
                return (_window.Size.X, _window.Size.Y);
            }
            set {
                _window.Size = new OpenTK.Mathematics.Vector2i(value.Item1, value.Item2);
            }
        }

        public void Close() {
            _window.Close();
        }

        public override WindowState WindowState {
            get => (WindowState)_window.WindowState;
            set {
                _window.WindowState = (OpenTK.Windowing.Common.WindowState)value;
            }
        }

        public override string Title {
            get {
                return _window.Title;
            }
            set {
                _window.Title = value;
            }
        }
        new public int Width {
            get {
                return _window.Width;
            }
        }
        new public int Height {
            get {
                return _window.Height;
            }
        }

        public float CurrentFPS {
            get {
                return _window.CurrentFPS;
            }
        }

        public float CurrentUpdateFPS {
            get {
                return _window.CurrentUpdateFPS;
            }
        }
        public override double UpdateFrequency {
            get {
                return _window.UpdateFrequency;
            }
            set {
                _window.UpdateFrequency = value;
            }
        }
        public override double RenderFrequency {
            get {
                return _window.RenderFrequency;
            }
            set {
                _window.RenderFrequency = value;
            }
        }
        public string ClipboardString {
            get {
                return _window.ClipboardString;
            }
            set {
                _window.ClipboardString = value;
            }
        }

        internal event Action<float> MouseWheel {
            add {
                lock (_window) {
                    _window.MouseWheelVertical += value;
                }
            }
            remove {
                lock (_window) {
                    _window.MouseWheelVertical -= value;
                }
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ApplicationWindow()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

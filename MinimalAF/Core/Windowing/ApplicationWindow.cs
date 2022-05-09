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
    public class ApplicationWindow : Window {
        OpenTKWindowWrapper window;
        Element rootElement;

        protected override bool SingleChild => true;

        public ApplicationWindow() {
        }

        public void Run(Element rootElement) {
            using (OpenTKWindowWrapper window = new OpenTKWindowWrapper(this)) {
                this.window = window;
                this.rootElement = new UIRootElement()
                    .SetChildren(rootElement);

                // event loop that polls
                window.Run();
            }

            SetChildren(null);
            window = null;
        }

        internal void StartMounting() {
            SetChildren(rootElement);
            Mounted = true;
        }

        public override (int, int) Size {
            get {
                return (window.Size.X, window.Size.Y);
            }
            set {
                window.Size = new OpenTK.Mathematics.Vector2i(value.Item1, value.Item2);
            }
        }

        public void Close() {
            window.Close();
        }

        public override WindowState WindowState {
            get => (WindowState)window.WindowState;
            set {
                window.WindowState = (OpenTK.Windowing.Common.WindowState)value;
            }
        }

        public override string Title {
            get {
                return window.Title;
            }
            set {
                window.Title = value;
            }
        }
        new public int Width {
            get {
                return window.Width;
            }
        }
        new public int Height {
            get {
                return window.Height;
            }
        }

        public float CurrentFPS {
            get {
                return window.CurrentFPS;
            }
        }

        public float CurrentUpdateFPS {
            get {
                return window.CurrentUpdateFPS;
            }
        }
        public override double UpdateFrequency {
            get {
                return window.UpdateFrequency;
            }
            set {
                window.UpdateFrequency = value;
            }
        }
        public override double RenderFrequency {
            get {
                return window.RenderFrequency;
            }
            set {
                window.RenderFrequency = value;
            }
        }
        public string ClipboardString {
            get {
                return window.ClipboardString;
            }
            set {
                window.ClipboardString = value;
            }
        }

        internal event Action<float> MouseWheel {
            add {
                lock (window) {
                    window.MouseWheelVertical += value;
                }
            }
            remove {
                lock (window) {
                    window.MouseWheelVertical -= value;
                }
            }
        }
    }
}

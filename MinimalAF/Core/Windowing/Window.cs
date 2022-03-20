﻿using OpenTK.Mathematics;
using System;

namespace MinimalAF {
    /// <summary>
    /// Only be getting this and modifying stuff if the element is a root-level element
    /// of your program.
    /// </summary>
    public class Window : Element {
        OpenTKWindowWrapper _window;

        public override bool SingleChild => true;

        public Window(Element child) {
            SetChildren(child);
        }

        public void Run() {
            using (OpenTKWindowWrapper window = new OpenTKWindowWrapper(this)) {
                _window = window;
                window.Run();
            }
            _window = null;
        }

        public Vector2i Size {
            get {
                return new Vector2i(_window.Size.X, _window.Size.Y);
            }
            set {
                _window.Size = new OpenTK.Mathematics.Vector2i(value.X, value.Y);
            }
        }

        public void Maximize() {
            _window.Maximize();
        }
        public void Fullscreen() {
            _window.Fullscreen();
        }
        public void Unmaximize() {
            _window.Unmaximize();
        }
        public string Title {
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
        internal override Rect GetParentRelativeRect() {
            return ScreenRect;
        }

        new public Rect ScreenRect {
            get {
                return _window.Rect;
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
        public double UpdateFrequency {
            get {
                return _window.UpdateFrequency;
            }
            set {
                _window.UpdateFrequency = value;
            }
        }
        public double RenderFrequency {
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
    }
}
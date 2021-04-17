using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderingEngine.Datatypes.Geometric;
using System;

namespace RenderingEngine.Logic
{
    public static class Window
    {
        static WindowInstance _instance;

        public static void RunProgram(EntryPoint program)
        {
            using (_instance = new WindowInstance(program))
            {
                _instance.Run();
            }

            _instance = null;
        }

        public static Vector2i Size { 
            get { return new Vector2i(_instance.Size.X, _instance.Size.Y); } 
            set { _instance.Size = new OpenTK.Mathematics.Vector2i(value.X, value.Y); } 
        }
        public static void Maximize() { _instance.Maximize(); }
        public static void Fullscreen() { _instance.Fullscreen(); }
        public static void Unmaximize() { _instance.Unmaximize(); }

        public static string Title { get { return _instance.Title; } set { _instance.Title = value; } }
        public static float Height { get { return _instance.Height; } }
        public static float Width { get { return _instance.Width; } }
        public static Rect2D Rect { get { return _instance.Rect; } }
        public static float CurrentFPS { get { return _instance.CurrentFPS; } }

        public static double UpdateFrequency { get { return _instance.UpdateFrequency; } set { _instance.UpdateFrequency = value; } }
        public static double RenderFrequency { get { return _instance.RenderFrequency; } set { _instance.RenderFrequency = value; } }

        public static string ClipboardString { get { return _instance.ClipboardString; } set { _instance.ClipboardString = value; } }


        public static event Action<float> MouseWheel {
            add {
                lock (_instance)
                {
                    _instance.MouseWheelVertical += value;
                }
            }
            remove {
                lock (_instance)
                {
                    _instance.MouseWheelVertical -= value;
                }
            }
        }
    }
}

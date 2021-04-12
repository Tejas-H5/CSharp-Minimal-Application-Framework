using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RenderingEngine.Datatypes;
using System;

namespace RenderingEngine.Logic
{
    public static class Window
    {
        static WindowInstance _instance;

        public static void RunProgram(EntryPoint program)
        {
            /*
            GLFW.WindowHint(WindowHintInt.GreenBits, 8);
            GLFW.WindowHint(WindowHintInt.BlueBits, 8);
            GLFW.WindowHint(WindowHintInt.AlphaBits, 8);
            GLFW.WindowHint(WindowHintInt.DepthBits, 24);
            GLFW.WindowHint(WindowHintInt.StencilBits, 8);
            //*/

            using (_instance = new WindowInstance(program))
            {
                _instance.Run();
            }

            _instance = null;
        }


        public static Vector2i Size { get { return _instance.Size; } set { _instance.Size = value; } }
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


        public static event Action<MouseWheelEventArgs> MouseWheel {
            add {
                lock (_instance)
                {
                    _instance.MouseWheel += value;
                }
            }
            remove {
                lock (_instance)
                {
                    _instance.MouseWheel -= value;
                }
            }
        }
    }
}

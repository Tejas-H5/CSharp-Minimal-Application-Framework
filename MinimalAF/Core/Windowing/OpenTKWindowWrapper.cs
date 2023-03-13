using MinimalAF.Audio;
using MinimalAF.Rendering;
using MinimalAF.ResourceManagement;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.ComponentModel;

namespace MinimalAF {
    public enum WindowState {
        Normal,
        Minimized,
        Maximized,
        Fullscreen
    }

    // TODO: rewrite with GLFW directly, no GameWindow
    public partial class ProgramWindow {
        GameWindow window;
        IRenderable renderable;
        Func<FrameworkContext, IRenderable> appConstructor;

        double time = 0;
        int renderFrames = 0;
        int updateFrames = 0;
        int deletionInterval = 0;
        float fps;
        float updateFps;
        public int Height => window.Size.Y;
        public int Width => window.Size.X;
        public Rect Rect => new Rect(0, 0, Width, Height);
        public float CurrentFPS => fps;
        public float CurrentUpdateFPS => updateFps;

        public Vector2i Size {
            get => window.Size;
            set => window.Size = value;
        }

        public string Title {
            get => window.Title;
            set => window.Title = value;
        }

        public double RenderFrequency {
            get => window.RenderFrequency;
            set => window.RenderFrequency = value;
        }
        public double UpdateFrequency {
            get => window.UpdateFrequency;
            set => window.UpdateFrequency = value;
        }

        public void Run() {
            window.Run();
        }


        public ProgramWindow(Func<FrameworkContext, IRenderable> appConstructor) {
            window = new GameWindow(
                new GameWindowSettings { },
                new NativeWindowSettings {
                    StartVisible = false
                }
            );

            window.IsVisible = false;
            window.Load += OnLoad;
            window.MouseWheel += OnWindowMouseWheel;
            window.RenderFrame += OnRenderFrame;
            window.UpdateFrame += OnUpdateFrame;
            window.Closing += OnClosing;
            //KeyDown += ProcessPhysicalKeyPress;
            //TextInput += keyboardManager.CharactersPressed;

            this.appConstructor = appConstructor;
        }


        private FrameworkContext CreateFrameworkContext() {
            return new FrameworkContext(
                new Rect(0, 0, Width, Height),
                this
            ).Use();
        }

        bool init = false;

        unsafe void OnLoad() {
            CTX.Init(window.Context);
            AudioCTX.Init();

            window.IsVisible = true;

            // Usually an app will be donig OpenGL/OpenAL stuff that will
            // require those things to be initialized, so we are constructing
            // the app with a constructor rather than injecting it directly
            renderable = appConstructor(CreateFrameworkContext());
        }

        void OnUpdateFrame(FrameEventArgs args) {
            UpdateKeyInput();
            MouseUpdateInput();
            AudioCTX.Update();

            Time.deltaTime = (float)args.Time;

            // TODO: set update/render mode flags
            //rootWindow.Render(new RenderContext {
            //    Rect = new Rect(0, 0, Width, Height)
            //});

            TrackUpdateFPS(args);

            deletionInterval++;
            // 100 is an arbitrary number.
            if (deletionInterval > 100) {
                deletionInterval = 0;

                GLDeletionQueue.DeleteResources();
            }
        }

        private void TrackUpdateFPS(FrameEventArgs args) {
            TrackFPS(args);

            updateFrames++;
        }

        private void TrackFPS(FrameEventArgs args) {
            time += args.Time;

            if (time >= 1) {
                fps = renderFrames / (float)time;
                updateFps = updateFrames / (float)time;

                Console.WriteLine("Render FPS: " + fps + ", Update FPS: " + updateFrames / time);

                time = 0;
                renderFrames = 0;
                updateFrames = 0;
            }
        }


        protected void OnRenderFrame(FrameEventArgs args) {
            Time.deltaTime = (float)args.Time;

            CTX.SetViewport(Rect);
            CTX.Clear();

            CTX.ContextWidth = Width;
            CTX.ScreenWidth = Width;
            CTX.ContextHeight = Height;
            CTX.ScreenHeight = Height;

            // rootWindow.RenderSelfAndChildren(new Rect(0, 0, Width, Height));
            renderable.Render(CreateFrameworkContext());

            CTX.SwapBuffers();

            renderFrames++;
        }

        unsafe void OnClosing(CancelEventArgs e) {
            Cleanup();
            e.Cancel = false;
        }

        private unsafe void Cleanup() {
            CTX.Dispose(true);
            AudioCTX.Cleanup();

            GLDeletionQueue.DeleteResources();
        }

        public void SetWindowState(WindowState state) {
            window.WindowState = (OpenTK.Windowing.Common.WindowState)state;
        }
    }
}

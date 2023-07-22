using System;
using System.Collections.Generic;

namespace MinimalAF.Testing {
    /// <summary>
    /// A testing harness for writing tests for your classes.
    /// Simple at the moment, but will feature more tests.
    /// 
    /// It is just an IRenderable, so you can probably nest tests, as in have a testing harness
    /// be one of the tests within another testing harness.
    /// 
    /// <code>
    /// var tests = new VisualTestRunner();
    ///
    /// tests.AddTest("Text Test", () => new TextTest());
    /// tests.AddTest("Error test", () => new ErrorTest());
    /// tests.AddTest("Arc test", () => new ArcTest());
    /// ...
    /// 
    /// new ProgramWindow((ctx) => tests.Init(ctx)).Run();
    /// 
    /// </code>
    /// </summary>
    public class VisualTestRunner : IRenderable {
        class VisualTestInfo {
            public Func<IRenderable> CreateTest;
            public string Name;
            public IRenderable Instance;
        }

        List<VisualTestInfo> tests = new List<VisualTestInfo>();
        int currentTestIndex = 0;
        bool showPropertiesPanel = false;
        IRenderable test;

        DrawableFont _font;

        public VisualTestRunner Init(AFContext ctx) {
            var w = ctx.Window;
            if (w == null) return this;

            w.Title = "Visual tests";
            //w.RenderFrequency = 60;
            w.SetWindowState(WindowState.Maximized);

            ctx.SetClearColor(Color.White);

            _font = new DrawableFont("Source Code Pro", new DrawableFontOptions { });

            return this;
        }

        public void AddTest(string name, Func<IRenderable> test) {
            tests.Add(new VisualTestInfo {
                Name = name,
                CreateTest = test
            });
        }

        public (Rect, bool) RenderTextBoxButton(ref AFContext ctx, string text, float x, float y, float padding) {
            // TODO: revert once we have finished testing text rendering
            float height = 24;
            var result = _font.MeasureText(ctx, text, height, new DrawTextOptions { });

            var rect = new Rect {
                X0 = x, X1 = x + result.Bounds.Width + 2 * padding,
                Y0 = y, Y1 = y + result.Bounds.Height + 2 * padding
            };

            var color = Color.Black;
            float alpha = 0.5f;

            bool wasClicked = false; 
            if (ctx.MouseIsOver(rect)) {
                if (ctx.MouseButtonJustReleased(MouseButton.Any)) {
                    wasClicked = true;
                }

                if (ctx.MouseButtonIsDown(MouseButton.Any)) {
                    color = Color.Black;
                    alpha = 1;
                } else {
                    color = Color.HSV(0, 0, 0.5f);
                }
            }

            ctx.SetTexture(null);
            ctx.SetDrawColor(color, alpha);
            IM.DrawRect(ctx, rect);

            ctx.SetDrawColor(Color.White);
            _font.DrawText(ctx, text, height, new DrawTextOptions {
                X = x + padding, Y = y + padding,
            });

            return (rect, wasClicked);
        }

        int frames;
        double timer;
        int fps;

        public void Render(AFContext ctx) {
            timer += Time.DeltaTime;
            if (timer > 1) {
                fps = frames;
                Console.WriteLine("FPS: " + fps);
                frames = 0;
                timer = 0;
            } else {
                frames++;
            }


            if (tests.Count == 0) {
                ctx.SetDrawColor(Color.Black);
                _font.DrawText(ctx, "No Tests", 24, new DrawTextOptions {
                    X = ctx.VW * 0.5f, Y = ctx.VH * 0.5f,
                    HAlign = 0.5f, VAlign = 0.5f, 
                });
                return;
            }

            var currentTest = tests[currentTestIndex];

            if (currentTest.Instance == null) {
                currentTest.Instance = currentTest.CreateTest();
            }

            // do the test
            {
                // its probably better if we don't catch the exceptions just yet.
                // maybe I could do some work on this later?
                // try {
                    currentTest.Instance.Render(ctx);
                //} catch (Exception err) {
                //    ctx.Use();
                //    ctx.UseFramebuffer(null);

                //    string stackTrace = "";
                //    if (err.StackTrace != null) {
                //        stackTrace = err.StackTrace;
                //    }

                //    ctx.SetDrawColor(Color.Red);
                //    _font.Draw(ctx, err.ToString(), ctx.VW * 0.5f, ctx.VH * 0.5f, HAlign.Center, VAlign.Center);
                //}
            }

            ctx.Use();

            var rect = new Rect();
            var isPressed = false;
            (rect, isPressed) = RenderTextBoxButton(ref ctx, " < ", rect.X1, 0, 10);
            if (isPressed) {
                DecrementCurrentTest(ref ctx);
            }
            (rect, isPressed) = RenderTextBoxButton(ref ctx, " > ", rect.X1, 0, 10);
            if (isPressed) {
                IncrementCurrentTest(ref ctx);
            }
            (rect, isPressed) = RenderTextBoxButton(ref ctx, currentTest.Name, rect.X1, 0, 10);

            (rect, isPressed) = RenderTextBoxButton(ref ctx, showPropertiesPanel ? "..." : " v ", rect.X1, 0, 10);
            if (isPressed) {
                TogglePropertiesPanel();
            }
        }

        void TogglePropertiesPanel() {
            showPropertiesPanel = !showPropertiesPanel;
        }

        void IncrementCurrentTest(ref AFContext ctx) {
            OnTestChange(ref ctx);

            currentTestIndex++;
            if (currentTestIndex >= tests.Count) {
                currentTestIndex = 0;
            }
        }

        void DecrementCurrentTest(ref AFContext ctx) {
            OnTestChange(ref ctx);

            currentTestIndex--;
            if (currentTestIndex < 0) {
                currentTestIndex = tests.Count - 1;
            }
        }

        private void OnTestChange(ref AFContext ctx) {
            if (tests.Count <= 1) return;

            var currentTest = tests[currentTestIndex];
            if (currentTest.Instance is IDisposable disposable) {
                disposable.Dispose();
            }

            ctx.Window.Title = "Test - " + currentTest.Name;
        }
    }
}

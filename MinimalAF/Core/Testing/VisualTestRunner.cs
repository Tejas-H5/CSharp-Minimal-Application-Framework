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

        public VisualTestRunner Init(FrameworkContext ctx) {
            var w = ctx.Window;
            if (w == null) return this;

            w.Title = "Visual tests";
            w.RenderFrequency = 60;
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

        public Rect RenderTextBoxButton(ref FrameworkContext ctx, string text, float x, float y, float padding, Action onClick = null) {
            // TODO: revert once we have finished testing text rendering
            float height = 24;
            var size = _font.MeasureText(ctx, text, new DrawTextOptions { 
                FontSize=height
            });

            var rect = new Rect {
                X0 = x, X1 = x + size.X + 2 * padding,
                Y0 = y, Y1 = y + size.Y + 2 * padding
            };

            var color = Color.Black;
            float alpha = 0.5f;
            if (onClick != null) {
                if (ctx.MouseIsOver(rect)) {
                    if (ctx.MouseButtonJustReleased(MouseButton.Any)) {
                        onClick();
                    }

                    if (ctx.MouseButtonIsDown(MouseButton.Any)) {
                        color = Color.Black;
                        alpha = 1;
                    } else {
                        color = Color.HSV(0, 0, 0.5f);
                    }
                }
            }

            ctx.SetTexture(null);
            ctx.SetDrawColor(color, alpha);
            IM.DrawRect(ctx, rect);

            ctx.SetDrawColor(Color.White);
            _font.DrawText(ctx, text, new DrawTextOptions {
                FontSize = height, X = rect.X0 + padding, Y = rect.Y0 + padding,
            });

            return rect;
        }


        public void Render(FrameworkContext ctx) {
            if (tests.Count == 0) {
                ctx.SetDrawColor(Color.Black);
                _font.DrawText(ctx, "No Tests", new DrawTextOptions {
                    FontSize = 24, 
                    X = ctx.VW * 0.5f, Y = ctx.VH * 0.5f,
                    HAlign = 0.5f, VAlign = 0.5f, 
                });
                return;
            }

            var currentTest = tests[currentTestIndex];
            ctx.Window.Title = "Test - " + currentTest.Name;

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
            rect = RenderTextBoxButton(ref ctx, " < ", rect.X1, 0, 10, DecrementCurrentTest);
            rect = RenderTextBoxButton(ref ctx, " > ", rect.X1, 0, 10, IncrementCurrentTest);
            rect = RenderTextBoxButton(ref ctx, currentTest.Name, rect.X1, 0, 10);

            if (showPropertiesPanel) {
                rect = RenderTextBoxButton(ref ctx, "...", rect.X1, 0, 10, TogglePropertiesPanel);
            } else {
                rect = RenderTextBoxButton(ref ctx, " v ", rect.X1, 0, 10, TogglePropertiesPanel);
            }
        }

        void TogglePropertiesPanel() {
            showPropertiesPanel = !showPropertiesPanel;
        }

        void IncrementCurrentTest() {
            OnTestChange();

            currentTestIndex++;
            if (currentTestIndex >= tests.Count) {
                currentTestIndex = 0;
            }
        }

        void DecrementCurrentTest() {
            OnTestChange();

            currentTestIndex--;
            if (currentTestIndex < 0) {
                currentTestIndex = tests.Count - 1;
            }
        }

        private void OnTestChange() {
            if (tests.Count <= 1) return;

            var test = tests[currentTestIndex];
            if (test.Instance is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}

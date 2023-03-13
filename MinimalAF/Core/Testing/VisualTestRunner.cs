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

        public VisualTestRunner Init(FrameworkContext ctx) {
            var w = ctx.Window;
            if (w == null) return this;

            w.Title = "Visual tests";
            w.SetWindowState(WindowState.Maximized);

            ctx.SetClearColor(Color.White);

            return this;
        }

        public void AddTest(string name, Func<IRenderable> test) {
            tests.Add(new VisualTestInfo {
                Name = name,
                CreateTest = test
            });
        }

        public Rect RenderTextBoxButton(ref FrameworkContext ctx, string text, float x, float y, float padding, Action onClick = null) {
            ctx.SetFont("Source Code Pro", 24);
            float width = ctx.GetStringWidth(text);
            float height = ctx.GetStringHeight(text);
            var rect = new Rect(x, y, x + width + 2 * padding, y + height + 2 * padding);

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

            ctx.SetDrawColor(color, alpha);
            ctx.DrawRect(rect);

            ctx.SetDrawColor(Color.White);
            ctx.DrawText(text, x + padding, y + padding, HAlign.Left, VAlign.Bottom);

            return rect;
        }


        public void Render(FrameworkContext ctx) {
            if (tests.Count == 0) {
                ctx.SetDrawColor(Color.Black);
                ctx.DrawText("No Tests", ctx.VW * 0.5f, ctx.VH * 0.5f);
                return;
            }

            var currentTest = tests[currentTestIndex];
            ctx.Window.Title = "Test - " + currentTest.Name;

            if (currentTest.Instance == null) {
                currentTest.Instance = currentTest.CreateTest();
            }

            // do the test
            {
                try {
                    currentTest.Instance.Render(ctx);
                } catch (Exception err) {
                    ctx.Use();
                    ctx.UseFramebuffer(null);

                    string stackTrace = "";
                    if (err.StackTrace != null) {
                        stackTrace = err.StackTrace;
                    }

                    ctx.SetFont("Source Code Pro", 24);
                    ctx.SetDrawColor(Color.Red);
                    ctx.DrawText(err.ToString(), ctx.VW * 0.5f, ctx.VH * 0.5f, HAlign.Center, VAlign.Center);
                }
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
            currentTestIndex++;
            if (currentTestIndex >= tests.Count) {
                currentTestIndex = 0;
            }
        }

        void DecrementCurrentTest() {
            currentTestIndex--;
            if (currentTestIndex < 0) {
                currentTestIndex = tests.Count - 1;
            }
        }
    }
}

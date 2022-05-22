using System;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    class TestError : Element {
        Exception exception;

        public TestError(Exception exception) {
            this.exception = exception;
        }

        public override void OnMount(Window w) {
            SetClearColor(Color4.VA(1, 1));
        }

        public override void OnRender() {
            SetFont("Consolas", 24);

            string text = "An exception occured:\n" +
                exception.Message;

            DrawText(text, VW(0.5f), VH(0.5f), HAlign.Center, VAlign.Center);
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Escape)) {
                GetAncestor<ApplicationWindow>().Close();
            }
        }
    }
}

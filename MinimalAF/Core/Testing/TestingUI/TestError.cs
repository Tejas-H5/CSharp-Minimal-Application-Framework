//using System;


//namespace MinimalAF {
//    class TestError : Element {
//        Exception exception;

//        public TestError(Exception exception) {
//            this.exception = exception;
//        }

//        public override void OnMount() {
//            SetClearColor(Color.VA(1, 1));
//        }

//        public override void OnRender() {
//            SetFont("Consolas", 24);

//            string text = "An exception occured:\n" +
//                exception.Message;

//            DrawText(text, ctx.Width * 0.5f, ctx.Height * 0.5f, HAlign.Center, VAlign.Center);
//        }

//        public override void OnUpdate() {
//            if (KeyPressed(KeyCode.Escape)) {
//                //GetAncestor<ApplicationWindow>().Close();
//            }
//        }
//    }
//}

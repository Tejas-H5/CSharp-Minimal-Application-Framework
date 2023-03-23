using MinimalAF;
using OpenTK.Mathematics;

namespace TextEditor {

    static class AppConfig {
        public static Color BG => Color.White;
        public static Color FG => Color.Black;
        public static Color ErrorCol => Color.Red;

        static DrawableFont? _mainFont;
        public static DrawableFont MainFont {
            get {
                if (_mainFont == null) {
                    _mainFont = new DrawableFont("Source Code Pro", 24);
                }

                return _mainFont;
            }
        }
    }


    class Program {
        static void Main(string[] args) {
#if DEBUG
            var test = new Testing();
            TextBuffer.RunTests(test);
#endif


            new ProgramWindow((FrameworkContext ctx) => {
                ctx.SetClearColor(AppConfig.BG);
                ctx.Window.Title = "Text editor";
                ctx.Window.SetWindowState(WindowState.Maximized);

                ctx.Window.RenderFrequency = 60;
                // ctx.Window.UpdateFrequency = 0;

                return new TextEditor();
            }).Run();

            Console.WriteLine("End");
        }
    }
}

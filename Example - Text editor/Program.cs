using MinimalAF;
using OpenTK.Mathematics;

namespace TextEditor {

    static class AppConfig {
        public static Color BG => Color.White;
        public static Color FG => Color.Black;
        public static Color ErrorCol => Color.Red;

        public static string EditorFont = "Source code pro";
        public static int FontSize1 = 24;
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
        }
    }
}

using MinimalAF;
using OpenTK.Mathematics;

namespace TextEditor {

    static class AppColors {
        public static Color BG => Color.White;
        public static Color FG => Color.Black;
    }


    class Program {
        static void Main(string[] args) {
#if DEBUG
            var test = new Testing();
            TextBuffer.RunTests(test);
#endif


            new ProgramWindow((FrameworkContext ctx) => {
                ctx.SetClearColor(AppColors.BG);
                ctx.Window.Title = "Text editor";
                ctx.Window.SetWindowState(WindowState.Maximized);

                ctx.Window.RenderFrequency = 60;
                // ctx.Window.UpdateFrequency = 0;

                return new TextEditor();
            }).Run();
        }
    }
}

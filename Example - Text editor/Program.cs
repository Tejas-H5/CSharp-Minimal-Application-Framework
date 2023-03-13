using MinimalAF;

namespace TextEditor {

    static class AppColors {
        public static Color BG => Color.White;
        public static Color FG => Color.Black;
    }


    class TextEditor : IRenderable {
        public void Render(FrameworkContext ctx) {
            ctx.SetFont("Source code pro", 24);
            ctx.SetDrawColor(AppColors.FG);
            ctx.DrawText("Test", 10, ctx.VH - 10, HAlign.Left, VAlign.Top);
        }
    }


    class Program {
        static void Main(string[] args) {
            new ProgramWindow((FrameworkContext ctx) => {
                ctx.SetClearColor(AppColors.BG);
                ctx.Window.Title = "Text editor";
                ctx.Window.SetWindowState(WindowState.Maximized);

                return new TextEditor();
            }).Run();
        }
    }
}

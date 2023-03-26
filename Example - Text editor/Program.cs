using MinimalAF;
using OpenTK.Mathematics;
using TextEditor;

namespace Main {

    class Program {
        static void Main(string[] args) {
            // Text editor
            new ProgramWindow((FrameworkContext ctx) => new TextEditorRoot(ctx)).Run();
        }
    }
}

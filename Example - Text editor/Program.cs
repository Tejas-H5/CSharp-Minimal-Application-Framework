using MinimalAF;
using OpenTK.Mathematics;
using TextEditor;

namespace Main {

    class Program {
        static void Main(string[] args) {
            // Text editor
            new ProgramWindow((AFContext ctx) => new TextEditorRoot(ctx)).Run();
        }
    }
}

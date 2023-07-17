using MinimalAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor {
    public class TextEditorRoot : IRenderable {
        TextEditor _editor = new TextEditor();

        public TextEditorRoot(AFContext ctx) {
            ctx.SetClearColor(AppConfig.BG);
            ctx.Window.Title = "Text editor";
            ctx.Window.SetWindowState(WindowState.Maximized);

            ctx.Window.RenderFrequency = 60;
            // ctx.Window.UpdateFrequency = 0;
        }

        public void Render(AFContext ctx) {
            _editor.Render(ctx);
        }
    }
}

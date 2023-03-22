using MinimalAF;
using OpenTK.Mathematics;

namespace TextEditor {

    class TextInputPrompt {
        TextArea _textInput;
        string _error;
        string _title;
        Func<string, string> _callback;

        public bool AutoClose = true;

        public TextInputPrompt(string title, Func<string, string> callback) {
            _textInput = new TextArea("");
            _textInput.LineNumbers = false;
            _textInput.AcceptsNewLines = false;

            _error = "";

            _title = title;
            _callback = callback;
        }

        public void Reset() {
            _error = "";
        }
        
        public void Render(FrameworkContext ctx) {
            ctx.SetFont(AppConfig.EditorFont, AppConfig.FontSize1);
            var charHeight = ctx.GetCharHeight();
            var padding = 5;

            var rect = new Rect(0, 0, ctx.VW, charHeight + 2 * padding);

            ctx.SetDrawColor(AppConfig.BG, 0.8f);
            ctx.SetTexture(null);
            IM.Rect(ctx, rect);
            IM.RectOutline(ctx, 2, rect);

            ctx.SetDrawColor(AppConfig.FG);
            ctx.SetTexture(ctx.CurrentFontTexture);
            var labelEnd = _font.Draw(ctx, _title, padding, padding);

            ctx.SetDrawColor(AppConfig.ErrorCol);
            labelEnd = _font.Draw(ctx, _error, labelEnd.X + padding, padding);

            rect.X0 = labelEnd.X + padding;

            _textInput.Render(
                ctx.WithRect(rect).Use()
            );

            if (ctx.KeyJustPressed(KeyCode.Enter)) {
                _error = _callback(_textInput.GetText());
                if (AutoClose && _error == "") {
                    _textInput.Clear();
                }
            }
        }
    }

    class TextEditor : IRenderable {
        TextArea _mainTextArea;
        TextInputPrompt _gotoLinePrompt;
        TextInputPrompt _findNext;
        TextInputPrompt? _currentPrompt;

        public TextEditor() {
            // _buffer = new TextBuffer("");
            _mainTextArea = new TextArea(
                TestData.TextTextCSharpCodeIDKwhereitsfromguysWhatCouldItBe
                    .Replace("\r", "")
            );

            _gotoLinePrompt = new TextInputPrompt("Go to line: ", MoveMainTextAreaToLine);

            _findNext = new TextInputPrompt("Find next: ", FindNext);
            _findNext.AutoClose = false;
        }

        string MoveMainTextAreaToLine(string line) {
            if (!int.TryParse(line, out int lineInt)) {
                return "Couldn't parse '" + line + "' :(";
            }

            if (_mainTextArea.GoToLine(lineInt) == -1) {
                return "Couldn't find line " + line;
            }

            _currentPrompt = null;
            return "";
        }

        string FindNext(string line) {
            return "Not yet implemented";
        }


        public void Render(FrameworkContext ctx) {
            _mainTextArea.RenderText(
                ctx,
                _currentPrompt == null
            );

            if (_currentPrompt != null) {
                _currentPrompt.Render(ctx);
            }


            // TODO: clean up if we add any more here
            if (ctx.KeyJustPressed(KeyCode.G) && ctx.KeyIsDown(KeyCode.Control)) {
                _currentPrompt = _gotoLinePrompt;
                _gotoLinePrompt.Reset();
            } else if (ctx.KeyJustPressed(KeyCode.F) && ctx.KeyIsDown(KeyCode.Control)) {
                _currentPrompt = _findNext;
                _findNext.Reset();
            } else if (ctx.KeyJustPressed(KeyCode.Escape)) {
                _currentPrompt = null;
                _gotoLinePrompt.Reset();
            }
        }
    }
}

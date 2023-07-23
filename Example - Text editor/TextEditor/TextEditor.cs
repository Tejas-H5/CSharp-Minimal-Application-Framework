using MinimalAF;
using OpenTK.Mathematics;

namespace TextEditor {

    class TextInputPrompt {
        public bool AutoClose = true;

        TextArea _textInput;

        string _title = "";
        string _error = "";
        bool _justEdited;

        public TextInputPrompt(Action<string> callback) {
            _textInput = new TextArea("");
            _textInput.LineNumbers = false;
            _textInput.AcceptsNewLines = false;
            _textInput.OnTextEdited = () => {
                _justEdited = true;
            };

            _error = "";
        }

        public void Clear() {
            _textInput.Clear();
        }

        public void SetTitle(string title) {
            _title = title;
        }

        public void SetError(string err) {
            _error = err;
        }
        
        public void Render(AFContext ctx) {
            var charHeight = 24;
            var padding = 5;

            var rect = new Rect(0, 0, ctx.VW, charHeight + 2 * padding);

            ctx.SetDrawColor(AppConfig.BG, 0.8f);
            ctx.SetTexture(null);
            IM.DrawRect(ctx, rect);
            IM.DrawRectOutline(ctx, 2, rect);

            ctx.SetDrawColor(AppConfig.FG);
            ctx.SetTexture(AppConfig.MainFont.Texture);
            var labelEnd = AppConfig.MainFont.DrawText(ctx, _title, charHeight, new DrawTextOptions {
                X = padding, Y = padding
            });

            ctx.SetDrawColor(AppConfig.ErrorCol);
            labelEnd = AppConfig.MainFont.DrawText(ctx, _error, charHeight, new DrawTextOptions {
                X = labelEnd.Bounds.Width + padding, Y = padding
            });

            rect.X0 = labelEnd.Bounds.Width + padding;

            _textInput.Render(
                ctx.WithRect(rect).Use()
            );

            if (_justEdited) {
                _justEdited = false;
                // todo: callback. or return a bool and branch on it
            }
        }
    }

    class TextEditor : IRenderable {
        TextArea _editorTextArea;
        TextInputPrompt _inputPrompt;

        enum InputCommandType {
            None, GotoLine, FindText
        }
        InputCommandType _currentInputCommand = InputCommandType.None;
        int _commandStartCursorPos;

        string _findKeyword;
        int _findDirection = 1;

        public TextEditor() {
            // _buffer = new TextBuffer("");
            _editorTextArea = new TextArea(
                TestData.TextTextCSharpCodeIDKwhereitsfromguysWhatCouldItBe
                    .Replace("\r", "")
            );

            _inputPrompt = new TextInputPrompt(OnTextInput);
            _inputPrompt.AutoClose = false;
        }

        void OnTextInput(string line) {
            if (_currentInputCommand == InputCommandType.None) {
                return;
            }

            // goto line command
            if (_currentInputCommand == InputCommandType.GotoLine) {
                _inputPrompt.SetError("");
                if (!int.TryParse(line, out int lineInt)) {
                    if (line.Length > 0) {
                        _inputPrompt.SetError("Couldn't parse '" + line + "' :(");
                    }
                    return;
                }

                int newPos = _editorTextArea.GetLinePos(lineInt);
                if (newPos == -1) {
                    _inputPrompt.SetError("Couldn't find line " + line);
                    return;
                }

                _editorTextArea.CursorPos = newPos;
                return;
            }

            // find text command
            if (_currentInputCommand == InputCommandType.FindText) {
                _findKeyword = line;

                AdvanceFindCommand(_commandStartCursorPos);
                return;
            }
        }

        void AdvanceFindCommand(int fromPos) {
            _inputPrompt.SetError("");

            int newPos = -1;
            if (_findKeyword != "") {
                if (_findDirection == 1) {
                    newPos = _editorTextArea.IndexOf(_findKeyword, fromPos + 1);
                } else {
                    newPos = _editorTextArea.PrevIndexOf(_findKeyword, fromPos - 1);
                }
            }

            if (newPos == -1) {
                _inputPrompt.SetError("Couldn't find any more occurances.");
            } else {
                _editorTextArea.CursorPos = newPos;
            }
        }


        void StartInputCommand(InputCommandType commandType, string title) {
            _currentInputCommand = commandType;
            _inputPrompt.Clear();
            _inputPrompt.SetError("");
            _inputPrompt.SetTitle(title);
            _commandStartCursorPos = _editorTextArea.CursorPos;
        }

        void EscapeCurrentInputCommand() {
            _currentInputCommand = InputCommandType.None;
            if (_commandStartCursorPos != -1) {
                _editorTextArea.CursorPos = _commandStartCursorPos;
            }
            _commandStartCursorPos = -1;
        }

        void ConfirmCurrentInputCommand() {
            _currentInputCommand = InputCommandType.None;
            _commandStartCursorPos = -1;
        }

        public void Render(AFContext ctx) {
            _editorTextArea.RenderText(ctx, _currentInputCommand == InputCommandType.None);

            // process inputs for the current command
            if (_currentInputCommand != InputCommandType.None) {
                _inputPrompt.Render(ctx);

                switch (_currentInputCommand) {
                    case InputCommandType.FindText:
                        bool hasCtrl = ctx.KeyIsDown(KeyCode.Ctrl);
                        if (hasCtrl) {
                            for(int i = 0; i < ctx.KeysJustPressedOrRepeated.Count; i++) {
                                var key = ctx.KeysJustPressedOrRepeated[i];
                                if (key == KeyCode.A) {
                                    // move to next occurance
                                    _findDirection = -1;
                                    AdvanceFindCommand(_editorTextArea.CursorPos);
                                } else if (key == KeyCode.D) {
                                    // move to previous occurance
                                    _findDirection = 1;
                                    AdvanceFindCommand(_editorTextArea.CursorPos);
                                } else {
                                    continue;
                                }
                            }
                        }
                        break;
                }

                if (ctx.KeyJustPressed(KeyCode.Enter)) {
                    ConfirmCurrentInputCommand();
                }
            }

            // process input commands 
            {
                if (ctx.KeyJustPressed(KeyCode.G) && ctx.KeyIsDown(KeyCode.Ctrl)) {
                    StartInputCommand(InputCommandType.GotoLine, "Go to line: ");
                } else if (ctx.KeyJustPressed(KeyCode.F) && ctx.KeyIsDown(KeyCode.Ctrl)) {
                    StartInputCommand(InputCommandType.FindText, "Find: ");
                    _findDirection = 1;
                } else if (ctx.KeyJustPressed(KeyCode.Escape)) {
                    EscapeCurrentInputCommand();
                }
            }
        }
    }
}

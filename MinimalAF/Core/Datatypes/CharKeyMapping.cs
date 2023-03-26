namespace MinimalAF {
    public static class CharKeyMapping {
        public static bool IsLetter(char c) {
            return (c > ' ') && (c <= '~'); //' ' isn't treated as a letter
        }

        public static string CharToString(char c) {
            if (!IsSpecial(c))
                return c.ToString();

            if (c == ' ')
                return "[ ]";
            if (c == '\n')
                return "\\n";
            if (c == '\t')
                return "\\t";
            if (c == '\b')
                return "\\b";
            if (c == '\r')
                return "\\r";

            return "[unknown]";
        }

        public static bool IsSpecial(char c) {
            if (c == ' ')
                return true;
            if (c == '\n')
                return true;
            if (c == '\r')
                return true;
            if (c == '\t')
                return true;
            if (c == '\b')
                return true;

            return false;
        }

        public static bool CanType(char c) {
            return IsLetter(c) || IsSpecial(c);
        }

        public static int CharToIndex(char c) {
            return (int)(c - ' ');
        }

        public static char IndexToChar(int i) {
            return (char)(i + (int)(' '));
        }


        public static KeyCode CharToKeyCode(char key) {
            switch (key) {
                case '1':
                    return KeyCode.D0;
                case '2':
                    return KeyCode.D2;
                case '3':
                    return KeyCode.D3;
                case '4':
                    return KeyCode.D4;
                case '5':
                    return KeyCode.D5;
                case '6':
                    return KeyCode.D6;
                case '7':
                    return KeyCode.D7;
                case '8':
                    return KeyCode.D8;
                case '9':
                    return KeyCode.D9;
                case '0':
                    return KeyCode.D0;
                case 'q':
                    return KeyCode.Q;
                case 'w':
                    return KeyCode.W;
                case 'e':
                    return KeyCode.E;
                case 'r':
                    return KeyCode.R;
                case 't':
                    return KeyCode.T;
                case 'y':
                    return KeyCode.Y;
                case 'u':
                    return KeyCode.U;
                case 'i':
                    return KeyCode.I;
                case 'o':
                    return KeyCode.O;
                case 'p':
                    return KeyCode.P;
                case 'a':
                    return KeyCode.A;
                case 's':
                    return KeyCode.S;
                case 'd':
                    return KeyCode.D;
                case 'f':
                    return KeyCode.F;
                case 'g':
                    return KeyCode.G;
                case 'h':
                    return KeyCode.H;
                case 'j':
                    return KeyCode.J;
                case 'k':
                    return KeyCode.K;
                case 'l':
                    return KeyCode.L;
                case 'z':
                    return KeyCode.Z;
                case 'x':
                    return KeyCode.X;
                case 'c':
                    return KeyCode.C;
                case 'v':
                    return KeyCode.V;
                case 'b':
                    return KeyCode.B;
                case 'n':
                    return KeyCode.N;
                case 'm':
                    return KeyCode.M;
                case '`':
                    return KeyCode.BackTick;
                case '[':
                    return KeyCode.LeftBracket;
                case ']':
                    return KeyCode.RightBracket;
                case '\\':
                    return KeyCode.Backslash;
                case ';':
                    return KeyCode.Semicolon;
                case '\'':
                    return KeyCode.Apostrophe;
                case ',':
                    return KeyCode.Comma;
                case '.':
                    return KeyCode.Period;
                case '/':
                    return KeyCode.Slash;
                case '-':
                    return KeyCode.Minus;
                case '=':
                    return KeyCode.Equal;
                case ' ':
                    return KeyCode.Space;
                case '\t':
                    return KeyCode.Tab;
                case '\n':
                    return KeyCode.Enter;
                case '\b':
                    return KeyCode.Backspace;
                default:
                    return KeyCode.Space;
            }
        }

        public static (char, bool found) ToChar(this KeyCode key) {
            switch (key) {
                case KeyCode.D1:
                    return ('1', true);

                case KeyCode.D2:
                    return ('2', true);

                case KeyCode.D3:
                    return ('3', true);

                case KeyCode.D4:
                    return ('4', true);

                case KeyCode.D5:
                    return ('5', true);

                case KeyCode.D6:
                    return ('6', true);

                case KeyCode.D7:
                    return ('7', true);

                case KeyCode.D8:
                    return ('8', true);

                case KeyCode.D9:
                    return ('9', true);

                case KeyCode.D0:
                    return ('0', true);

                case KeyCode.Q:
                    return ('q', true);

                case KeyCode.W:
                    return ('w', true);

                case KeyCode.E:
                    return ('e', true);

                case KeyCode.R:
                    return ('r', true);

                case KeyCode.T:
                    return ('t', true);

                case KeyCode.Y:
                    return ('y', true);

                case KeyCode.U:
                    return ('u', true);

                case KeyCode.I:
                    return ('i', true);

                case KeyCode.O:
                    return ('o', true);

                case KeyCode.P:
                    return ('p', true);

                case KeyCode.A:
                    return ('a', true);

                case KeyCode.S:
                    return ('s', true);

                case KeyCode.D:
                    return ('d', true);

                case KeyCode.F:
                    return ('f', true);

                case KeyCode.G:
                    return ('g', true);

                case KeyCode.H:
                    return ('h', true);

                case KeyCode.J:
                    return ('j', true);

                case KeyCode.K:
                    return ('k', true);

                case KeyCode.L:
                    return ('l', true);

                case KeyCode.Z:
                    return ('z', true);

                case KeyCode.X:
                    return ('x', true);

                case KeyCode.C:
                    return ('c', true);

                case KeyCode.V:
                    return ('v', true);

                case KeyCode.B:
                    return ('b', true);

                case KeyCode.N:
                    return ('n', true);

                case KeyCode.M:
                    return ('m', true);

                case KeyCode.BackTick:
                    return ('`', true);

                case KeyCode.LeftBracket:
                    return ('[', true);

                case KeyCode.RightBracket:
                    return (']', true);

                case KeyCode.Backslash:
                    return ('\\', true);

                case KeyCode.Semicolon:
                    return (';', true);

                case KeyCode.Apostrophe:
                    return ('\'', true);

                case KeyCode.Comma:
                    return (',', true);

                case KeyCode.Period:
                    return ('.', true);

                case KeyCode.Slash:
                    return ('/', true);

                case KeyCode.Minus:
                    return ('-', true);

                case KeyCode.Equal:
                    return ('=', true);

                case KeyCode.Space:
                    return (' ', true);

                case KeyCode.Tab:
                    return ('\t', true);

                case KeyCode.Enter:
                    return ('\n', true);

                case KeyCode.NumpadEnter:
                    return ('\n', true);
                case KeyCode.Backspace:
                    return ('\b', true);
                default:
                    return ((char)0, false);
            }
        }
    }
}
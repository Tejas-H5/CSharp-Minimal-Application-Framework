using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderingEngine.Logic;
using System.Collections;
using System.Collections.Generic;

namespace RenderingEngine.Logic
{
    public static class CharKeyMapping
    {
        public static bool IsLetter(char c)
        {
            return (c > ' ') && (c <= '~'); //' ' isn't treated as a letter
        }

        public static string CharToString(char c)
        {
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

        public static bool IsSpecial(char c)
        {
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

        public static bool CanType(char c)
        {
            return IsLetter(c) || IsSpecial(c);
        }

        public static int CharToIndex(char c)
        {
            return (int)(c - ' ');
        }

        public static char IndexToChar(int i)
        {
            return (char)(i + (int)(' '));
        }


        public static KeyCode CharToKeyCode(char key)
        {
            switch (key)
            {
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

        public static char KeyCodeToChar(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.D1:
                    { return '1'; }
                case KeyCode.D2:
                    { return '2'; }
                case KeyCode.D3:
                    { return '3'; }
                case KeyCode.D4:
                    { return '4'; }
                case KeyCode.D5:
                    { return '5'; }
                case KeyCode.D6:
                    { return '6'; }
                case KeyCode.D7:
                    { return '7'; }
                case KeyCode.D8:
                    { return '8'; }
                case KeyCode.D9:
                    { return '9'; }
                case KeyCode.D0:
                    { return '0'; }
                case KeyCode.Q:
                    { return 'q'; }
                case KeyCode.W:
                    { return 'w'; }
                case KeyCode.E:
                    { return 'e'; }
                case KeyCode.R:
                    { return 'r'; }
                case KeyCode.T:
                    { return 't'; }
                case KeyCode.Y:
                    { return 'y'; }
                case KeyCode.U:
                    { return 'u'; }
                case KeyCode.I:
                    { return 'i'; }
                case KeyCode.O:
                    { return 'o'; }
                case KeyCode.P:
                    { return 'p'; }
                case KeyCode.A:
                    { return 'a'; }
                case KeyCode.S:
                    { return 's'; }
                case KeyCode.D:
                    { return 'd'; }
                case KeyCode.F:
                    { return 'f'; }
                case KeyCode.G:
                    { return 'g'; }
                case KeyCode.H:
                    { return 'h'; }
                case KeyCode.J:
                    { return 'j'; }
                case KeyCode.K:
                    { return 'k'; }
                case KeyCode.L:
                    { return 'l'; }
                case KeyCode.Z:
                    { return 'z'; }
                case KeyCode.X:
                    { return 'x'; }
                case KeyCode.C:
                    { return 'c'; }
                case KeyCode.V:
                    { return 'v'; }
                case KeyCode.B:
                    { return 'b'; }
                case KeyCode.N:
                    { return 'n'; }
                case KeyCode.M:
                    { return 'm'; }
                case KeyCode.BackTick:
                    { return '`'; }
                case KeyCode.LeftBracket:
                    { return '['; }
                case KeyCode.RightBracket:
                    { return ']'; }
                case KeyCode.Backslash:
                    { return '\\'; }
                case KeyCode.Semicolon:
                    { return ';'; }
                case KeyCode.Apostrophe:
                    { return '\''; }
                case KeyCode.Comma:
                    { return ','; }
                case KeyCode.Period:
                    { return '.'; }
                case KeyCode.Slash:
                    { return '/'; }
                case KeyCode.Minus:
                    { return '-'; }
                case KeyCode.Equal:
                    { return '='; }
                case KeyCode.Space:
                    { return ' '; }
                case KeyCode.Tab:
                    { return '\t'; }
                case KeyCode.Enter:
                    { return '\n'; }
                case KeyCode.Backspace:
                    { return '\b'; }
            }

            return (char)0;
        }
    }
}
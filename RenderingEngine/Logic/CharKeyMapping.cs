using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections;
using System.Collections.Generic;

public static class CharKeyMapping
{
    public static bool IsLetter(char c)
    {
        return (c > ' ') && (c <= '~'); //' ' isn't treated as a letter
    }

    public static string CharToString(char c)
    {
        if (!IsSpecial(c))
            return "" + c;

        if (c == ' ') return "[ ]";
        if (c == '\n') return "\\n";
        if (c == '\t') return "tab";
        if (c == '\b') return "backspace";

        return "[unknown]";
    }

    public static bool IsSpecial(char c)
    {
        if (c == ' ') return true;
        if (c == '\n') return true;
        if (c == '\t') return true;
        if (c == '\b') return true;
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


    public static Keys CharToKey(char key)
    {
        switch (key)
        {
            case '1': return Keys.D0;
            case '2': return Keys.D2;
            case '3': return Keys.D3;
            case '4': return Keys.D4;
            case '5': return Keys.D5;
            case '6': return Keys.D6;
            case '7': return Keys.D7;
            case '8': return Keys.D8;
            case '9': return Keys.D9;
            case '0': return Keys.D0;
            case 'q': return Keys.Q;
            case 'w': return Keys.W;
            case 'e': return Keys.E;
            case 'r': return Keys.R;
            case 't': return Keys.T;
            case 'y': return Keys.Y;
            case 'u': return Keys.U;
            case 'i': return Keys.I;
            case 'o': return Keys.O;
            case 'p': return Keys.P;
            case 'a': return Keys.A;
            case 's': return Keys.S;
            case 'd': return Keys.D;
            case 'f': return Keys.F;
            case 'g': return Keys.G;
            case 'h': return Keys.H;
            case 'j': return Keys.J;
            case 'k': return Keys.K;
            case 'l': return Keys.L;
            case 'z': return Keys.Z;
            case 'x': return Keys.X;
            case 'c': return Keys.C;
            case 'v': return Keys.V;
            case 'b': return Keys.B;
            case 'n': return Keys.N;
            case 'm': return Keys.M;
            case '`': return Keys.GraveAccent;
            case '[': return Keys.LeftBracket;
            case ']': return Keys.RightBracket;
            case '\\': return Keys.Backslash;
            case ';': return Keys.Semicolon;
            case '\'': return Keys.Apostrophe;
            case ',': return Keys.Comma;
            case '.': return Keys.Period;
            case '/': return Keys.Slash;
            case '-': return Keys.Minus;
            case '=': return Keys.Equal;
            case ' ': return Keys.Space;
            case '\t': return Keys.Tab;
            case '\n': return Keys.Enter;
            case '\b': return Keys.Backspace;
            default: return Keys.Space;
        }
    }

    public static char KeyCodeToChar(Keys key)
    {
        switch (key)
        {
            case Keys.D1: { return '1'; }
            case Keys.D2: { return '2'; }
            case Keys.D3: { return '3'; }
            case Keys.D4: { return '4'; }
            case Keys.D5: { return '5'; }
            case Keys.D6: { return '6'; }
            case Keys.D7: { return '7'; }
            case Keys.D8: { return '8'; }
            case Keys.D9: { return '9'; }
            case Keys.D0: { return '0'; }
            case Keys.Q: { return 'q'; }
            case Keys.W: { return 'w'; }
            case Keys.E: { return 'e'; }
            case Keys.R: { return 'r'; }
            case Keys.T: { return 't'; }
            case Keys.Y: { return 'y'; }
            case Keys.U: { return 'u'; }
            case Keys.I: { return 'i'; }
            case Keys.O: { return 'o'; }
            case Keys.P: { return 'p'; }
            case Keys.A: { return 'a'; }
            case Keys.S: { return 's'; }
            case Keys.D: { return 'd'; }
            case Keys.F: { return 'f'; }
            case Keys.G: { return 'g'; }
            case Keys.H: { return 'h'; }
            case Keys.J: { return 'j'; }
            case Keys.K: { return 'k'; }
            case Keys.L: { return 'l'; }
            case Keys.Z: { return 'z'; }
            case Keys.X: { return 'x'; }
            case Keys.C: { return 'c'; }
            case Keys.V: { return 'v'; }
            case Keys.B: { return 'b'; }
            case Keys.N: { return 'n'; }
            case Keys.M: { return 'm'; }
            case Keys.GraveAccent: { return '`'; }
            case Keys.LeftBracket: { return '['; }
            case Keys.RightBracket: { return ']'; }
            case Keys.Backslash: { return '\\'; }
            case Keys.Semicolon: { return ';'; }
            case Keys.Apostrophe: { return '\''; }
            case Keys.Comma: { return ','; }
            case Keys.Period: { return '.'; }
            case Keys.Slash: { return '/'; }
            case Keys.Minus: { return '-'; }
            case Keys.Equal: { return '='; }
            case Keys.Space: { return ' '; }
            case Keys.Tab: { return '\t'; }
            case Keys.Enter: { return '\n'; }
            case Keys.Backspace: { return '\b'; }
        }

        return (char)0;
    }

}

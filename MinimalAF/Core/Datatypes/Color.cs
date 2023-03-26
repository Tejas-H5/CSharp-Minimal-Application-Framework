using System;

namespace MinimalAF {
    /// <summary>
    /// OpenTK's Color4 has a float constructor as well as an int constructor
    /// so writing new Color4(1,1,1) vs new Color4(1.0f,1.0f,1.0f) has two different outcomes
    /// which I dont particularly like. So I made a new class just for that.
    /// 
    /// I have also renamed it to just Color, as there are situations where I want to use OpenTK.Mathematics; and the
    /// two types were conflicting.
    /// </summary>
    public struct Color {
        public float R;
        public float G;
        public float B;
        public float A;

        private Color(float r, float g, float b, float a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static implicit operator OpenTK.Mathematics.Color4(Color value) {
            return new OpenTK.Mathematics.Color4(value.R, value.G, value.B, value.A);
        }

        /// <summary>
        /// bytes, 0-255
        /// </summary>
        public static Color RGBA255(byte r, byte g, byte b, byte a) {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        /// <summary>
        /// ints, 0-255
        /// </summary>
        public static Color RGB255(byte r, byte g, byte b) {
            return RGBA255(r, g, b, (byte)255);
        }


        /// <summary>
        /// floats, 0.0f to 1.0f
        /// </summary>
        public static Color RGB(float r, float g, float b) {
            return Color.RGBA(r, g, b, 1.0f);
        }



        /// <summary>
        /// Make a color from HSV. Think of that colour circle you often see in drawing prorams,
        /// </summary>
        /// <param name="h">An angle around the color circle in degrees</param>
        /// <param name="s">A distance from the center of the color circle from 0 being the center to 1 being the outside</param>
        /// <param name="v">Darkness, with 0 being dark and 1 being bright</param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Color HSVA(float h, float s, float v, float a) {
            float c = v * s;
            float x = c * (1 - MathF.Abs(h / 60) % 2 - 1);
            float m = v - c;
            float r = 0, g = 0, b = 0;
            if (h < 60) {
                r = c;
                g = x;
            } else if (h < 120) {
                r = x;
                g = c;
            } else if (h < 180) {
                g = c;
                b = x;
            } else if (h < 240) {
                g = x;
                b = c;
            } else if (h < 300) {
                r = x;
                b = c;
            } else {
                r = c;
                b = x;
            }

            return new Color(r + m, g + m, b + m, a);
        }

        public static Color HSV(float h, float s, float v) {
            return HSVA(h, s, v, 1f);
        }

        /// <summary>
        /// floats, 0.0f to 1.0f
        /// </summary>
        public static Color RGBA(float r, float g, float b, float a) {
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// floats, 0.0f to 1.0f.
        /// Value, like value from HSV, and Alpha
        /// </summary>
        public static Color VA(float v, float a) {
            return new Color(v, v, v, a);
        }

        public static bool operator ==(Color a, Color b) {
            float eps = 0.0001f;
            return MathF.Abs(a.R - b.R) +
                MathF.Abs(a.G - b.G) +
                MathF.Abs(a.B - b.B) +
                MathF.Abs(a.A - b.A) < eps;
        }

        public static bool operator !=(Color a, Color b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            return obj is Color col && col == this;
        }

        public override int GetHashCode() {
            return HashCode.Combine(R.GetHashCode(), G.GetHashCode(), B.GetHashCode(), A.GetHashCode());
        }

        #region All CSS colors
        public static Color AliceBlue => RGB255(240, 248, 255);
        public static Color AntiqueWhite => RGB255(250, 235, 215);
        public static Color Aqua => RGB255(0, 255, 255);
        public static Color Aquamarine => RGB255(127, 255, 212);
        public static Color Azure => RGB255(240, 255, 255);
        public static Color Beige => RGB255(245, 245, 220);
        public static Color Bisque => RGB255(255, 228, 196);
        public static Color Black => RGB255(0, 0, 0);
        public static Color BlanchedAlmond => RGB255(255, 235, 205);
        public static Color Blue => RGB255(0, 0, 255);
        public static Color BlueViolet => RGB255(138, 43, 226);
        public static Color Brown => RGB255(165, 42, 42);
        public static Color BurlyWood => RGB255(222, 184, 135);
        public static Color CadetBlue => RGB255(95, 158, 160);
        public static Color Chartreuse => RGB255(127, 255, 0);
        public static Color Chocolate => RGB255(210, 105, 30);
        public static Color Coral => RGB255(255, 127, 80);
        public static Color CornflowerBlue => RGB255(100, 149, 237);
        public static Color CornSilk => RGB255(255, 248, 220);
        public static Color Crimson => RGB255(220, 20, 60);
        public static Color Cyan => RGB255(0, 255, 255);
        public static Color DarkBlue => RGB255(0, 0, 139);
        public static Color DarkCyan => RGB255(0, 139, 139);
        public static Color DarkGoldenrod => RGB255(184, 134, 11);
        public static Color DarkGray => RGB255(169, 169, 169);
        public static Color DarkGreen => RGB255(0, 100, 0);
        public static Color DarkGrey => RGB255(169, 169, 169);
        public static Color DarkKhaki => RGB255(189, 183, 107);
        public static Color DarkMagenta => RGB255(139, 0, 139);
        public static Color DarkOliveGreen => RGB255(85, 107, 47);
        public static Color DarkOrange => RGB255(255, 140, 0);
        public static Color DarkOrchid => RGB255(153, 50, 204);
        public static Color DarkRed => RGB255(139, 0, 0);
        public static Color DarkSalmon => RGB255(233, 150, 122);
        public static Color DarkSeaGreen => RGB255(143, 188, 143);
        public static Color DarkSlateBlue => RGB255(72, 61, 139);
        public static Color DarkSlateGray => RGB255(47, 79, 79);
        public static Color DarkSlateGrey => RGB255(47, 79, 79);
        public static Color DarkTurquoise => RGB255(0, 206, 209);
        public static Color DarkViolet => RGB255(148, 0, 211);
        public static Color DeepPink => RGB255(255, 20, 147);
        public static Color DeepSkyBlue => RGB255(0, 191, 255);
        public static Color DimGray => RGB255(105, 105, 105);
        public static Color DimGrey => RGB255(105, 105, 105);
        public static Color DodgerBlue => RGB255(30, 144, 255);
        public static Color Firebrick => RGB255(178, 34, 34);
        public static Color FloralWhite => RGB255(255, 250, 240);
        public static Color ForestGreen => RGB255(34, 139, 34);
        public static Color Fuchsia => RGB255(255, 0, 255);
        public static Color Gainsboro => RGB255(220, 220, 220);
        public static Color GhostWhite => RGB255(248, 248, 255);
        public static Color Gold => RGB255(255, 215, 0);
        public static Color Goldenrod => RGB255(218, 165, 32);
        public static Color Gray => RGB255(128, 128, 128);
        public static Color Green => RGB255(0, 128, 0);
        public static Color GreenYellow => RGB255(173, 255, 47);
        public static Color Grey => RGB255(128, 128, 128);
        public static Color Honeydew => RGB255(240, 255, 240);
        public static Color HotPink => RGB255(255, 105, 180);
        public static Color IndianRed => RGB255(205, 92, 92);
        public static Color Indigo => RGB255(75, 0, 130);
        public static Color Ivory => RGB255(255, 255, 240);
        public static Color Khaki => RGB255(240, 230, 140);
        public static Color Lavender => RGB255(230, 230, 250);
        public static Color LavenderBlush => RGB255(255, 240, 245);
        public static Color LawnGreen => RGB255(124, 252, 0);
        public static Color LemonChiffon => RGB255(255, 250, 205);
        public static Color LightBlue => RGB255(173, 216, 230);
        public static Color LightCoral => RGB255(240, 128, 128);
        public static Color LightCyan => RGB255(224, 255, 255);
        public static Color LightGoldenrodYellow => RGB255(250, 250, 210);
        public static Color LightGray => RGB255(211, 211, 211);
        public static Color LightGreen => RGB255(144, 238, 144);
        public static Color LightGrey => RGB255(211, 211, 211);
        public static Color LightPink => RGB255(255, 182, 193);
        public static Color LightSalmon => RGB255(255, 160, 122);
        public static Color LightSeaGreen => RGB255(32, 178, 170);
        public static Color LightSkyBlue => RGB255(135, 206, 250);
        public static Color LightSlateGray => RGB255(119, 136, 153);
        public static Color LightSlateGrey => RGB255(119, 136, 153);
        public static Color LightSteelBlue => RGB255(176, 196, 222);
        public static Color LightYellow => RGB255(255, 255, 224);
        public static Color Lime => RGB255(0, 255, 0);
        public static Color LimeGreen => RGB255(50, 205, 50);
        public static Color Linen => RGB255(250, 240, 230);
        public static Color Magenta => RGB255(255, 0, 255);
        public static Color Maroon => RGB255(128, 0, 0);
        public static Color MediumAquamarine => RGB255(102, 205, 170);
        public static Color MediumBlue => RGB255(0, 0, 205);
        public static Color MediumOrchid => RGB255(186, 85, 211);
        public static Color MediumPurple => RGB255(147, 112, 219);
        public static Color MediumSeaGreen => RGB255(60, 179, 113);
        public static Color MediumSlateBlue => RGB255(123, 104, 238);
        public static Color MediumSpringGreen => RGB255(0, 250, 154);
        public static Color MediumTurquoise => RGB255(72, 209, 204);
        public static Color MediumVioletRed => RGB255(199, 21, 133);
        public static Color MidnightBlue => RGB255(25, 25, 112);
        public static Color MintCream => RGB255(245, 255, 250);
        public static Color MistyRose => RGB255(255, 228, 225);
        public static Color Moccasin => RGB255(255, 228, 181);
        public static Color NavajoWhite => RGB255(255, 222, 173);
        public static Color Navy => RGB255(0, 0, 128);
        public static Color OldLace => RGB255(253, 245, 230);
        public static Color Olive => RGB255(128, 128, 0);
        public static Color OliveDrab => RGB255(107, 142, 35);
        public static Color Orange => RGB255(255, 165, 0);
        public static Color OrangeRed => RGB255(255, 69, 0);
        public static Color Orchid => RGB255(218, 112, 214);
        public static Color PaleGoldenrod => RGB255(238, 232, 170);
        public static Color PaleGreen => RGB255(152, 251, 152);
        public static Color PaleTurquoise => RGB255(175, 238, 238);
        public static Color PaleVioletRed => RGB255(219, 112, 147);
        public static Color PapayaWhip => RGB255(255, 239, 213);
        public static Color PeachPuff => RGB255(255, 218, 185);
        public static Color Peru => RGB255(205, 133, 63);
        public static Color Pink => RGB255(255, 192, 203);
        public static Color Plum => RGB255(221, 160, 221);
        public static Color PowderBlue => RGB255(176, 224, 230);
        public static Color Purple => RGB255(128, 0, 128);
        public static Color Red => RGB255(255, 0, 0);
        public static Color RosyBrown => RGB255(188, 143, 143);
        public static Color RoyalBlue => RGB255(65, 105, 225);
        public static Color SaddleBrown => RGB255(139, 69, 19);
        public static Color Salmon => RGB255(250, 128, 114);
        public static Color SandyBrown => RGB255(244, 164, 96);
        public static Color SeaGreen => RGB255(46, 139, 87);
        public static Color Seashell => RGB255(255, 245, 238);
        public static Color Sienna => RGB255(160, 82, 45);
        public static Color Silver => RGB255(192, 192, 192);
        public static Color SkyBlue => RGB255(135, 206, 235);
        public static Color SlateBlue => RGB255(106, 90, 205);
        public static Color SlateGray => RGB255(112, 128, 144);
        public static Color SlateGrey => RGB255(112, 128, 144);
        public static Color Snow => RGB255(255, 250, 250);
        public static Color SpringGreen => RGB255(0, 255, 127);
        public static Color SteelBlue => RGB255(70, 130, 180);
        public static Color Tan => RGB255(210, 180, 140);
        public static Color Teal => RGB255(0, 128, 128);
        public static Color Thistle => RGB255(216, 191, 216);
        public static Color Tomato => RGB255(255, 99, 71);
        public static Color Turquoise => RGB255(64, 224, 208);
        public static Color Violet => RGB255(238, 130, 238);
        public static Color Wheat => RGB255(245, 222, 179);
        public static Color White => RGB255(255, 255, 255);
        public static Color WhiteSmoke => RGB255(245, 245, 245);
        public static Color Yellow => RGB255(255, 255, 0);
        public static Color YellowGreen => RGB255(154, 205, 50);
        #endregion
    }
}

using System;

namespace MinimalAF {
    /// <summary>
    /// OpenTK's Color4 has a float constructor as well as an int constructor
    /// so writing new Color4(1,1,1) vs new Color4(1.0f,1.0f,1.0f) has two different outcomes
    /// which I dont particularly like. So I made a new class just for that.
    /// 
    /// I
    /// </summary>
    public struct Color4 {
        public float R;
        public float G;
        public float B;
        public float A;

        private Color4(float r, float g, float b, float a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static implicit operator OpenTK.Mathematics.Color4(Color4 value) {
            return new OpenTK.Mathematics.Color4(value.R, value.G, value.B, value.A);
        }

        /// <summary>
        /// bytes, 0-255
        /// </summary>
        public static Color4 RGBA255(byte r, byte g, byte b, byte a) {
            return new Color4(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        /// <summary>
        /// floats, 0.0f to 1.0f
        /// </summary>
        public static Color4 RGB(float r, float g, float b) {
            return Color4.RGBA(r, g, b, 1.0f);
        }


		/// <summary>
        /// Make a color from HSV. Think of that colour circle you often see in drawing prorams,
        /// </summary>
        /// <param name="h">An angle around the color circle in degrees</param>
        /// <param name="s">A distance from the center of the color circle from 0 being the center to 1 being the outside</param>
        /// <param name="v">Darkness, with 0 being dark and 1 being bright</param>
        /// <param name="a"></param>
        /// <returns></returns>
		public static Color4 HSVA(float h, float s, float v, float a) {
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

			return new Color4(r + m, g + m, b + m, a);
		}

		/// <summary>
		/// floats, 0.0f to 1.0f
		/// </summary>
		public static Color4 RGBA(float r, float g, float b, float a) {
            return new Color4(r, g, b, a);
        }

        /// <summary>
        /// floats, 0.0f to 1.0f.
        /// Value, like value from HSV, and Alpha
        /// </summary>
        public static Color4 VA(float v, float a) {
            return new Color4(v, v, v, a);
        }

        public static bool operator == (Color4 a, Color4 b) {
            float eps = 0.0001f;
            return MathF.Abs(a.R - b.R) +
                MathF.Abs(a.G - b.B) +
                MathF.Abs(a.B - b.B) +
                MathF.Abs(a.A - b.A) < eps;
        }

        public static bool operator !=(Color4 a, Color4 b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            return obj is Color4 col && col == this;
        }

        public override int GetHashCode() {
            return HashCode.Combine(R.GetHashCode(), G.GetHashCode(), B.GetHashCode(), A.GetHashCode());
        }

        #region All CSS colors
        public static Color4 AliceBlue => RGB(240, 248, 255);
        public static Color4 AntiqueWhite => RGB(250, 235, 215);
        public static Color4 Aqua => RGB(0, 255, 255);
        public static Color4 Aquamarine => RGB(127, 255, 212);
        public static Color4 Azure => RGB(240, 255, 255);
        public static Color4 Beige => RGB(245, 245, 220);
        public static Color4 Bisque => RGB(255, 228, 196);
        public static Color4 Black => RGB(0, 0, 0);
        public static Color4 BlanchedAlmond => RGB(255, 235, 205);
        public static Color4 Blue => RGB(0, 0, 255);
        public static Color4 BlueViolet => RGB(138, 43, 226);
        public static Color4 Brown => RGB(165, 42, 42);
        public static Color4 BurlyWood => RGB(222, 184, 135);
        public static Color4 CadetBlue => RGB(95, 158, 160);
        public static Color4 Chartreuse => RGB(127, 255, 0);
        public static Color4 Chocolate => RGB(210, 105, 30);
        public static Color4 Coral => RGB(255, 127, 80);
        public static Color4 CornflowerBlue => RGB(100, 149, 237);
        public static Color4 CornSilk => RGB(255, 248, 220);
        public static Color4 Crimson => RGB(220, 20, 60);
        public static Color4 Cyan => RGB(0, 255, 255);
        public static Color4 DarkBlue => RGB(0, 0, 139);
        public static Color4 DarkCyan => RGB(0, 139, 139);
        public static Color4 DarkGoldenrod => RGB(184, 134, 11);
        public static Color4 DarkGray => RGB(169, 169, 169);
        public static Color4 DarkGreen => RGB(0, 100, 0);
        public static Color4 DarkGrey => RGB(169, 169, 169);
        public static Color4 DarkKhaki => RGB(189, 183, 107);
        public static Color4 DarkMagenta => RGB(139, 0, 139);
        public static Color4 DarkOliveGreen => RGB(85, 107, 47);
        public static Color4 DarkOrange => RGB(255, 140, 0);
        public static Color4 DarkOrchid => RGB(153, 50, 204);
        public static Color4 DarkRed => RGB(139, 0, 0);
        public static Color4 DarkSalmon => RGB(233, 150, 122);
        public static Color4 DarkSeaGreen => RGB(143, 188, 143);
        public static Color4 DarkSlateBlue => RGB(72, 61, 139);
        public static Color4 DarkSlateGray => RGB(47, 79, 79);
        public static Color4 DarkSlateGrey => RGB(47, 79, 79);
        public static Color4 DarkTurquoise => RGB(0, 206, 209);
        public static Color4 DarkViolet => RGB(148, 0, 211);
        public static Color4 DeepPink => RGB(255, 20, 147);
        public static Color4 DeepSkyBlue => RGB(0, 191, 255);
        public static Color4 DimGray => RGB(105, 105, 105);
        public static Color4 DimGrey => RGB(105, 105, 105);
        public static Color4 DodgerBlue => RGB(30, 144, 255);
        public static Color4 Firebrick => RGB(178, 34, 34);
        public static Color4 FloralWhite => RGB(255, 250, 240);
        public static Color4 ForestGreen => RGB(34, 139, 34);
        public static Color4 Fuchsia => RGB(255, 0, 255);
        public static Color4 Gainsboro => RGB(220, 220, 220);
        public static Color4 GhostWhite => RGB(248, 248, 255);
        public static Color4 Gold => RGB(255, 215, 0);
        public static Color4 Goldenrod => RGB(218, 165, 32);
        public static Color4 Gray => RGB(128, 128, 128);
        public static Color4 Green => RGB(0, 128, 0);
        public static Color4 GreenYellow => RGB(173, 255, 47);
        public static Color4 Grey => RGB(128, 128, 128);
        public static Color4 Honeydew => RGB(240, 255, 240);
        public static Color4 HotPink => RGB(255, 105, 180);
        public static Color4 IndianRed => RGB(205, 92, 92);
        public static Color4 Indigo => RGB(75, 0, 130);
        public static Color4 Ivory => RGB(255, 255, 240);
        public static Color4 Khaki => RGB(240, 230, 140);
        public static Color4 Lavender => RGB(230, 230, 250);
        public static Color4 LavenderBlush => RGB(255, 240, 245);
        public static Color4 LawnGreen => RGB(124, 252, 0);
        public static Color4 LemonChiffon => RGB(255, 250, 205);
        public static Color4 LightBlue => RGB(173, 216, 230);
        public static Color4 LightCoral => RGB(240, 128, 128);
        public static Color4 LightCyan => RGB(224, 255, 255);
        public static Color4 LightGoldenrodYellow => RGB(250, 250, 210);
        public static Color4 LightGray => RGB(211, 211, 211);
        public static Color4 LightGreen => RGB(144, 238, 144);
        public static Color4 LightGrey => RGB(211, 211, 211);
        public static Color4 LightPink => RGB(255, 182, 193);
        public static Color4 LightSalmon => RGB(255, 160, 122);
        public static Color4 LightSeaGreen => RGB(32, 178, 170);
        public static Color4 LightSkyBlue => RGB(135, 206, 250);
        public static Color4 LightSlateGray => RGB(119, 136, 153);
        public static Color4 LightSlateGrey => RGB(119, 136, 153);
        public static Color4 LightSteelBlue => RGB(176, 196, 222);
        public static Color4 LightYellow => RGB(255, 255, 224);
        public static Color4 Lime => RGB(0, 255, 0);
        public static Color4 LimeGreen => RGB(50, 205, 50);
        public static Color4 Linen => RGB(250, 240, 230);
        public static Color4 Magenta => RGB(255, 0, 255);
        public static Color4 Maroon => RGB(128, 0, 0);
        public static Color4 MediumAquamarine => RGB(102, 205, 170);
        public static Color4 MediumBlue => RGB(0, 0, 205);
        public static Color4 MediumOrchid => RGB(186, 85, 211);
        public static Color4 MediumPurple => RGB(147, 112, 219);
        public static Color4 MediumSeaGreen => RGB(60, 179, 113);
        public static Color4 MediumSlateBlue => RGB(123, 104, 238);
        public static Color4 MediumSpringGreen => RGB(0, 250, 154);
        public static Color4 MediumTurquoise => RGB(72, 209, 204);
        public static Color4 MediumVioletRed => RGB(199, 21, 133);
        public static Color4 MidnightBlue => RGB(25, 25, 112);
        public static Color4 MintCream => RGB(245, 255, 250);
        public static Color4 MistyRose => RGB(255, 228, 225);
        public static Color4 Moccasin => RGB(255, 228, 181);
        public static Color4 NavajoWhite => RGB(255, 222, 173);
        public static Color4 Navy => RGB(0, 0, 128);
        public static Color4 OldLace => RGB(253, 245, 230);
        public static Color4 Olive => RGB(128, 128, 0);
        public static Color4 OliveDrab => RGB(107, 142, 35);
        public static Color4 Orange => RGB(255, 165, 0);
        public static Color4 OrangeRed => RGB(255, 69, 0);
        public static Color4 Orchid => RGB(218, 112, 214);
        public static Color4 PaleGoldenrod => RGB(238, 232, 170);
        public static Color4 PaleGreen => RGB(152, 251, 152);
        public static Color4 PaleTurquoise => RGB(175, 238, 238);
        public static Color4 PaleVioletRed => RGB(219, 112, 147);
        public static Color4 PapayaWhip => RGB(255, 239, 213);
        public static Color4 PeachPuff => RGB(255, 218, 185);
        public static Color4 Peru => RGB(205, 133, 63);
        public static Color4 Pink => RGB(255, 192, 203);
        public static Color4 Plum => RGB(221, 160, 221);
        public static Color4 PowderBlue => RGB(176, 224, 230);
        public static Color4 Purple => RGB(128, 0, 128);
        public static Color4 Red => RGB(255, 0, 0);
        public static Color4 RosyBrown => RGB(188, 143, 143);
        public static Color4 RoyalBlue => RGB(65, 105, 225);
        public static Color4 SaddleBrown => RGB(139, 69, 19);
        public static Color4 Salmon => RGB(250, 128, 114);
        public static Color4 SandyBrown => RGB(244, 164, 96);
        public static Color4 SeaGreen => RGB(46, 139, 87);
        public static Color4 Seashell => RGB(255, 245, 238);
        public static Color4 Sienna => RGB(160, 82, 45);
        public static Color4 Silver => RGB(192, 192, 192);
        public static Color4 SkyBlue => RGB(135, 206, 235);
        public static Color4 SlateBlue => RGB(106, 90, 205);
        public static Color4 SlateGray => RGB(112, 128, 144);
        public static Color4 SlateGrey => RGB(112, 128, 144);
        public static Color4 Snow => RGB(255, 250, 250);
        public static Color4 SpringGreen => RGB(0, 255, 127);
        public static Color4 SteelBlue => RGB(70, 130, 180);
        public static Color4 Tan => RGB(210, 180, 140);
        public static Color4 Teal => RGB(0, 128, 128);
        public static Color4 Thistle => RGB(216, 191, 216);
        public static Color4 Tomato => RGB(255, 99, 71);
        public static Color4 Turquoise => RGB(64, 224, 208);
        public static Color4 Violet => RGB(238, 130, 238);
        public static Color4 Wheat => RGB(245, 222, 179);
        public static Color4 White => RGB(255, 255, 255);
        public static Color4 WhiteSmoke => RGB(245, 245, 245);
        public static Color4 Yellow => RGB(255, 255, 0);
        public static Color4 YellowGreen => RGB(154, 205, 50);

        #endregion
    }
}

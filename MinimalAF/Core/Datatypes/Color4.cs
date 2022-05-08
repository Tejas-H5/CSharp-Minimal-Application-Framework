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
        /// ints, 0-255
        /// </summary>
        public static Color4 RGB255(byte r, byte g, byte b) {
            return RGBA255(r, g, b, (byte)255);
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

        public static Color4 HSV(float h, float s, float v) {
            return HSVA(h, s, v, 1f);
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
                MathF.Abs(a.G - b.G) +
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
        public static Color4 AliceBlue => RGB255(240, 248, 255);
        public static Color4 AntiqueWhite => RGB255(250, 235, 215);
        public static Color4 Aqua => RGB255(0, 255, 255);
        public static Color4 Aquamarine => RGB255(127, 255, 212);
        public static Color4 Azure => RGB255(240, 255, 255);
        public static Color4 Beige => RGB255(245, 245, 220);
        public static Color4 Bisque => RGB255(255, 228, 196);
        public static Color4 Black => RGB255(0, 0, 0);
        public static Color4 BlanchedAlmond => RGB255(255, 235, 205);
        public static Color4 Blue => RGB255(0, 0, 255);
        public static Color4 BlueViolet => RGB255(138, 43, 226);
        public static Color4 Brown => RGB255(165, 42, 42);
        public static Color4 BurlyWood => RGB255(222, 184, 135);
        public static Color4 CadetBlue => RGB255(95, 158, 160);
        public static Color4 Chartreuse => RGB255(127, 255, 0);
        public static Color4 Chocolate => RGB255(210, 105, 30);
        public static Color4 Coral => RGB255(255, 127, 80);
        public static Color4 CornflowerBlue => RGB255(100, 149, 237);
        public static Color4 CornSilk => RGB255(255, 248, 220);
        public static Color4 Crimson => RGB255(220, 20, 60);
        public static Color4 Cyan => RGB255(0, 255, 255);
        public static Color4 DarkBlue => RGB255(0, 0, 139);
        public static Color4 DarkCyan => RGB255(0, 139, 139);
        public static Color4 DarkGoldenrod => RGB255(184, 134, 11);
        public static Color4 DarkGray => RGB255(169, 169, 169);
        public static Color4 DarkGreen => RGB255(0, 100, 0);
        public static Color4 DarkGrey => RGB255(169, 169, 169);
        public static Color4 DarkKhaki => RGB255(189, 183, 107);
        public static Color4 DarkMagenta => RGB255(139, 0, 139);
        public static Color4 DarkOliveGreen => RGB255(85, 107, 47);
        public static Color4 DarkOrange => RGB255(255, 140, 0);
        public static Color4 DarkOrchid => RGB255(153, 50, 204);
        public static Color4 DarkRed => RGB255(139, 0, 0);
        public static Color4 DarkSalmon => RGB255(233, 150, 122);
        public static Color4 DarkSeaGreen => RGB255(143, 188, 143);
        public static Color4 DarkSlateBlue => RGB255(72, 61, 139);
        public static Color4 DarkSlateGray => RGB255(47, 79, 79);
        public static Color4 DarkSlateGrey => RGB255(47, 79, 79);
        public static Color4 DarkTurquoise => RGB255(0, 206, 209);
        public static Color4 DarkViolet => RGB255(148, 0, 211);
        public static Color4 DeepPink => RGB255(255, 20, 147);
        public static Color4 DeepSkyBlue => RGB255(0, 191, 255);
        public static Color4 DimGray => RGB255(105, 105, 105);
        public static Color4 DimGrey => RGB255(105, 105, 105);
        public static Color4 DodgerBlue => RGB255(30, 144, 255);
        public static Color4 Firebrick => RGB255(178, 34, 34);
        public static Color4 FloralWhite => RGB255(255, 250, 240);
        public static Color4 ForestGreen => RGB255(34, 139, 34);
        public static Color4 Fuchsia => RGB255(255, 0, 255);
        public static Color4 Gainsboro => RGB255(220, 220, 220);
        public static Color4 GhostWhite => RGB255(248, 248, 255);
        public static Color4 Gold => RGB255(255, 215, 0);
        public static Color4 Goldenrod => RGB255(218, 165, 32);
        public static Color4 Gray => RGB255(128, 128, 128);
        public static Color4 Green => RGB255(0, 128, 0);
        public static Color4 GreenYellow => RGB255(173, 255, 47);
        public static Color4 Grey => RGB255(128, 128, 128);
        public static Color4 Honeydew => RGB255(240, 255, 240);
        public static Color4 HotPink => RGB255(255, 105, 180);
        public static Color4 IndianRed => RGB255(205, 92, 92);
        public static Color4 Indigo => RGB255(75, 0, 130);
        public static Color4 Ivory => RGB255(255, 255, 240);
        public static Color4 Khaki => RGB255(240, 230, 140);
        public static Color4 Lavender => RGB255(230, 230, 250);
        public static Color4 LavenderBlush => RGB255(255, 240, 245);
        public static Color4 LawnGreen => RGB255(124, 252, 0);
        public static Color4 LemonChiffon => RGB255(255, 250, 205);
        public static Color4 LightBlue => RGB255(173, 216, 230);
        public static Color4 LightCoral => RGB255(240, 128, 128);
        public static Color4 LightCyan => RGB255(224, 255, 255);
        public static Color4 LightGoldenrodYellow => RGB255(250, 250, 210);
        public static Color4 LightGray => RGB255(211, 211, 211);
        public static Color4 LightGreen => RGB255(144, 238, 144);
        public static Color4 LightGrey => RGB255(211, 211, 211);
        public static Color4 LightPink => RGB255(255, 182, 193);
        public static Color4 LightSalmon => RGB255(255, 160, 122);
        public static Color4 LightSeaGreen => RGB255(32, 178, 170);
        public static Color4 LightSkyBlue => RGB255(135, 206, 250);
        public static Color4 LightSlateGray => RGB255(119, 136, 153);
        public static Color4 LightSlateGrey => RGB255(119, 136, 153);
        public static Color4 LightSteelBlue => RGB255(176, 196, 222);
        public static Color4 LightYellow => RGB255(255, 255, 224);
        public static Color4 Lime => RGB255(0, 255, 0);
        public static Color4 LimeGreen => RGB255(50, 205, 50);
        public static Color4 Linen => RGB255(250, 240, 230);
        public static Color4 Magenta => RGB255(255, 0, 255);
        public static Color4 Maroon => RGB255(128, 0, 0);
        public static Color4 MediumAquamarine => RGB255(102, 205, 170);
        public static Color4 MediumBlue => RGB255(0, 0, 205);
        public static Color4 MediumOrchid => RGB255(186, 85, 211);
        public static Color4 MediumPurple => RGB255(147, 112, 219);
        public static Color4 MediumSeaGreen => RGB255(60, 179, 113);
        public static Color4 MediumSlateBlue => RGB255(123, 104, 238);
        public static Color4 MediumSpringGreen => RGB255(0, 250, 154);
        public static Color4 MediumTurquoise => RGB255(72, 209, 204);
        public static Color4 MediumVioletRed => RGB255(199, 21, 133);
        public static Color4 MidnightBlue => RGB255(25, 25, 112);
        public static Color4 MintCream => RGB255(245, 255, 250);
        public static Color4 MistyRose => RGB255(255, 228, 225);
        public static Color4 Moccasin => RGB255(255, 228, 181);
        public static Color4 NavajoWhite => RGB255(255, 222, 173);
        public static Color4 Navy => RGB255(0, 0, 128);
        public static Color4 OldLace => RGB255(253, 245, 230);
        public static Color4 Olive => RGB255(128, 128, 0);
        public static Color4 OliveDrab => RGB255(107, 142, 35);
        public static Color4 Orange => RGB255(255, 165, 0);
        public static Color4 OrangeRed => RGB255(255, 69, 0);
        public static Color4 Orchid => RGB255(218, 112, 214);
        public static Color4 PaleGoldenrod => RGB255(238, 232, 170);
        public static Color4 PaleGreen => RGB255(152, 251, 152);
        public static Color4 PaleTurquoise => RGB255(175, 238, 238);
        public static Color4 PaleVioletRed => RGB255(219, 112, 147);
        public static Color4 PapayaWhip => RGB255(255, 239, 213);
        public static Color4 PeachPuff => RGB255(255, 218, 185);
        public static Color4 Peru => RGB255(205, 133, 63);
        public static Color4 Pink => RGB255(255, 192, 203);
        public static Color4 Plum => RGB255(221, 160, 221);
        public static Color4 PowderBlue => RGB255(176, 224, 230);
        public static Color4 Purple => RGB255(128, 0, 128);
        public static Color4 Red => RGB255(255, 0, 0);
        public static Color4 RosyBrown => RGB255(188, 143, 143);
        public static Color4 RoyalBlue => RGB255(65, 105, 225);
        public static Color4 SaddleBrown => RGB255(139, 69, 19);
        public static Color4 Salmon => RGB255(250, 128, 114);
        public static Color4 SandyBrown => RGB255(244, 164, 96);
        public static Color4 SeaGreen => RGB255(46, 139, 87);
        public static Color4 Seashell => RGB255(255, 245, 238);
        public static Color4 Sienna => RGB255(160, 82, 45);
        public static Color4 Silver => RGB255(192, 192, 192);
        public static Color4 SkyBlue => RGB255(135, 206, 235);
        public static Color4 SlateBlue => RGB255(106, 90, 205);
        public static Color4 SlateGray => RGB255(112, 128, 144);
        public static Color4 SlateGrey => RGB255(112, 128, 144);
        public static Color4 Snow => RGB255(255, 250, 250);
        public static Color4 SpringGreen => RGB255(0, 255, 127);
        public static Color4 SteelBlue => RGB255(70, 130, 180);
        public static Color4 Tan => RGB255(210, 180, 140);
        public static Color4 Teal => RGB255(0, 128, 128);
        public static Color4 Thistle => RGB255(216, 191, 216);
        public static Color4 Tomato => RGB255(255, 99, 71);
        public static Color4 Turquoise => RGB255(64, 224, 208);
        public static Color4 Violet => RGB255(238, 130, 238);
        public static Color4 Wheat => RGB255(245, 222, 179);
        public static Color4 White => RGB255(255, 255, 255);
        public static Color4 WhiteSmoke => RGB255(245, 245, 245);
        public static Color4 Yellow => RGB255(255, 255, 0);
        public static Color4 YellowGreen => RGB255(154, 205, 50);

        #endregion
    }
}

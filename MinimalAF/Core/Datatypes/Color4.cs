﻿using System;

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

        #region All named colors from Wikipedia
        public static Color4 AbsoluteZero {
            get {
                return RGB(0f, 0.28f, 0.73f);
            }
        }
        public static Color4 AcidGreen {
            get {
                return RGB(0.69f, 0.75f, 0.1f);
            }
        }
        public static Color4 Aero {
            get {
                return RGB(0.49f, 0.73f, 0.91f);
            }
        }
        public static Color4 AeroBlue {
            get {
                return RGB(0.75f, 0.91f, 0.84f);
            }
        }
        public static Color4 AfricanViolet {
            get {
                return RGB(0.7f, 0.52f, 0.75f);
            }
        }
        public static Color4 AirSuperiorityBlue {
            get {
                return RGB(0.45f, 0.63f, 0.76f);
            }
        }
        public static Color4 Alabaster {
            get {
                return RGB(0.93f, 0.92f, 0.88f);
            }
        }
        public static Color4 AliceBlue {
            get {
                return RGB(0.94f, 0.97f, 1f);
            }
        }
        public static Color4 AlloyOrange {
            get {
                return RGB(0.77f, 0.38f, 0.06f);
            }
        }
        public static Color4 Almond {
            get {
                return RGB(0.94f, 0.87f, 0.8f);
            }
        }
        public static Color4 Amaranth {
            get {
                return RGB(0.9f, 0.17f, 0.31f);
            }
        }
        public static Color4 AmaranthMAndP {
            get {
                return RGB(0.62f, 0.17f, 0.41f);
            }
        }
        public static Color4 AmaranthPink {
            get {
                return RGB(0.95f, 0.61f, 0.73f);
            }
        }
        public static Color4 AmaranthPurple {
            get {
                return RGB(0.67f, 0.15f, 0.31f);
            }
        }
        public static Color4 AmaranthRed {
            get {
                return RGB(0.83f, 0.13f, 0.18f);
            }
        }
        public static Color4 Amazon {
            get {
                return RGB(0.23f, 0.48f, 0.34f);
            }
        }
        public static Color4 Amber {
            get {
                return RGB(1f, 0.75f, 0f);
            }
        }
        public static Color4 AmberSaeEce {
            get {
                return RGB(1f, 0.49f, 0f);
            }
        }
        public static Color4 Amethyst {
            get {
                return RGB(0.6f, 0.4f, 0.8f);
            }
        }
        public static Color4 AndroidGreen {
            get {
                return RGB(0.24f, 0.86f, 0.53f);
            }
        }
        public static Color4 AntiqueBrass {
            get {
                return RGB(0.8f, 0.58f, 0.46f);
            }
        }
        public static Color4 AntiqueBronze {
            get {
                return RGB(0.4f, 0.36f, 0.12f);
            }
        }
        public static Color4 AntiqueFuchsia {
            get {
                return RGB(0.57f, 0.36f, 0.51f);
            }
        }
        public static Color4 AntiqueRuby {
            get {
                return RGB(0.52f, 0.11f, 0.18f);
            }
        }
        public static Color4 AntiqueWhite {
            get {
                return RGB(0.98f, 0.92f, 0.84f);
            }
        }
        public static Color4 AoEnglish {
            get {
                return RGB(0f, 0.5f, 0f);
            }
        }
        public static Color4 AppleGreen {
            get {
                return RGB(0.55f, 0.71f, 0f);
            }
        }
        public static Color4 Apricot {
            get {
                return RGB(0.98f, 0.81f, 0.69f);
            }
        }
        public static Color4 Aqua {
            get {
                return RGB(0f, 1f, 1f);
            }
        }
        public static Color4 Aquamarine {
            get {
                return RGB(0.5f, 1f, 0.83f);
            }
        }
        public static Color4 ArcticLime {
            get {
                return RGB(0.82f, 1f, 0.08f);
            }
        }
        public static Color4 ArmyGreen {
            get {
                return RGB(0.29f, 0.33f, 0.13f);
            }
        }
        public static Color4 Artichoke {
            get {
                return RGB(0.56f, 0.59f, 0.47f);
            }
        }
        public static Color4 ArylideYellow {
            get {
                return RGB(0.91f, 0.84f, 0.42f);
            }
        }
        public static Color4 AshGray {
            get {
                return RGB(0.7f, 0.75f, 0.71f);
            }
        }
        public static Color4 Asparagus {
            get {
                return RGB(0.53f, 0.66f, 0.42f);
            }
        }
        public static Color4 AtomicTangerine {
            get {
                return RGB(1f, 0.6f, 0.4f);
            }
        }
        public static Color4 Auburn {
            get {
                return RGB(0.65f, 0.16f, 0.16f);
            }
        }
        public static Color4 Aureolin {
            get {
                return RGB(0.99f, 0.93f, 0f);
            }
        }
        public static Color4 Avocado {
            get {
                return RGB(0.34f, 0.51f, 0.01f);
            }
        }
        public static Color4 Azure {
            get {
                return RGB(0f, 0.5f, 1f);
            }
        }
        public static Color4 AzureX11WebColor {
            get {
                return RGB(0.94f, 1f, 1f);
            }
        }
        public static Color4 BabyBlue {
            get {
                return RGB(0.54f, 0.81f, 0.94f);
            }
        }
        public static Color4 BabyBlueEyes {
            get {
                return RGB(0.63f, 0.79f, 0.95f);
            }
        }
        public static Color4 BabyPink {
            get {
                return RGB(0.96f, 0.76f, 0.76f);
            }
        }
        public static Color4 BabyPowder {
            get {
                return RGB(1f, 1f, 0.98f);
            }
        }
        public static Color4 BakerMillerPink {
            get {
                return RGB(1f, 0.57f, 0.69f);
            }
        }
        public static Color4 BananaMania {
            get {
                return RGB(0.98f, 0.91f, 0.71f);
            }
        }
        public static Color4 BarbiePink {
            get {
                return RGB(0.85f, 0.09f, 0.52f);
            }
        }
        public static Color4 BarnRed {
            get {
                return RGB(0.49f, 0.04f, 0.01f);
            }
        }
        public static Color4 BattleshipGrey {
            get {
                return RGB(0.52f, 0.52f, 0.51f);
            }
        }
        public static Color4 BeauBlue {
            get {
                return RGB(0.74f, 0.83f, 0.9f);
            }
        }
        public static Color4 Beaver {
            get {
                return RGB(0.62f, 0.51f, 0.44f);
            }
        }
        public static Color4 Beige {
            get {
                return RGB(0.96f, 0.96f, 0.86f);
            }
        }
        public static Color4 BDazzledBlue {
            get {
                return RGB(0.18f, 0.35f, 0.58f);
            }
        }
        public static Color4 BigDipORuby {
            get {
                return RGB(0.61f, 0.15f, 0.26f);
            }
        }
        public static Color4 Bisque {
            get {
                return RGB(1f, 0.89f, 0.77f);
            }
        }
        public static Color4 Bistre {
            get {
                return RGB(0.24f, 0.17f, 0.12f);
            }
        }
        public static Color4 BistreBrown {
            get {
                return RGB(0.59f, 0.44f, 0.09f);
            }
        }
        public static Color4 BitterLemon {
            get {
                return RGB(0.79f, 0.88f, 0.05f);
            }
        }
        public static Color4 BitterLime {
            get {
                return RGB(0.75f, 1f, 0f);
            }
        }
        public static Color4 Bittersweet {
            get {
                return RGB(1f, 0.44f, 0.37f);
            }
        }
        public static Color4 BittersweetShimmer {
            get {
                return RGB(0.75f, 0.31f, 0.32f);
            }
        }
        public static Color4 Black {
            get {
                return RGB(0f, 0f, 0f);
            }
        }
        public static Color4 BlackBean {
            get {
                return RGB(0.24f, 0.05f, 0.01f);
            }
        }
        public static Color4 BlackChocolate {
            get {
                return RGB(0.11f, 0.09f, 0.07f);
            }
        }
        public static Color4 BlackCoffee {
            get {
                return RGB(0.23f, 0.18f, 0.18f);
            }
        }
        public static Color4 BlackCoral {
            get {
                return RGB(0.33f, 0.38f, 0.44f);
            }
        }
        public static Color4 BlackOlive {
            get {
                return RGB(0.23f, 0.24f, 0.21f);
            }
        }
        public static Color4 BlackShadows {
            get {
                return RGB(0.75f, 0.69f, 0.7f);
            }
        }
        public static Color4 BlanchedAlmond {
            get {
                return RGB(1f, 0.92f, 0.8f);
            }
        }
        public static Color4 BlastOffBronze {
            get {
                return RGB(0.65f, 0.44f, 0.39f);
            }
        }
        public static Color4 BleuDeFrance {
            get {
                return RGB(0.19f, 0.55f, 0.91f);
            }
        }
        public static Color4 BlizzardBlue {
            get {
                return RGB(0.67f, 0.9f, 0.93f);
            }
        }
        public static Color4 Blond {
            get {
                return RGB(0.98f, 0.94f, 0.75f);
            }
        }
        public static Color4 BloodRed {
            get {
                return RGB(0.4f, 0f, 0f);
            }
        }
        public static Color4 Blue {
            get {
                return RGB(0f, 0f, 1f);
            }
        }
        public static Color4 BlueCrayola {
            get {
                return RGB(0.12f, 0.46f, 1f);
            }
        }
        public static Color4 BlueMunsell {
            get {
                return RGB(0f, 0.58f, 0.69f);
            }
        }
        public static Color4 BlueNcs {
            get {
                return RGB(0f, 0.53f, 0.74f);
            }
        }
        public static Color4 BluePantone {
            get {
                return RGB(0f, 0.09f, 0.66f);
            }
        }
        public static Color4 BluePigment {
            get {
                return RGB(0.2f, 0.2f, 0.6f);
            }
        }
        public static Color4 BlueRyb {
            get {
                return RGB(0.01f, 0.28f, 1f);
            }
        }
        public static Color4 BlueBell {
            get {
                return RGB(0.64f, 0.64f, 0.82f);
            }
        }
        public static Color4 BlueGray {
            get {
                return RGB(0.4f, 0.6f, 0.8f);
            }
        }
        public static Color4 BlueGreen {
            get {
                return RGB(0.05f, 0.6f, 0.73f);
            }
        }
        public static Color4 BlueGreenColorWheel {
            get {
                return RGB(0.02f, 0.31f, 0.25f);
            }
        }
        public static Color4 BlueJeans {
            get {
                return RGB(0.36f, 0.68f, 0.93f);
            }
        }
        public static Color4 BlueSapphire {
            get {
                return RGB(0.07f, 0.38f, 0.5f);
            }
        }
        public static Color4 BlueViolet {
            get {
                return RGB(0.54f, 0.17f, 0.89f);
            }
        }
        public static Color4 BlueVioletCrayola {
            get {
                return RGB(0.45f, 0.4f, 0.74f);
            }
        }
        public static Color4 BlueVioletColorWheel {
            get {
                return RGB(0.3f, 0.1f, 0.5f);
            }
        }
        public static Color4 BlueYonder {
            get {
                return RGB(0.31f, 0.45f, 0.65f);
            }
        }
        public static Color4 Bluetiful {
            get {
                return RGB(0.24f, 0.41f, 0.91f);
            }
        }
        public static Color4 Blush {
            get {
                return RGB(0.87f, 0.36f, 0.51f);
            }
        }
        public static Color4 Bole {
            get {
                return RGB(0.47f, 0.27f, 0.23f);
            }
        }
        public static Color4 Bone {
            get {
                return RGB(0.89f, 0.85f, 0.79f);
            }
        }
        public static Color4 BottleGreen {
            get {
                return RGB(0f, 0.42f, 0.31f);
            }
        }
        public static Color4 Brandy {
            get {
                return RGB(0.53f, 0.25f, 0.25f);
            }
        }
        public static Color4 BrickRed {
            get {
                return RGB(0.8f, 0.25f, 0.33f);
            }
        }
        public static Color4 BrightGreen {
            get {
                return RGB(0.4f, 1f, 0f);
            }
        }
        public static Color4 BrightLilac {
            get {
                return RGB(0.85f, 0.57f, 0.94f);
            }
        }
        public static Color4 BrightMaroon {
            get {
                return RGB(0.76f, 0.13f, 0.28f);
            }
        }
        public static Color4 BrightNavyBlue {
            get {
                return RGB(0.1f, 0.45f, 0.82f);
            }
        }
        public static Color4 BrightYellowCrayola {
            get {
                return RGB(1f, 0.67f, 0.11f);
            }
        }
        public static Color4 BrilliantRose {
            get {
                return RGB(1f, 0.33f, 0.64f);
            }
        }
        public static Color4 BrinkPink {
            get {
                return RGB(0.98f, 0.38f, 0.5f);
            }
        }
        public static Color4 BritishRacingGreen {
            get {
                return RGB(0f, 0.26f, 0.15f);
            }
        }
        public static Color4 Bronze {
            get {
                return RGB(0.8f, 0.5f, 0.2f);
            }
        }
        public static Color4 Brown {
            get {
                return RGB(0.53f, 0.33f, 0.04f);
            }
        }
        public static Color4 BrownSugar {
            get {
                return RGB(0.69f, 0.43f, 0.3f);
            }
        }
        public static Color4 BrunswickGreen {
            get {
                return RGB(0.11f, 0.3f, 0.24f);
            }
        }
        public static Color4 BudGreen {
            get {
                return RGB(0.48f, 0.71f, 0.38f);
            }
        }
        public static Color4 Buff {
            get {
                return RGB(1f, 0.78f, 0.5f);
            }
        }
        public static Color4 Burgundy {
            get {
                return RGB(0.5f, 0f, 0.13f);
            }
        }
        public static Color4 Burlywood {
            get {
                return RGB(0.87f, 0.72f, 0.53f);
            }
        }
        public static Color4 BurnishedBrown {
            get {
                return RGB(0.63f, 0.48f, 0.45f);
            }
        }
        public static Color4 BurntOrange {
            get {
                return RGB(0.8f, 0.33f, 0f);
            }
        }
        public static Color4 BurntSienna {
            get {
                return RGB(0.91f, 0.45f, 0.32f);
            }
        }
        public static Color4 BurntUmber {
            get {
                return RGB(0.54f, 0.2f, 0.14f);
            }
        }
        public static Color4 Byzantine {
            get {
                return RGB(0.74f, 0.2f, 0.64f);
            }
        }
        public static Color4 Byzantium {
            get {
                return RGB(0.44f, 0.16f, 0.39f);
            }
        }
        public static Color4 Cadet {
            get {
                return RGB(0.33f, 0.41f, 0.45f);
            }
        }
        public static Color4 CadetBlue {
            get {
                return RGB(0.37f, 0.62f, 0.63f);
            }
        }
        public static Color4 CadetBlueCrayola {
            get {
                return RGB(0.66f, 0.7f, 0.76f);
            }
        }
        public static Color4 CadetGrey {
            get {
                return RGB(0.57f, 0.64f, 0.69f);
            }
        }
        public static Color4 CadmiumGreen {
            get {
                return RGB(0f, 0.42f, 0.24f);
            }
        }
        public static Color4 CadmiumOrange {
            get {
                return RGB(0.93f, 0.53f, 0.18f);
            }
        }
        public static Color4 CadmiumRed {
            get {
                return RGB(0.89f, 0f, 0.13f);
            }
        }
        public static Color4 CadmiumYellow {
            get {
                return RGB(1f, 0.96f, 0f);
            }
        }
        public static Color4 CafeAuLait {
            get {
                return RGB(0.65f, 0.48f, 0.36f);
            }
        }
        public static Color4 CafeNoir {
            get {
                return RGB(0.29f, 0.21f, 0.13f);
            }
        }
        public static Color4 CambridgeBlue {
            get {
                return RGB(0.64f, 0.76f, 0.68f);
            }
        }
        public static Color4 Camel {
            get {
                return RGB(0.76f, 0.6f, 0.42f);
            }
        }
        public static Color4 CameoPink {
            get {
                return RGB(0.94f, 0.73f, 0.8f);
            }
        }
        public static Color4 Canary {
            get {
                return RGB(1f, 1f, 0.6f);
            }
        }
        public static Color4 CanaryYellow {
            get {
                return RGB(1f, 0.94f, 0f);
            }
        }
        public static Color4 CandyAppleRed {
            get {
                return RGB(1f, 0.03f, 0f);
            }
        }
        public static Color4 CandyPink {
            get {
                return RGB(0.89f, 0.44f, 0.48f);
            }
        }
        public static Color4 Capri {
            get {
                return RGB(0f, 0.75f, 1f);
            }
        }
        public static Color4 CaputMortuum {
            get {
                return RGB(0.35f, 0.15f, 0.13f);
            }
        }
        public static Color4 Cardinal {
            get {
                return RGB(0.77f, 0.12f, 0.23f);
            }
        }
        public static Color4 CaribbeanGreen {
            get {
                return RGB(0f, 0.8f, 0.6f);
            }
        }
        public static Color4 Carmine {
            get {
                return RGB(0.59f, 0f, 0.09f);
            }
        }
        public static Color4 CarmineMAndP {
            get {
                return RGB(0.84f, 0f, 0.25f);
            }
        }
        public static Color4 CarnationPink {
            get {
                return RGB(1f, 0.65f, 0.79f);
            }
        }
        public static Color4 Carnelian {
            get {
                return RGB(0.7f, 0.11f, 0.11f);
            }
        }
        public static Color4 CarolinaBlue {
            get {
                return RGB(0.34f, 0.63f, 0.83f);
            }
        }
        public static Color4 CarrotOrange {
            get {
                return RGB(0.93f, 0.57f, 0.13f);
            }
        }
        public static Color4 CastletonGreen {
            get {
                return RGB(0f, 0.34f, 0.25f);
            }
        }
        public static Color4 Catawba {
            get {
                return RGB(0.44f, 0.21f, 0.26f);
            }
        }
        public static Color4 CedarChest {
            get {
                return RGB(0.79f, 0.35f, 0.29f);
            }
        }
        public static Color4 Celadon {
            get {
                return RGB(0.67f, 0.88f, 0.69f);
            }
        }
        public static Color4 CeladonBlue {
            get {
                return RGB(0f, 0.48f, 0.65f);
            }
        }
        public static Color4 CeladonGreen {
            get {
                return RGB(0.18f, 0.52f, 0.49f);
            }
        }
        public static Color4 Celeste {
            get {
                return RGB(0.7f, 1f, 1f);
            }
        }
        public static Color4 CelticBlue {
            get {
                return RGB(0.14f, 0.42f, 0.81f);
            }
        }
        public static Color4 Cerise {
            get {
                return RGB(0.87f, 0.19f, 0.39f);
            }
        }
        public static Color4 Cerulean {
            get {
                return RGB(0f, 0.48f, 0.65f);
            }
        }
        public static Color4 CeruleanBlue {
            get {
                return RGB(0.16f, 0.32f, 0.75f);
            }
        }
        public static Color4 CeruleanFrost {
            get {
                return RGB(0.43f, 0.61f, 0.76f);
            }
        }
        public static Color4 CeruleanCrayola {
            get {
                return RGB(0.11f, 0.67f, 0.84f);
            }
        }
        public static Color4 CgBlue {
            get {
                return RGB(0f, 0.48f, 0.65f);
            }
        }
        public static Color4 CgRed {
            get {
                return RGB(0.88f, 0.24f, 0.19f);
            }
        }
        public static Color4 Champagne {
            get {
                return RGB(0.97f, 0.91f, 0.81f);
            }
        }
        public static Color4 ChampagnePink {
            get {
                return RGB(0.95f, 0.87f, 0.81f);
            }
        }
        public static Color4 Charcoal {
            get {
                return RGB(0.21f, 0.27f, 0.31f);
            }
        }
        public static Color4 CharlestonGreen {
            get {
                return RGB(0.14f, 0.17f, 0.17f);
            }
        }
        public static Color4 CharmPink {
            get {
                return RGB(0.9f, 0.56f, 0.67f);
            }
        }
        public static Color4 ChartreuseTraditional {
            get {
                return RGB(0.87f, 1f, 0f);
            }
        }
        public static Color4 ChartreuseWeb {
            get {
                return RGB(0.5f, 1f, 0f);
            }
        }
        public static Color4 CherryBlossomPink {
            get {
                return RGB(1f, 0.72f, 0.77f);
            }
        }
        public static Color4 Chestnut {
            get {
                return RGB(0.58f, 0.27f, 0.21f);
            }
        }
        public static Color4 ChiliRed {
            get {
                return RGB(0.89f, 0.24f, 0.16f);
            }
        }
        public static Color4 ChinaPink {
            get {
                return RGB(0.87f, 0.44f, 0.63f);
            }
        }
        public static Color4 ChinaRose {
            get {
                return RGB(0.66f, 0.32f, 0.43f);
            }
        }
        public static Color4 ChineseRed {
            get {
                return RGB(0.67f, 0.22f, 0.12f);
            }
        }
        public static Color4 ChineseViolet {
            get {
                return RGB(0.52f, 0.38f, 0.53f);
            }
        }
        public static Color4 ChineseYellow {
            get {
                return RGB(1f, 0.7f, 0f);
            }
        }
        public static Color4 ChocolateTraditional {
            get {
                return RGB(0.48f, 0.25f, 0f);
            }
        }
        public static Color4 ChocolateWeb {
            get {
                return RGB(0.82f, 0.41f, 0.12f);
            }
        }
        public static Color4 ChocolateCosmos {
            get {
                return RGB(0.35f, 0.07f, 0.1f);
            }
        }
        public static Color4 ChromeYellow {
            get {
                return RGB(1f, 0.65f, 0f);
            }
        }
        public static Color4 Cinereous {
            get {
                return RGB(0.6f, 0.51f, 0.48f);
            }
        }
        public static Color4 Cinnabar {
            get {
                return RGB(0.89f, 0.26f, 0.2f);
            }
        }
        public static Color4 CinnamonSatin {
            get {
                return RGB(0.8f, 0.38f, 0.49f);
            }
        }
        public static Color4 Citrine {
            get {
                return RGB(0.89f, 0.82f, 0.04f);
            }
        }
        public static Color4 Citron {
            get {
                return RGB(0.62f, 0.66f, 0.12f);
            }
        }
        public static Color4 Claret {
            get {
                return RGB(0.5f, 0.09f, 0.2f);
            }
        }
        public static Color4 CobaltBlue {
            get {
                return RGB(0f, 0.28f, 0.67f);
            }
        }
        public static Color4 CocoaBrown {
            get {
                return RGB(0.82f, 0.41f, 0.12f);
            }
        }
        public static Color4 Coffee {
            get {
                return RGB(0.44f, 0.31f, 0.22f);
            }
        }
        public static Color4 ColumbiaBlue {
            get {
                return RGB(0.73f, 0.85f, 0.92f);
            }
        }
        public static Color4 CongoPink {
            get {
                return RGB(0.97f, 0.51f, 0.47f);
            }
        }
        public static Color4 CoolGrey {
            get {
                return RGB(0.55f, 0.57f, 0.67f);
            }
        }
        public static Color4 Copper {
            get {
                return RGB(0.72f, 0.45f, 0.2f);
            }
        }
        public static Color4 CopperCrayola {
            get {
                return RGB(0.85f, 0.54f, 0.4f);
            }
        }
        public static Color4 CopperPenny {
            get {
                return RGB(0.68f, 0.44f, 0.41f);
            }
        }
        public static Color4 CopperRed {
            get {
                return RGB(0.8f, 0.43f, 0.32f);
            }
        }
        public static Color4 CopperRose {
            get {
                return RGB(0.6f, 0.4f, 0.4f);
            }
        }
        public static Color4 Coquelicot {
            get {
                return RGB(1f, 0.22f, 0f);
            }
        }
        public static Color4 Coral {
            get {
                return RGB(1f, 0.5f, 0.31f);
            }
        }
        public static Color4 CoralPink {
            get {
                return RGB(0.97f, 0.51f, 0.47f);
            }
        }
        public static Color4 Cordovan {
            get {
                return RGB(0.54f, 0.25f, 0.27f);
            }
        }
        public static Color4 Corn {
            get {
                return RGB(0.98f, 0.93f, 0.36f);
            }
        }
        public static Color4 CornellRed {
            get {
                return RGB(0.7f, 0.11f, 0.11f);
            }
        }
        public static Color4 CornflowerBlue {
            get {
                return RGB(0.39f, 0.58f, 0.93f);
            }
        }
        public static Color4 Cornsilk {
            get {
                return RGB(1f, 0.97f, 0.86f);
            }
        }
        public static Color4 CosmicCobalt {
            get {
                return RGB(0.18f, 0.18f, 0.53f);
            }
        }
        public static Color4 CosmicLatte {
            get {
                return RGB(1f, 0.97f, 0.91f);
            }
        }
        public static Color4 CoyoteBrown {
            get {
                return RGB(0.51f, 0.38f, 0.24f);
            }
        }
        public static Color4 CottonCandy {
            get {
                return RGB(1f, 0.74f, 0.85f);
            }
        }
        public static Color4 Cream {
            get {
                return RGB(1f, 0.99f, 0.82f);
            }
        }
        public static Color4 Crimson {
            get {
                return RGB(0.86f, 0.08f, 0.24f);
            }
        }
        public static Color4 CrimsonUa {
            get {
                return RGB(0.62f, 0.11f, 0.2f);
            }
        }
        public static Color4 Crystal {
            get {
                return RGB(0.65f, 0.85f, 0.87f);
            }
        }
        public static Color4 Cultured {
            get {
                return RGB(0.96f, 0.96f, 0.96f);
            }
        }
        public static Color4 Cyan {
            get {
                return RGB(0f, 1f, 1f);
            }
        }
        public static Color4 CyanProcess {
            get {
                return RGB(0f, 0.72f, 0.92f);
            }
        }
        public static Color4 CyberGrape {
            get {
                return RGB(0.35f, 0.26f, 0.49f);
            }
        }
        public static Color4 CyberYellow {
            get {
                return RGB(1f, 0.83f, 0f);
            }
        }
        public static Color4 Cyclamen {
            get {
                return RGB(0.96f, 0.44f, 0.63f);
            }
        }
        public static Color4 DarkBlueGray {
            get {
                return RGB(0.4f, 0.4f, 0.6f);
            }
        }
        public static Color4 DarkBrown {
            get {
                return RGB(0.4f, 0.26f, 0.13f);
            }
        }
        public static Color4 DarkByzantium {
            get {
                return RGB(0.36f, 0.22f, 0.33f);
            }
        }
        public static Color4 DarkCornflowerBlue {
            get {
                return RGB(0.15f, 0.26f, 0.55f);
            }
        }
        public static Color4 DarkCyan {
            get {
                return RGB(0f, 0.55f, 0.55f);
            }
        }
        public static Color4 DarkElectricBlue {
            get {
                return RGB(0.33f, 0.41f, 0.47f);
            }
        }
        public static Color4 DarkGoldenrod {
            get {
                return RGB(0.72f, 0.53f, 0.04f);
            }
        }
        public static Color4 DarkGreen {
            get {
                return RGB(0f, 0.2f, 0.13f);
            }
        }
        public static Color4 DarkGreenX11 {
            get {
                return RGB(0f, 0.39f, 0f);
            }
        }
        public static Color4 DarkJungleGreen {
            get {
                return RGB(0.1f, 0.14f, 0.13f);
            }
        }
        public static Color4 DarkKhaki {
            get {
                return RGB(0.74f, 0.72f, 0.42f);
            }
        }
        public static Color4 DarkLava {
            get {
                return RGB(0.28f, 0.24f, 0.2f);
            }
        }
        public static Color4 DarkLiver {
            get {
                return RGB(0.33f, 0.29f, 0.31f);
            }
        }
        public static Color4 DarkLiverHorses {
            get {
                return RGB(0.33f, 0.24f, 0.22f);
            }
        }
        public static Color4 DarkMagenta {
            get {
                return RGB(0.55f, 0f, 0.55f);
            }
        }
        public static Color4 DarkMossGreen {
            get {
                return RGB(0.29f, 0.36f, 0.14f);
            }
        }
        public static Color4 DarkOliveGreen {
            get {
                return RGB(0.33f, 0.42f, 0.18f);
            }
        }
        public static Color4 DarkOrange {
            get {
                return RGB(1f, 0.55f, 0f);
            }
        }
        public static Color4 DarkOrchid {
            get {
                return RGB(0.6f, 0.2f, 0.8f);
            }
        }
        public static Color4 DarkPastelGreen {
            get {
                return RGB(0.01f, 0.75f, 0.24f);
            }
        }
        public static Color4 DarkPurple {
            get {
                return RGB(0.19f, 0.1f, 0.2f);
            }
        }
        public static Color4 DarkRed {
            get {
                return RGB(0.55f, 0f, 0f);
            }
        }
        public static Color4 DarkSalmon {
            get {
                return RGB(0.91f, 0.59f, 0.48f);
            }
        }
        public static Color4 DarkSeaGreen {
            get {
                return RGB(0.56f, 0.74f, 0.56f);
            }
        }
        public static Color4 DarkSienna {
            get {
                return RGB(0.24f, 0.08f, 0.08f);
            }
        }
        public static Color4 DarkSkyBlue {
            get {
                return RGB(0.55f, 0.75f, 0.84f);
            }
        }
        public static Color4 DarkSlateBlue {
            get {
                return RGB(0.28f, 0.24f, 0.55f);
            }
        }
        public static Color4 DarkSlateGray {
            get {
                return RGB(0.18f, 0.31f, 0.31f);
            }
        }
        public static Color4 DarkSpringGreen {
            get {
                return RGB(0.09f, 0.45f, 0.27f);
            }
        }
        public static Color4 DarkTurquoise {
            get {
                return RGB(0f, 0.81f, 0.82f);
            }
        }
        public static Color4 DarkViolet {
            get {
                return RGB(0.58f, 0f, 0.83f);
            }
        }
        public static Color4 DartmouthGreen {
            get {
                return RGB(0f, 0.44f, 0.24f);
            }
        }
        public static Color4 DavySGrey {
            get {
                return RGB(0.33f, 0.33f, 0.33f);
            }
        }
        public static Color4 DeepCerise {
            get {
                return RGB(0.85f, 0.2f, 0.53f);
            }
        }
        public static Color4 DeepChampagne {
            get {
                return RGB(0.98f, 0.84f, 0.65f);
            }
        }
        public static Color4 DeepChestnut {
            get {
                return RGB(0.73f, 0.31f, 0.28f);
            }
        }
        public static Color4 DeepJungleGreen {
            get {
                return RGB(0f, 0.29f, 0.29f);
            }
        }
        public static Color4 DeepPink {
            get {
                return RGB(1f, 0.08f, 0.58f);
            }
        }
        public static Color4 DeepSaffron {
            get {
                return RGB(1f, 0.6f, 0.2f);
            }
        }
        public static Color4 DeepSkyBlue {
            get {
                return RGB(0f, 0.75f, 1f);
            }
        }
        public static Color4 DeepSpaceSparkle {
            get {
                return RGB(0.29f, 0.39f, 0.42f);
            }
        }
        public static Color4 DeepTaupe {
            get {
                return RGB(0.49f, 0.37f, 0.38f);
            }
        }
        public static Color4 Denim {
            get {
                return RGB(0.08f, 0.38f, 0.74f);
            }
        }
        public static Color4 DenimBlue {
            get {
                return RGB(0.13f, 0.26f, 0.71f);
            }
        }
        public static Color4 Desert {
            get {
                return RGB(0.76f, 0.6f, 0.42f);
            }
        }
        public static Color4 DesertSand {
            get {
                return RGB(0.93f, 0.79f, 0.69f);
            }
        }
        public static Color4 DimGray {
            get {
                return RGB(0.41f, 0.41f, 0.41f);
            }
        }
        public static Color4 DodgerBlue {
            get {
                return RGB(0.12f, 0.56f, 1f);
            }
        }
        public static Color4 DogwoodRose {
            get {
                return RGB(0.84f, 0.09f, 0.41f);
            }
        }
        public static Color4 Drab {
            get {
                return RGB(0.59f, 0.44f, 0.09f);
            }
        }
        public static Color4 DukeBlue {
            get {
                return RGB(0f, 0f, 0.61f);
            }
        }
        public static Color4 DutchWhite {
            get {
                return RGB(0.94f, 0.87f, 0.73f);
            }
        }
        public static Color4 EarthYellow {
            get {
                return RGB(0.88f, 0.66f, 0.37f);
            }
        }
        public static Color4 Ebony {
            get {
                return RGB(0.33f, 0.36f, 0.31f);
            }
        }
        public static Color4 Ecru {
            get {
                return RGB(0.76f, 0.7f, 0.5f);
            }
        }
        public static Color4 EerieBlack {
            get {
                return RGB(0.11f, 0.11f, 0.11f);
            }
        }
        public static Color4 Eggplant {
            get {
                return RGB(0.38f, 0.25f, 0.32f);
            }
        }
        public static Color4 Eggshell {
            get {
                return RGB(0.94f, 0.92f, 0.84f);
            }
        }
        public static Color4 EgyptianBlue {
            get {
                return RGB(0.06f, 0.2f, 0.65f);
            }
        }
        public static Color4 Eigengrau {
            get {
                return RGB(0.09f, 0.09f, 0.11f);
            }
        }
        public static Color4 ElectricBlue {
            get {
                return RGB(0.49f, 0.98f, 1f);
            }
        }
        public static Color4 ElectricGreen {
            get {
                return RGB(0f, 1f, 0f);
            }
        }
        public static Color4 ElectricIndigo {
            get {
                return RGB(0.44f, 0f, 1f);
            }
        }
        public static Color4 ElectricLime {
            get {
                return RGB(0.8f, 1f, 0f);
            }
        }
        public static Color4 ElectricPurple {
            get {
                return RGB(0.75f, 0f, 1f);
            }
        }
        public static Color4 ElectricViolet {
            get {
                return RGB(0.56f, 0f, 1f);
            }
        }
        public static Color4 Emerald {
            get {
                return RGB(0.31f, 0.78f, 0.47f);
            }
        }
        public static Color4 Eminence {
            get {
                return RGB(0.42f, 0.19f, 0.51f);
            }
        }
        public static Color4 EnglishGreen {
            get {
                return RGB(0.11f, 0.3f, 0.24f);
            }
        }
        public static Color4 EnglishLavender {
            get {
                return RGB(0.71f, 0.51f, 0.58f);
            }
        }
        public static Color4 EnglishRed {
            get {
                return RGB(0.67f, 0.29f, 0.32f);
            }
        }
        public static Color4 EnglishVermillion {
            get {
                return RGB(0.8f, 0.28f, 0.29f);
            }
        }
        public static Color4 EnglishViolet {
            get {
                return RGB(0.34f, 0.24f, 0.36f);
            }
        }
        public static Color4 Erin {
            get {
                return RGB(0f, 1f, 0.25f);
            }
        }
        public static Color4 EtonBlue {
            get {
                return RGB(0.59f, 0.78f, 0.64f);
            }
        }
        public static Color4 Fallow {
            get {
                return RGB(0.76f, 0.6f, 0.42f);
            }
        }
        public static Color4 FaluRed {
            get {
                return RGB(0.5f, 0.09f, 0.09f);
            }
        }
        public static Color4 Fandango {
            get {
                return RGB(0.71f, 0.2f, 0.54f);
            }
        }
        public static Color4 FandangoPink {
            get {
                return RGB(0.87f, 0.32f, 0.52f);
            }
        }
        public static Color4 FashionFuchsia {
            get {
                return RGB(0.96f, 0f, 0.63f);
            }
        }
        public static Color4 Fawn {
            get {
                return RGB(0.9f, 0.67f, 0.44f);
            }
        }
        public static Color4 Feldgrau {
            get {
                return RGB(0.3f, 0.36f, 0.33f);
            }
        }
        public static Color4 FernGreen {
            get {
                return RGB(0.31f, 0.47f, 0.26f);
            }
        }
        public static Color4 FieldDrab {
            get {
                return RGB(0.42f, 0.33f, 0.12f);
            }
        }
        public static Color4 FieryRose {
            get {
                return RGB(1f, 0.33f, 0.44f);
            }
        }
        public static Color4 Firebrick {
            get {
                return RGB(0.7f, 0.13f, 0.13f);
            }
        }
        public static Color4 FireEngineRed {
            get {
                return RGB(0.81f, 0.13f, 0.16f);
            }
        }
        public static Color4 FireOpal {
            get {
                return RGB(0.91f, 0.36f, 0.29f);
            }
        }
        public static Color4 Flame {
            get {
                return RGB(0.89f, 0.35f, 0.13f);
            }
        }
        public static Color4 Flax {
            get {
                return RGB(0.93f, 0.86f, 0.51f);
            }
        }
        public static Color4 Flirt {
            get {
                return RGB(0.64f, 0f, 0.43f);
            }
        }
        public static Color4 FloralWhite {
            get {
                return RGB(1f, 0.98f, 0.94f);
            }
        }
        public static Color4 FluorescentBlue {
            get {
                return RGB(0.08f, 0.96f, 0.93f);
            }
        }
        public static Color4 ForestGreenCrayola {
            get {
                return RGB(0.37f, 0.65f, 0.47f);
            }
        }
        public static Color4 ForestGreenTraditional {
            get {
                return RGB(0f, 0.27f, 0.13f);
            }
        }
        public static Color4 ForestGreenWeb {
            get {
                return RGB(0.13f, 0.55f, 0.13f);
            }
        }
        public static Color4 FrenchBeige {
            get {
                return RGB(0.65f, 0.48f, 0.36f);
            }
        }
        public static Color4 FrenchBistre {
            get {
                return RGB(0.52f, 0.43f, 0.3f);
            }
        }
        public static Color4 FrenchBlue {
            get {
                return RGB(0f, 0.45f, 0.73f);
            }
        }
        public static Color4 FrenchFuchsia {
            get {
                return RGB(0.99f, 0.25f, 0.57f);
            }
        }
        public static Color4 FrenchLilac {
            get {
                return RGB(0.53f, 0.38f, 0.56f);
            }
        }
        public static Color4 FrenchLime {
            get {
                return RGB(0.62f, 0.99f, 0.22f);
            }
        }
        public static Color4 FrenchMauve {
            get {
                return RGB(0.83f, 0.45f, 0.83f);
            }
        }
        public static Color4 FrenchPink {
            get {
                return RGB(0.99f, 0.42f, 0.62f);
            }
        }
        public static Color4 FrenchRaspberry {
            get {
                return RGB(0.78f, 0.17f, 0.28f);
            }
        }
        public static Color4 FrenchRose {
            get {
                return RGB(0.96f, 0.29f, 0.54f);
            }
        }
        public static Color4 FrenchSkyBlue {
            get {
                return RGB(0.47f, 0.71f, 1f);
            }
        }
        public static Color4 FrenchViolet {
            get {
                return RGB(0.53f, 0.02f, 0.81f);
            }
        }
        public static Color4 Frostbite {
            get {
                return RGB(0.91f, 0.21f, 0.65f);
            }
        }
        public static Color4 Fuchsia {
            get {
                return RGB(1f, 0f, 1f);
            }
        }
        public static Color4 FuchsiaCrayola {
            get {
                return RGB(0.76f, 0.33f, 0.76f);
            }
        }
        public static Color4 FuchsiaPurple {
            get {
                return RGB(0.8f, 0.22f, 0.48f);
            }
        }
        public static Color4 FuchsiaRose {
            get {
                return RGB(0.78f, 0.26f, 0.46f);
            }
        }
        public static Color4 Fulvous {
            get {
                return RGB(0.89f, 0.52f, 0f);
            }
        }
        public static Color4 FuzzyWuzzy {
            get {
                return RGB(0.53f, 0.26f, 0.12f);
            }
        }
        public static Color4 Gainsboro {
            get {
                return RGB(0.86f, 0.86f, 0.86f);
            }
        }
        public static Color4 Gamboge {
            get {
                return RGB(0.89f, 0.61f, 0.06f);
            }
        }
        public static Color4 GenericViridian {
            get {
                return RGB(0f, 0.5f, 0.4f);
            }
        }
        public static Color4 GhostWhite {
            get {
                return RGB(0.97f, 0.97f, 1f);
            }
        }
        public static Color4 Glaucous {
            get {
                return RGB(0.38f, 0.51f, 0.71f);
            }
        }
        public static Color4 GlossyGrape {
            get {
                return RGB(0.67f, 0.57f, 0.7f);
            }
        }
        public static Color4 GoGreen {
            get {
                return RGB(0f, 0.67f, 0.4f);
            }
        }
        public static Color4 Gold {
            get {
                return RGB(0.65f, 0.49f, 0f);
            }
        }
        public static Color4 GoldMetallic {
            get {
                return RGB(0.83f, 0.69f, 0.22f);
            }
        }
        public static Color4 GoldWebGolden {
            get {
                return RGB(1f, 0.84f, 0f);
            }
        }
        public static Color4 GoldCrayola {
            get {
                return RGB(0.9f, 0.75f, 0.54f);
            }
        }
        public static Color4 GoldFusion {
            get {
                return RGB(0.52f, 0.46f, 0.31f);
            }
        }
        public static Color4 GoldenBrown {
            get {
                return RGB(0.6f, 0.4f, 0.08f);
            }
        }
        public static Color4 GoldenPoppy {
            get {
                return RGB(0.99f, 0.76f, 0f);
            }
        }
        public static Color4 GoldenYellow {
            get {
                return RGB(1f, 0.87f, 0f);
            }
        }
        public static Color4 Goldenrod {
            get {
                return RGB(0.85f, 0.65f, 0.13f);
            }
        }
        public static Color4 GothamGreen {
            get {
                return RGB(0f, 0.34f, 0.25f);
            }
        }
        public static Color4 GraniteGray {
            get {
                return RGB(0.4f, 0.4f, 0.4f);
            }
        }
        public static Color4 GrannySmithApple {
            get {
                return RGB(0.66f, 0.89f, 0.63f);
            }
        }
        public static Color4 GrayWeb {
            get {
                return RGB(0.5f, 0.5f, 0.5f);
            }
        }
        public static Color4 GrayX11Gray {
            get {
                return RGB(0.75f, 0.75f, 0.75f);
            }
        }
        public static Color4 Green {
            get {
                return RGB(0f, 1f, 0f);
            }
        }
        public static Color4 GreenCrayola {
            get {
                return RGB(0.11f, 0.67f, 0.47f);
            }
        }
        public static Color4 GreenWeb {
            get {
                return RGB(0f, 0.5f, 0f);
            }
        }
        public static Color4 GreenMunsell {
            get {
                return RGB(0f, 0.66f, 0.47f);
            }
        }
        public static Color4 GreenNcs {
            get {
                return RGB(0f, 0.62f, 0.42f);
            }
        }
        public static Color4 GreenPantone {
            get {
                return RGB(0f, 0.68f, 0.26f);
            }
        }
        public static Color4 GreenPigment {
            get {
                return RGB(0f, 0.65f, 0.31f);
            }
        }
        public static Color4 GreenRyb {
            get {
                return RGB(0.4f, 0.69f, 0.2f);
            }
        }
        public static Color4 GreenBlue {
            get {
                return RGB(0.07f, 0.39f, 0.71f);
            }
        }
        public static Color4 GreenBlueCrayola {
            get {
                return RGB(0.16f, 0.53f, 0.78f);
            }
        }
        public static Color4 GreenCyan {
            get {
                return RGB(0f, 0.6f, 0.4f);
            }
        }
        public static Color4 GreenLizard {
            get {
                return RGB(0.65f, 0.96f, 0.2f);
            }
        }
        public static Color4 GreenSheen {
            get {
                return RGB(0.43f, 0.68f, 0.63f);
            }
        }
        public static Color4 GreenYellow {
            get {
                return RGB(0.68f, 1f, 0.18f);
            }
        }
        public static Color4 GreenYellowCrayola {
            get {
                return RGB(0.94f, 0.91f, 0.57f);
            }
        }
        public static Color4 Grullo {
            get {
                return RGB(0.66f, 0.6f, 0.53f);
            }
        }
        public static Color4 Gunmetal {
            get {
                return RGB(0.16f, 0.2f, 0.22f);
            }
        }
        public static Color4 HanBlue {
            get {
                return RGB(0.27f, 0.42f, 0.81f);
            }
        }
        public static Color4 HanPurple {
            get {
                return RGB(0.32f, 0.09f, 0.98f);
            }
        }
        public static Color4 HansaYellow {
            get {
                return RGB(0.91f, 0.84f, 0.42f);
            }
        }
        public static Color4 Harlequin {
            get {
                return RGB(0.25f, 1f, 0f);
            }
        }
        public static Color4 HarvestGold {
            get {
                return RGB(0.85f, 0.57f, 0f);
            }
        }
        public static Color4 HeatWave {
            get {
                return RGB(1f, 0.48f, 0f);
            }
        }
        public static Color4 Heliotrope {
            get {
                return RGB(0.87f, 0.45f, 1f);
            }
        }
        public static Color4 HeliotropeGray {
            get {
                return RGB(0.67f, 0.6f, 0.66f);
            }
        }
        public static Color4 HollywoodCerise {
            get {
                return RGB(0.96f, 0f, 0.63f);
            }
        }
        public static Color4 Honeydew {
            get {
                return RGB(0.94f, 1f, 0.94f);
            }
        }
        public static Color4 HonoluluBlue {
            get {
                return RGB(0f, 0.43f, 0.69f);
            }
        }
        public static Color4 HookerSGreen {
            get {
                return RGB(0.29f, 0.47f, 0.42f);
            }
        }
        public static Color4 HotMagenta {
            get {
                return RGB(1f, 0.11f, 0.81f);
            }
        }
        public static Color4 HotPink {
            get {
                return RGB(1f, 0.41f, 0.71f);
            }
        }
        public static Color4 HunterGreen {
            get {
                return RGB(0.21f, 0.37f, 0.23f);
            }
        }
        public static Color4 Iceberg {
            get {
                return RGB(0.44f, 0.65f, 0.82f);
            }
        }
        public static Color4 Icterine {
            get {
                return RGB(0.99f, 0.97f, 0.37f);
            }
        }
        public static Color4 IlluminatingEmerald {
            get {
                return RGB(0.19f, 0.57f, 0.47f);
            }
        }
        public static Color4 ImperialRed {
            get {
                return RGB(0.93f, 0.16f, 0.22f);
            }
        }
        public static Color4 Inchworm {
            get {
                return RGB(0.7f, 0.93f, 0.36f);
            }
        }
        public static Color4 Independence {
            get {
                return RGB(0.3f, 0.32f, 0.43f);
            }
        }
        public static Color4 IndiaGreen {
            get {
                return RGB(0.07f, 0.53f, 0.03f);
            }
        }
        public static Color4 IndianRed {
            get {
                return RGB(0.8f, 0.36f, 0.36f);
            }
        }
        public static Color4 IndianYellow {
            get {
                return RGB(0.89f, 0.66f, 0.34f);
            }
        }
        public static Color4 Indigo {
            get {
                return RGB(0.29f, 0f, 0.51f);
            }
        }
        public static Color4 IndigoDye {
            get {
                return RGB(0f, 0.25f, 0.42f);
            }
        }
        public static Color4 Infrared {
            get {
                return RGB(1f, 0.29f, 0.42f);
            }
        }
        public static Color4 InternationalKleinBlue {
            get {
                return RGB(0.07f, 0.04f, 0.56f);
            }
        }
        public static Color4 InternationalOrangeAerospace {
            get {
                return RGB(1f, 0.31f, 0f);
            }
        }
        public static Color4 InternationalOrangeEngineering {
            get {
                return RGB(0.73f, 0.09f, 0.05f);
            }
        }
        public static Color4 InternationalOrangeGoldenGateBridge {
            get {
                return RGB(0.75f, 0.21f, 0.17f);
            }
        }
        public static Color4 Iris {
            get {
                return RGB(0.35f, 0.31f, 0.81f);
            }
        }
        public static Color4 Irresistible {
            get {
                return RGB(0.7f, 0.27f, 0.42f);
            }
        }
        public static Color4 Isabelline {
            get {
                return RGB(0.96f, 0.94f, 0.93f);
            }
        }
        public static Color4 ItalianSkyBlue {
            get {
                return RGB(0.7f, 1f, 1f);
            }
        }
        public static Color4 Ivory {
            get {
                return RGB(1f, 1f, 0.94f);
            }
        }
        public static Color4 Jade {
            get {
                return RGB(0f, 0.66f, 0.42f);
            }
        }
        public static Color4 JapaneseCarmine {
            get {
                return RGB(0.62f, 0.16f, 0.2f);
            }
        }
        public static Color4 JapaneseViolet {
            get {
                return RGB(0.36f, 0.2f, 0.34f);
            }
        }
        public static Color4 Jasmine {
            get {
                return RGB(0.97f, 0.87f, 0.49f);
            }
        }
        public static Color4 JazzberryJam {
            get {
                return RGB(0.65f, 0.04f, 0.37f);
            }
        }
        public static Color4 Jet {
            get {
                return RGB(0.2f, 0.2f, 0.2f);
            }
        }
        public static Color4 Jonquil {
            get {
                return RGB(0.96f, 0.79f, 0.09f);
            }
        }
        public static Color4 JuneBud {
            get {
                return RGB(0.74f, 0.85f, 0.34f);
            }
        }
        public static Color4 JungleGreen {
            get {
                return RGB(0.16f, 0.67f, 0.53f);
            }
        }
        public static Color4 KellyGreen {
            get {
                return RGB(0.3f, 0.73f, 0.09f);
            }
        }
        public static Color4 Keppel {
            get {
                return RGB(0.23f, 0.69f, 0.62f);
            }
        }
        public static Color4 KeyLime {
            get {
                return RGB(0.91f, 0.96f, 0.55f);
            }
        }
        public static Color4 KhakiWeb {
            get {
                return RGB(0.76f, 0.69f, 0.57f);
            }
        }
        public static Color4 KhakiX11LightKhaki {
            get {
                return RGB(0.94f, 0.9f, 0.55f);
            }
        }
        public static Color4 Kobe {
            get {
                return RGB(0.53f, 0.18f, 0.09f);
            }
        }
        public static Color4 Kobi {
            get {
                return RGB(0.91f, 0.62f, 0.77f);
            }
        }
        public static Color4 Kobicha {
            get {
                return RGB(0.42f, 0.27f, 0.14f);
            }
        }
        public static Color4 KombuGreen {
            get {
                return RGB(0.21f, 0.26f, 0.19f);
            }
        }
        public static Color4 KsuPurple {
            get {
                return RGB(0.31f, 0.15f, 0.51f);
            }
        }
        public static Color4 LanguidLavender {
            get {
                return RGB(0.84f, 0.79f, 0.87f);
            }
        }
        public static Color4 LapisLazuli {
            get {
                return RGB(0.15f, 0.38f, 0.61f);
            }
        }
        public static Color4 LaserLemon {
            get {
                return RGB(1f, 1f, 0.4f);
            }
        }
        public static Color4 LaurelGreen {
            get {
                return RGB(0.66f, 0.73f, 0.62f);
            }
        }
        public static Color4 Lava {
            get {
                return RGB(0.81f, 0.06f, 0.13f);
            }
        }
        public static Color4 LavenderFloral {
            get {
                return RGB(0.71f, 0.49f, 0.86f);
            }
        }
        public static Color4 LavenderWeb {
            get {
                return RGB(0.9f, 0.9f, 0.98f);
            }
        }
        public static Color4 LavenderBlue {
            get {
                return RGB(0.8f, 0.8f, 1f);
            }
        }
        public static Color4 LavenderBlush {
            get {
                return RGB(1f, 0.94f, 0.96f);
            }
        }
        public static Color4 LavenderGray {
            get {
                return RGB(0.77f, 0.76f, 0.82f);
            }
        }
        public static Color4 LawnGreen {
            get {
                return RGB(0.49f, 0.99f, 0f);
            }
        }
        public static Color4 Lemon {
            get {
                return RGB(1f, 0.97f, 0f);
            }
        }
        public static Color4 LemonChiffon {
            get {
                return RGB(1f, 0.98f, 0.8f);
            }
        }
        public static Color4 LemonCurry {
            get {
                return RGB(0.8f, 0.63f, 0.11f);
            }
        }
        public static Color4 LemonGlacier {
            get {
                return RGB(0.99f, 1f, 0f);
            }
        }
        public static Color4 LemonMeringue {
            get {
                return RGB(0.96f, 0.92f, 0.75f);
            }
        }
        public static Color4 LemonYellow {
            get {
                return RGB(1f, 0.96f, 0.31f);
            }
        }
        public static Color4 LemonYellowCrayola {
            get {
                return RGB(1f, 1f, 0.62f);
            }
        }
        public static Color4 Liberty {
            get {
                return RGB(0.33f, 0.35f, 0.65f);
            }
        }
        public static Color4 LightBlue {
            get {
                return RGB(0.68f, 0.85f, 0.9f);
            }
        }
        public static Color4 LightCoral {
            get {
                return RGB(0.94f, 0.5f, 0.5f);
            }
        }
        public static Color4 LightCornflowerBlue {
            get {
                return RGB(0.58f, 0.8f, 0.92f);
            }
        }
        public static Color4 LightCyan {
            get {
                return RGB(0.88f, 1f, 1f);
            }
        }
        public static Color4 LightFrenchBeige {
            get {
                return RGB(0.78f, 0.68f, 0.5f);
            }
        }
        public static Color4 LightGoldenrodYellow {
            get {
                return RGB(0.98f, 0.98f, 0.82f);
            }
        }
        public static Color4 LightGray {
            get {
                return RGB(0.83f, 0.83f, 0.83f);
            }
        }
        public static Color4 LightGreen {
            get {
                return RGB(0.56f, 0.93f, 0.56f);
            }
        }
        public static Color4 LightOrange {
            get {
                return RGB(1f, 0.85f, 0.69f);
            }
        }
        public static Color4 LightPeriwinkle {
            get {
                return RGB(0.77f, 0.8f, 0.88f);
            }
        }
        public static Color4 LightPink {
            get {
                return RGB(1f, 0.71f, 0.76f);
            }
        }
        public static Color4 LightSalmon {
            get {
                return RGB(1f, 0.63f, 0.48f);
            }
        }
        public static Color4 LightSeaGreen {
            get {
                return RGB(0.13f, 0.7f, 0.67f);
            }
        }
        public static Color4 LightSkyBlue {
            get {
                return RGB(0.53f, 0.81f, 0.98f);
            }
        }
        public static Color4 LightSlateGray {
            get {
                return RGB(0.47f, 0.53f, 0.6f);
            }
        }
        public static Color4 LightSteelBlue {
            get {
                return RGB(0.69f, 0.77f, 0.87f);
            }
        }
        public static Color4 LightYellow {
            get {
                return RGB(1f, 1f, 0.88f);
            }
        }
        public static Color4 Lilac {
            get {
                return RGB(0.78f, 0.64f, 0.78f);
            }
        }
        public static Color4 LilacLuster {
            get {
                return RGB(0.68f, 0.6f, 0.67f);
            }
        }
        public static Color4 LimeColorWheel {
            get {
                return RGB(0.75f, 1f, 0f);
            }
        }
        public static Color4 LimeWebX11Green {
            get {
                return RGB(0f, 1f, 0f);
            }
        }
        public static Color4 LimeGreen {
            get {
                return RGB(0.2f, 0.8f, 0.2f);
            }
        }
        public static Color4 LincolnGreen {
            get {
                return RGB(0.1f, 0.35f, 0.02f);
            }
        }
        public static Color4 Linen {
            get {
                return RGB(0.98f, 0.94f, 0.9f);
            }
        }
        public static Color4 Lion {
            get {
                return RGB(0.76f, 0.6f, 0.42f);
            }
        }
        public static Color4 LiseranPurple {
            get {
                return RGB(0.87f, 0.44f, 0.63f);
            }
        }
        public static Color4 LittleBoyBlue {
            get {
                return RGB(0.42f, 0.63f, 0.86f);
            }
        }
        public static Color4 Liver {
            get {
                return RGB(0.4f, 0.3f, 0.28f);
            }
        }
        public static Color4 LiverDogs {
            get {
                return RGB(0.72f, 0.43f, 0.16f);
            }
        }
        public static Color4 LiverOrgan {
            get {
                return RGB(0.42f, 0.18f, 0.12f);
            }
        }
        public static Color4 LiverChestnut {
            get {
                return RGB(0.6f, 0.45f, 0.34f);
            }
        }
        public static Color4 Livid {
            get {
                return RGB(0.4f, 0.6f, 0.8f);
            }
        }
        public static Color4 MacaroniAndCheese {
            get {
                return RGB(1f, 0.74f, 0.53f);
            }
        }
        public static Color4 MadderLake {
            get {
                return RGB(0.8f, 0.2f, 0.21f);
            }
        }
        public static Color4 Magenta {
            get {
                return RGB(1f, 0f, 1f);
            }
        }
        public static Color4 MagentaCrayola {
            get {
                return RGB(0.96f, 0.33f, 0.65f);
            }
        }
        public static Color4 MagentaDye {
            get {
                return RGB(0.79f, 0.12f, 0.48f);
            }
        }
        public static Color4 MagentaPantone {
            get {
                return RGB(0.82f, 0.25f, 0.49f);
            }
        }
        public static Color4 MagentaProcess {
            get {
                return RGB(1f, 0f, 0.56f);
            }
        }
        public static Color4 MagentaHaze {
            get {
                return RGB(0.62f, 0.27f, 0.46f);
            }
        }
        public static Color4 MagicMint {
            get {
                return RGB(0.67f, 0.94f, 0.82f);
            }
        }
        public static Color4 Magnolia {
            get {
                return RGB(0.95f, 0.91f, 0.84f);
            }
        }
        public static Color4 Mahogany {
            get {
                return RGB(0.75f, 0.25f, 0f);
            }
        }
        public static Color4 Maize {
            get {
                return RGB(0.98f, 0.93f, 0.36f);
            }
        }
        public static Color4 MaizeCrayola {
            get {
                return RGB(0.95f, 0.78f, 0.29f);
            }
        }
        public static Color4 MajorelleBlue {
            get {
                return RGB(0.38f, 0.31f, 0.86f);
            }
        }
        public static Color4 Malachite {
            get {
                return RGB(0.04f, 0.85f, 0.32f);
            }
        }
        public static Color4 Manatee {
            get {
                return RGB(0.59f, 0.6f, 0.67f);
            }
        }
        public static Color4 Mandarin {
            get {
                return RGB(0.95f, 0.48f, 0.28f);
            }
        }
        public static Color4 Mango {
            get {
                return RGB(0.99f, 0.75f, 0.01f);
            }
        }
        public static Color4 MangoTango {
            get {
                return RGB(1f, 0.51f, 0.26f);
            }
        }
        public static Color4 Mantis {
            get {
                return RGB(0.45f, 0.76f, 0.4f);
            }
        }
        public static Color4 MardiGras {
            get {
                return RGB(0.53f, 0f, 0.52f);
            }
        }
        public static Color4 Marigold {
            get {
                return RGB(0.92f, 0.64f, 0.13f);
            }
        }
        public static Color4 MaroonCrayola {
            get {
                return RGB(0.76f, 0.13f, 0.28f);
            }
        }
        public static Color4 MaroonWeb {
            get {
                return RGB(0.5f, 0f, 0f);
            }
        }
        public static Color4 MaroonX11 {
            get {
                return RGB(0.69f, 0.19f, 0.38f);
            }
        }
        public static Color4 Mauve {
            get {
                return RGB(0.88f, 0.69f, 1f);
            }
        }
        public static Color4 MauveTaupe {
            get {
                return RGB(0.57f, 0.37f, 0.43f);
            }
        }
        public static Color4 Mauvelous {
            get {
                return RGB(0.94f, 0.6f, 0.67f);
            }
        }
        public static Color4 MaximumBlue {
            get {
                return RGB(0.28f, 0.67f, 0.8f);
            }
        }
        public static Color4 MaximumBlueGreen {
            get {
                return RGB(0.19f, 0.75f, 0.75f);
            }
        }
        public static Color4 MaximumBluePurple {
            get {
                return RGB(0.67f, 0.67f, 0.9f);
            }
        }
        public static Color4 MaximumGreen {
            get {
                return RGB(0.37f, 0.55f, 0.19f);
            }
        }
        public static Color4 MaximumGreenYellow {
            get {
                return RGB(0.85f, 0.9f, 0.31f);
            }
        }
        public static Color4 MaximumPurple {
            get {
                return RGB(0.45f, 0.2f, 0.5f);
            }
        }
        public static Color4 MaximumRed {
            get {
                return RGB(0.85f, 0.13f, 0.13f);
            }
        }
        public static Color4 MaximumRedPurple {
            get {
                return RGB(0.65f, 0.23f, 0.47f);
            }
        }
        public static Color4 MaximumYellow {
            get {
                return RGB(0.98f, 0.98f, 0.22f);
            }
        }
        public static Color4 MaximumYellowRed {
            get {
                return RGB(0.95f, 0.73f, 0.29f);
            }
        }
        public static Color4 MayGreen {
            get {
                return RGB(0.3f, 0.57f, 0.25f);
            }
        }
        public static Color4 MayaBlue {
            get {
                return RGB(0.45f, 0.76f, 0.98f);
            }
        }
        public static Color4 MediumAquamarine {
            get {
                return RGB(0.4f, 0.87f, 0.67f);
            }
        }
        public static Color4 MediumBlue {
            get {
                return RGB(0f, 0f, 0.8f);
            }
        }
        public static Color4 MediumCandyAppleRed {
            get {
                return RGB(0.89f, 0.02f, 0.17f);
            }
        }
        public static Color4 MediumCarmine {
            get {
                return RGB(0.69f, 0.25f, 0.21f);
            }
        }
        public static Color4 MediumChampagne {
            get {
                return RGB(0.95f, 0.9f, 0.67f);
            }
        }
        public static Color4 MediumOrchid {
            get {
                return RGB(0.73f, 0.33f, 0.83f);
            }
        }
        public static Color4 MediumPurple {
            get {
                return RGB(0.58f, 0.44f, 0.86f);
            }
        }
        public static Color4 MediumSeaGreen {
            get {
                return RGB(0.24f, 0.7f, 0.44f);
            }
        }
        public static Color4 MediumSlateBlue {
            get {
                return RGB(0.48f, 0.41f, 0.93f);
            }
        }
        public static Color4 MediumSpringGreen {
            get {
                return RGB(0f, 0.98f, 0.6f);
            }
        }
        public static Color4 MediumTurquoise {
            get {
                return RGB(0.28f, 0.82f, 0.8f);
            }
        }
        public static Color4 MediumVioletRed {
            get {
                return RGB(0.78f, 0.08f, 0.52f);
            }
        }
        public static Color4 MellowApricot {
            get {
                return RGB(0.97f, 0.72f, 0.47f);
            }
        }
        public static Color4 MellowYellow {
            get {
                return RGB(0.97f, 0.87f, 0.49f);
            }
        }
        public static Color4 Melon {
            get {
                return RGB(1f, 0.73f, 0.68f);
            }
        }
        public static Color4 MetallicGold {
            get {
                return RGB(0.83f, 0.69f, 0.22f);
            }
        }
        public static Color4 MetallicSeaweed {
            get {
                return RGB(0.04f, 0.49f, 0.55f);
            }
        }
        public static Color4 MetallicSunburst {
            get {
                return RGB(0.61f, 0.49f, 0.22f);
            }
        }
        public static Color4 MexicanPink {
            get {
                return RGB(0.89f, 0f, 0.49f);
            }
        }
        public static Color4 MiddleBlue {
            get {
                return RGB(0.49f, 0.83f, 0.9f);
            }
        }
        public static Color4 MiddleBlueGreen {
            get {
                return RGB(0.55f, 0.85f, 0.8f);
            }
        }
        public static Color4 MiddleBluePurple {
            get {
                return RGB(0.55f, 0.45f, 0.75f);
            }
        }
        public static Color4 MiddleGrey {
            get {
                return RGB(0.55f, 0.53f, 0.5f);
            }
        }
        public static Color4 MiddleGreen {
            get {
                return RGB(0.3f, 0.55f, 0.34f);
            }
        }
        public static Color4 MiddleGreenYellow {
            get {
                return RGB(0.67f, 0.75f, 0.38f);
            }
        }
        public static Color4 MiddlePurple {
            get {
                return RGB(0.85f, 0.51f, 0.71f);
            }
        }
        public static Color4 MiddleRed {
            get {
                return RGB(0.9f, 0.56f, 0.45f);
            }
        }
        public static Color4 MiddleRedPurple {
            get {
                return RGB(0.65f, 0.33f, 0.33f);
            }
        }
        public static Color4 MiddleYellow {
            get {
                return RGB(1f, 0.92f, 0f);
            }
        }
        public static Color4 MiddleYellowRed {
            get {
                return RGB(0.93f, 0.69f, 0.46f);
            }
        }
        public static Color4 Midnight {
            get {
                return RGB(0.44f, 0.15f, 0.44f);
            }
        }
        public static Color4 MidnightBlue {
            get {
                return RGB(0.1f, 0.1f, 0.44f);
            }
        }
        public static Color4 MidnightGreenEagleGreen {
            get {
                return RGB(0f, 0.29f, 0.33f);
            }
        }
        public static Color4 MikadoYellow {
            get {
                return RGB(1f, 0.77f, 0.05f);
            }
        }
        public static Color4 MimiPink {
            get {
                return RGB(1f, 0.85f, 0.91f);
            }
        }
        public static Color4 Mindaro {
            get {
                return RGB(0.89f, 0.98f, 0.53f);
            }
        }
        public static Color4 Ming {
            get {
                return RGB(0.21f, 0.45f, 0.49f);
            }
        }
        public static Color4 MinionYellow {
            get {
                return RGB(0.96f, 0.86f, 0.31f);
            }
        }
        public static Color4 Mint {
            get {
                return RGB(0.24f, 0.71f, 0.54f);
            }
        }
        public static Color4 MintCream {
            get {
                return RGB(0.96f, 1f, 0.98f);
            }
        }
        public static Color4 MintGreen {
            get {
                return RGB(0.6f, 1f, 0.6f);
            }
        }
        public static Color4 MistyMoss {
            get {
                return RGB(0.73f, 0.71f, 0.47f);
            }
        }
        public static Color4 MistyRose {
            get {
                return RGB(1f, 0.89f, 0.88f);
            }
        }
        public static Color4 ModeBeige {
            get {
                return RGB(0.59f, 0.44f, 0.09f);
            }
        }
        public static Color4 MorningBlue {
            get {
                return RGB(0.55f, 0.64f, 0.6f);
            }
        }
        public static Color4 MossGreen {
            get {
                return RGB(0.54f, 0.6f, 0.36f);
            }
        }
        public static Color4 MountainMeadow {
            get {
                return RGB(0.19f, 0.73f, 0.56f);
            }
        }
        public static Color4 MountbattenPink {
            get {
                return RGB(0.6f, 0.48f, 0.55f);
            }
        }
        public static Color4 MsuGreen {
            get {
                return RGB(0.09f, 0.27f, 0.23f);
            }
        }
        public static Color4 Mulberry {
            get {
                return RGB(0.77f, 0.29f, 0.55f);
            }
        }
        public static Color4 MulberryCrayola {
            get {
                return RGB(0.78f, 0.31f, 0.61f);
            }
        }
        public static Color4 Mustard {
            get {
                return RGB(1f, 0.86f, 0.35f);
            }
        }
        public static Color4 MyrtleGreen {
            get {
                return RGB(0.19f, 0.47f, 0.45f);
            }
        }
        public static Color4 Mystic {
            get {
                return RGB(0.84f, 0.32f, 0.51f);
            }
        }
        public static Color4 MysticMaroon {
            get {
                return RGB(0.68f, 0.26f, 0.47f);
            }
        }
        public static Color4 NadeshikoPink {
            get {
                return RGB(0.96f, 0.68f, 0.78f);
            }
        }
        public static Color4 NaplesYellow {
            get {
                return RGB(0.98f, 0.85f, 0.37f);
            }
        }
        public static Color4 NavajoWhite {
            get {
                return RGB(1f, 0.87f, 0.68f);
            }
        }
        public static Color4 NavyBlue {
            get {
                return RGB(0f, 0f, 0.5f);
            }
        }
        public static Color4 NavyBlueCrayola {
            get {
                return RGB(0.1f, 0.45f, 0.82f);
            }
        }
        public static Color4 NeonBlue {
            get {
                return RGB(0.27f, 0.4f, 1f);
            }
        }
        public static Color4 NeonCarrot {
            get {
                return RGB(1f, 0.64f, 0.26f);
            }
        }
        public static Color4 NeonGreen {
            get {
                return RGB(0.22f, 1f, 0.08f);
            }
        }
        public static Color4 NeonFuchsia {
            get {
                return RGB(1f, 0.25f, 0.39f);
            }
        }
        public static Color4 NewYorkPink {
            get {
                return RGB(0.84f, 0.51f, 0.5f);
            }
        }
        public static Color4 Nickel {
            get {
                return RGB(0.45f, 0.45f, 0.45f);
            }
        }
        public static Color4 NonPhotoBlue {
            get {
                return RGB(0.64f, 0.87f, 0.93f);
            }
        }
        public static Color4 Nyanza {
            get {
                return RGB(0.91f, 1f, 0.86f);
            }
        }
        public static Color4 OceanBlue {
            get {
                return RGB(0.31f, 0.26f, 0.71f);
            }
        }
        public static Color4 OceanGreen {
            get {
                return RGB(0.28f, 0.75f, 0.57f);
            }
        }
        public static Color4 Ochre {
            get {
                return RGB(0.8f, 0.47f, 0.13f);
            }
        }
        public static Color4 OldBurgundy {
            get {
                return RGB(0.26f, 0.19f, 0.18f);
            }
        }
        public static Color4 OldGold {
            get {
                return RGB(0.81f, 0.71f, 0.23f);
            }
        }
        public static Color4 OldLace {
            get {
                return RGB(0.99f, 0.96f, 0.9f);
            }
        }
        public static Color4 OldLavender {
            get {
                return RGB(0.47f, 0.41f, 0.47f);
            }
        }
        public static Color4 OldMauve {
            get {
                return RGB(0.4f, 0.19f, 0.28f);
            }
        }
        public static Color4 OldRose {
            get {
                return RGB(0.75f, 0.5f, 0.51f);
            }
        }
        public static Color4 OldSilver {
            get {
                return RGB(0.52f, 0.52f, 0.51f);
            }
        }
        public static Color4 Olive {
            get {
                return RGB(0.5f, 0.5f, 0f);
            }
        }
        public static Color4 OliveDrab3 {
            get {
                return RGB(0.42f, 0.56f, 0.14f);
            }
        }
        public static Color4 OliveDrab7 {
            get {
                return RGB(0.24f, 0.2f, 0.12f);
            }
        }
        public static Color4 OliveGreen {
            get {
                return RGB(0.71f, 0.7f, 0.36f);
            }
        }
        public static Color4 Olivine {
            get {
                return RGB(0.6f, 0.73f, 0.45f);
            }
        }
        public static Color4 Onyx {
            get {
                return RGB(0.21f, 0.22f, 0.22f);
            }
        }
        public static Color4 Opal {
            get {
                return RGB(0.66f, 0.76f, 0.74f);
            }
        }
        public static Color4 OperaMauve {
            get {
                return RGB(0.72f, 0.52f, 0.65f);
            }
        }
        public static Color4 Orange {
            get {
                return RGB(1f, 0.5f, 0f);
            }
        }
        public static Color4 OrangeCrayola {
            get {
                return RGB(1f, 0.46f, 0.22f);
            }
        }
        public static Color4 OrangePantone {
            get {
                return RGB(1f, 0.35f, 0f);
            }
        }
        public static Color4 OrangeWeb {
            get {
                return RGB(1f, 0.65f, 0f);
            }
        }
        public static Color4 OrangePeel {
            get {
                return RGB(1f, 0.62f, 0f);
            }
        }
        public static Color4 OrangeRed {
            get {
                return RGB(1f, 0.41f, 0.12f);
            }
        }
        public static Color4 OrangeRedCrayola {
            get {
                return RGB(1f, 0.33f, 0.29f);
            }
        }
        public static Color4 OrangeSoda {
            get {
                return RGB(0.98f, 0.36f, 0.24f);
            }
        }
        public static Color4 OrangeYellow {
            get {
                return RGB(0.96f, 0.74f, 0.12f);
            }
        }
        public static Color4 OrangeYellowCrayola {
            get {
                return RGB(0.97f, 0.84f, 0.41f);
            }
        }
        public static Color4 Orchid {
            get {
                return RGB(0.85f, 0.44f, 0.84f);
            }
        }
        public static Color4 OrchidPink {
            get {
                return RGB(0.95f, 0.74f, 0.8f);
            }
        }
        public static Color4 OrchidCrayola {
            get {
                return RGB(0.89f, 0.61f, 0.82f);
            }
        }
        public static Color4 OuterSpaceCrayola {
            get {
                return RGB(0.18f, 0.22f, 0.23f);
            }
        }
        public static Color4 OutrageousOrange {
            get {
                return RGB(1f, 0.43f, 0.29f);
            }
        }
        public static Color4 Oxblood {
            get {
                return RGB(0.29f, 0f, 0f);
            }
        }
        public static Color4 OxfordBlue {
            get {
                return RGB(0f, 0.13f, 0.28f);
            }
        }
        public static Color4 OuCrimsonRed {
            get {
                return RGB(0.52f, 0.09f, 0.09f);
            }
        }
        public static Color4 PacificBlue {
            get {
                return RGB(0.11f, 0.66f, 0.79f);
            }
        }
        public static Color4 PakistanGreen {
            get {
                return RGB(0f, 0.4f, 0f);
            }
        }
        public static Color4 PalatinatePurple {
            get {
                return RGB(0.41f, 0.16f, 0.38f);
            }
        }
        public static Color4 PaleAqua {
            get {
                return RGB(0.74f, 0.83f, 0.9f);
            }
        }
        public static Color4 PaleCerulean {
            get {
                return RGB(0.61f, 0.77f, 0.89f);
            }
        }
        public static Color4 PaleDogwood {
            get {
                return RGB(0.93f, 0.48f, 0.61f);
            }
        }
        public static Color4 PalePink {
            get {
                return RGB(0.98f, 0.85f, 0.87f);
            }
        }
        public static Color4 PalePurplePantone {
            get {
                return RGB(0.98f, 0.9f, 0.98f);
            }
        }
        public static Color4 PaleSilver {
            get {
                return RGB(0.79f, 0.75f, 0.73f);
            }
        }
        public static Color4 PaleSpringBud {
            get {
                return RGB(0.93f, 0.92f, 0.74f);
            }
        }
        public static Color4 PansyPurple {
            get {
                return RGB(0.47f, 0.09f, 0.29f);
            }
        }
        public static Color4 PaoloVeroneseGreen {
            get {
                return RGB(0f, 0.61f, 0.49f);
            }
        }
        public static Color4 PapayaWhip {
            get {
                return RGB(1f, 0.94f, 0.84f);
            }
        }
        public static Color4 ParadisePink {
            get {
                return RGB(0.9f, 0.24f, 0.38f);
            }
        }
        public static Color4 Parchment {
            get {
                return RGB(0.95f, 0.91f, 0.82f);
            }
        }
        public static Color4 ParisGreen {
            get {
                return RGB(0.31f, 0.78f, 0.47f);
            }
        }
        public static Color4 PastelPink {
            get {
                return RGB(0.87f, 0.65f, 0.64f);
            }
        }
        public static Color4 Patriarch {
            get {
                return RGB(0.5f, 0f, 0.5f);
            }
        }
        public static Color4 PayneSGrey {
            get {
                return RGB(0.33f, 0.41f, 0.47f);
            }
        }
        public static Color4 Peach {
            get {
                return RGB(1f, 0.9f, 0.71f);
            }
        }
        public static Color4 PeachCrayola {
            get {
                return RGB(1f, 0.8f, 0.64f);
            }
        }
        public static Color4 PeachPuff {
            get {
                return RGB(1f, 0.85f, 0.73f);
            }
        }
        public static Color4 Pear {
            get {
                return RGB(0.82f, 0.89f, 0.19f);
            }
        }
        public static Color4 PearlyPurple {
            get {
                return RGB(0.72f, 0.41f, 0.64f);
            }
        }
        public static Color4 Periwinkle {
            get {
                return RGB(0.8f, 0.8f, 1f);
            }
        }
        public static Color4 PeriwinkleCrayola {
            get {
                return RGB(0.76f, 0.8f, 0.9f);
            }
        }
        public static Color4 PermanentGeraniumLake {
            get {
                return RGB(0.88f, 0.17f, 0.17f);
            }
        }
        public static Color4 PersianBlue {
            get {
                return RGB(0.11f, 0.22f, 0.73f);
            }
        }
        public static Color4 PersianGreen {
            get {
                return RGB(0f, 0.65f, 0.58f);
            }
        }
        public static Color4 PersianIndigo {
            get {
                return RGB(0.2f, 0.07f, 0.48f);
            }
        }
        public static Color4 PersianOrange {
            get {
                return RGB(0.85f, 0.56f, 0.35f);
            }
        }
        public static Color4 PersianPink {
            get {
                return RGB(0.97f, 0.5f, 0.75f);
            }
        }
        public static Color4 PersianPlum {
            get {
                return RGB(0.44f, 0.11f, 0.11f);
            }
        }
        public static Color4 PersianRed {
            get {
                return RGB(0.8f, 0.2f, 0.2f);
            }
        }
        public static Color4 PersianRose {
            get {
                return RGB(1f, 0.16f, 0.64f);
            }
        }
        public static Color4 Persimmon {
            get {
                return RGB(0.93f, 0.35f, 0f);
            }
        }
        public static Color4 PewterBlue {
            get {
                return RGB(0.55f, 0.66f, 0.72f);
            }
        }
        public static Color4 Phlox {
            get {
                return RGB(0.87f, 0f, 1f);
            }
        }
        public static Color4 PhthaloBlue {
            get {
                return RGB(0f, 0.06f, 0.54f);
            }
        }
        public static Color4 PhthaloGreen {
            get {
                return RGB(0.07f, 0.21f, 0.14f);
            }
        }
        public static Color4 PicoteeBlue {
            get {
                return RGB(0.18f, 0.15f, 0.53f);
            }
        }
        public static Color4 PictorialCarmine {
            get {
                return RGB(0.76f, 0.04f, 0.31f);
            }
        }
        public static Color4 PiggyPink {
            get {
                return RGB(0.99f, 0.87f, 0.9f);
            }
        }
        public static Color4 PineGreen {
            get {
                return RGB(0f, 0.47f, 0.44f);
            }
        }
        public static Color4 PineTree {
            get {
                return RGB(0.16f, 0.18f, 0.14f);
            }
        }
        public static Color4 Pink {
            get {
                return RGB(1f, 0.75f, 0.8f);
            }
        }
        public static Color4 PinkPantone {
            get {
                return RGB(0.84f, 0.28f, 0.58f);
            }
        }
        public static Color4 PinkFlamingo {
            get {
                return RGB(0.99f, 0.45f, 0.99f);
            }
        }
        public static Color4 PinkLace {
            get {
                return RGB(1f, 0.87f, 0.96f);
            }
        }
        public static Color4 PinkLavender {
            get {
                return RGB(0.85f, 0.7f, 0.82f);
            }
        }
        public static Color4 PinkSherbet {
            get {
                return RGB(0.97f, 0.56f, 0.65f);
            }
        }
        public static Color4 Pistachio {
            get {
                return RGB(0.58f, 0.77f, 0.45f);
            }
        }
        public static Color4 Platinum {
            get {
                return RGB(0.9f, 0.89f, 0.89f);
            }
        }
        public static Color4 Plum {
            get {
                return RGB(0.56f, 0.27f, 0.52f);
            }
        }
        public static Color4 PlumWeb {
            get {
                return RGB(0.87f, 0.63f, 0.87f);
            }
        }
        public static Color4 PlumpPurple {
            get {
                return RGB(0.35f, 0.27f, 0.7f);
            }
        }
        public static Color4 PolishedPine {
            get {
                return RGB(0.36f, 0.64f, 0.58f);
            }
        }
        public static Color4 PompAndPower {
            get {
                return RGB(0.53f, 0.38f, 0.56f);
            }
        }
        public static Color4 Popstar {
            get {
                return RGB(0.75f, 0.31f, 0.38f);
            }
        }
        public static Color4 PortlandOrange {
            get {
                return RGB(1f, 0.35f, 0.21f);
            }
        }
        public static Color4 PowderBlue {
            get {
                return RGB(0.69f, 0.88f, 0.9f);
            }
        }
        public static Color4 PrincetonOrange {
            get {
                return RGB(0.96f, 0.5f, 0.15f);
            }
        }
        public static Color4 ProcessYellow {
            get {
                return RGB(1f, 0.94f, 0f);
            }
        }
        public static Color4 Prune {
            get {
                return RGB(0.44f, 0.11f, 0.11f);
            }
        }
        public static Color4 PrussianBlue {
            get {
                return RGB(0f, 0.19f, 0.33f);
            }
        }
        public static Color4 PsychedelicPurple {
            get {
                return RGB(0.87f, 0f, 1f);
            }
        }
        public static Color4 Puce {
            get {
                return RGB(0.8f, 0.53f, 0.6f);
            }
        }
        public static Color4 PullmanBrownUpsBrown {
            get {
                return RGB(0.39f, 0.25f, 0.09f);
            }
        }
        public static Color4 Pumpkin {
            get {
                return RGB(1f, 0.46f, 0.09f);
            }
        }
        public static Color4 Purple {
            get {
                return RGB(0.38f, 0f, 0.5f);
            }
        }
        public static Color4 PurpleWeb {
            get {
                return RGB(0.5f, 0f, 0.5f);
            }
        }
        public static Color4 PurpleMunsell {
            get {
                return RGB(0.62f, 0f, 0.77f);
            }
        }
        public static Color4 PurpleX11 {
            get {
                return RGB(0.63f, 0.13f, 0.94f);
            }
        }
        public static Color4 PurpleMountainMajesty {
            get {
                return RGB(0.59f, 0.47f, 0.71f);
            }
        }
        public static Color4 PurpleNavy {
            get {
                return RGB(0.31f, 0.32f, 0.5f);
            }
        }
        public static Color4 PurplePizzazz {
            get {
                return RGB(1f, 0.31f, 0.85f);
            }
        }
        public static Color4 PurplePlum {
            get {
                return RGB(0.61f, 0.32f, 0.71f);
            }
        }
        public static Color4 Purpureus {
            get {
                return RGB(0.6f, 0.31f, 0.68f);
            }
        }
        public static Color4 QueenBlue {
            get {
                return RGB(0.26f, 0.42f, 0.58f);
            }
        }
        public static Color4 QueenPink {
            get {
                return RGB(0.91f, 0.8f, 0.84f);
            }
        }
        public static Color4 QuickSilver {
            get {
                return RGB(0.65f, 0.65f, 0.65f);
            }
        }
        public static Color4 QuinacridoneMagenta {
            get {
                return RGB(0.56f, 0.23f, 0.35f);
            }
        }
        public static Color4 RadicalRed {
            get {
                return RGB(1f, 0.21f, 0.37f);
            }
        }
        public static Color4 RaisinBlack {
            get {
                return RGB(0.14f, 0.13f, 0.14f);
            }
        }
        public static Color4 Rajah {
            get {
                return RGB(0.98f, 0.67f, 0.38f);
            }
        }
        public static Color4 Raspberry {
            get {
                return RGB(0.89f, 0.04f, 0.36f);
            }
        }
        public static Color4 RaspberryGlace {
            get {
                return RGB(0.57f, 0.37f, 0.43f);
            }
        }
        public static Color4 RaspberryRose {
            get {
                return RGB(0.7f, 0.27f, 0.42f);
            }
        }
        public static Color4 RawSienna {
            get {
                return RGB(0.84f, 0.54f, 0.35f);
            }
        }
        public static Color4 RawUmber {
            get {
                return RGB(0.51f, 0.4f, 0.27f);
            }
        }
        public static Color4 RazzleDazzleRose {
            get {
                return RGB(1f, 0.2f, 0.8f);
            }
        }
        public static Color4 Razzmatazz {
            get {
                return RGB(0.89f, 0.15f, 0.42f);
            }
        }
        public static Color4 RazzmicBerry {
            get {
                return RGB(0.55f, 0.31f, 0.52f);
            }
        }
        public static Color4 RebeccaPurple {
            get {
                return RGB(0.4f, 0.2f, 0.6f);
            }
        }
        public static Color4 Red {
            get {
                return RGB(1f, 0f, 0f);
            }
        }
        public static Color4 RedCrayola {
            get {
                return RGB(0.93f, 0.13f, 0.3f);
            }
        }
        public static Color4 RedMunsell {
            get {
                return RGB(0.95f, 0f, 0.24f);
            }
        }
        public static Color4 RedNcs {
            get {
                return RGB(0.77f, 0.01f, 0.2f);
            }
        }
        public static Color4 RedPantone {
            get {
                return RGB(0.93f, 0.16f, 0.22f);
            }
        }
        public static Color4 RedPigment {
            get {
                return RGB(0.93f, 0.11f, 0.14f);
            }
        }
        public static Color4 RedRyb {
            get {
                return RGB(1f, 0.15f, 0.07f);
            }
        }
        public static Color4 RedOrange {
            get {
                return RGB(1f, 0.33f, 0.29f);
            }
        }
        public static Color4 RedOrangeCrayola {
            get {
                return RGB(1f, 0.41f, 0.12f);
            }
        }
        public static Color4 RedOrangeColorWheel {
            get {
                return RGB(1f, 0.27f, 0f);
            }
        }
        public static Color4 RedPurple {
            get {
                return RGB(0.89f, 0f, 0.47f);
            }
        }
        public static Color4 RedSalsa {
            get {
                return RGB(0.99f, 0.23f, 0.29f);
            }
        }
        public static Color4 RedViolet {
            get {
                return RGB(0.78f, 0.08f, 0.52f);
            }
        }
        public static Color4 RedVioletCrayola {
            get {
                return RGB(0.75f, 0.27f, 0.56f);
            }
        }
        public static Color4 RedVioletColorWheel {
            get {
                return RGB(0.57f, 0.17f, 0.24f);
            }
        }
        public static Color4 Redwood {
            get {
                return RGB(0.64f, 0.35f, 0.32f);
            }
        }
        public static Color4 ResolutionBlue {
            get {
                return RGB(0f, 0.14f, 0.53f);
            }
        }
        public static Color4 Rhythm {
            get {
                return RGB(0.47f, 0.46f, 0.59f);
            }
        }
        public static Color4 RichBlack {
            get {
                return RGB(0f, 0.25f, 0.25f);
            }
        }
        public static Color4 RichBlackFogra29 {
            get {
                return RGB(0f, 0.04f, 0.07f);
            }
        }
        public static Color4 RichBlackFogra39 {
            get {
                return RGB(0f, 0.01f, 0.01f);
            }
        }
        public static Color4 RifleGreen {
            get {
                return RGB(0.27f, 0.3f, 0.22f);
            }
        }
        public static Color4 RobinEggBlue {
            get {
                return RGB(0f, 0.8f, 0.8f);
            }
        }
        public static Color4 RocketMetallic {
            get {
                return RGB(0.54f, 0.5f, 0.5f);
            }
        }
        public static Color4 RojoSpanishRed {
            get {
                return RGB(0.66f, 0.07f, 0f);
            }
        }
        public static Color4 RomanSilver {
            get {
                return RGB(0.51f, 0.54f, 0.59f);
            }
        }
        public static Color4 Rose {
            get {
                return RGB(1f, 0f, 0.5f);
            }
        }
        public static Color4 RoseBonbon {
            get {
                return RGB(0.98f, 0.26f, 0.62f);
            }
        }
        public static Color4 RoseDust {
            get {
                return RGB(0.62f, 0.37f, 0.44f);
            }
        }
        public static Color4 RoseEbony {
            get {
                return RGB(0.4f, 0.28f, 0.27f);
            }
        }
        public static Color4 RoseMadder {
            get {
                return RGB(0.89f, 0.15f, 0.21f);
            }
        }
        public static Color4 RosePink {
            get {
                return RGB(1f, 0.4f, 0.8f);
            }
        }
        public static Color4 RosePompadour {
            get {
                return RGB(0.93f, 0.48f, 0.61f);
            }
        }
        public static Color4 RoseQuartz {
            get {
                return RGB(0.67f, 0.6f, 0.66f);
            }
        }
        public static Color4 RoseRed {
            get {
                return RGB(0.76f, 0.12f, 0.34f);
            }
        }
        public static Color4 RoseTaupe {
            get {
                return RGB(0.56f, 0.36f, 0.36f);
            }
        }
        public static Color4 RoseVale {
            get {
                return RGB(0.67f, 0.31f, 0.32f);
            }
        }
        public static Color4 Rosewood {
            get {
                return RGB(0.4f, 0f, 0.04f);
            }
        }
        public static Color4 RossoCorsa {
            get {
                return RGB(0.83f, 0f, 0f);
            }
        }
        public static Color4 RosyBrown {
            get {
                return RGB(0.74f, 0.56f, 0.56f);
            }
        }
        public static Color4 RoyalBlueDark {
            get {
                return RGB(0f, 0.14f, 0.4f);
            }
        }
        public static Color4 RoyalBlueLight {
            get {
                return RGB(0.25f, 0.41f, 0.88f);
            }
        }
        public static Color4 RoyalPurple {
            get {
                return RGB(0.47f, 0.32f, 0.66f);
            }
        }
        public static Color4 RoyalYellow {
            get {
                return RGB(0.98f, 0.85f, 0.37f);
            }
        }
        public static Color4 Ruber {
            get {
                return RGB(0.81f, 0.27f, 0.46f);
            }
        }
        public static Color4 RubineRed {
            get {
                return RGB(0.82f, 0f, 0.34f);
            }
        }
        public static Color4 Ruby {
            get {
                return RGB(0.88f, 0.07f, 0.37f);
            }
        }
        public static Color4 RubyRed {
            get {
                return RGB(0.61f, 0.07f, 0.12f);
            }
        }
        public static Color4 Rufous {
            get {
                return RGB(0.66f, 0.11f, 0.03f);
            }
        }
        public static Color4 Russet {
            get {
                return RGB(0.5f, 0.27f, 0.11f);
            }
        }
        public static Color4 RussianGreen {
            get {
                return RGB(0.4f, 0.57f, 0.4f);
            }
        }
        public static Color4 RussianViolet {
            get {
                return RGB(0.2f, 0.09f, 0.3f);
            }
        }
        public static Color4 Rust {
            get {
                return RGB(0.72f, 0.25f, 0.05f);
            }
        }
        public static Color4 RustyRed {
            get {
                return RGB(0.85f, 0.17f, 0.26f);
            }
        }
        public static Color4 SacramentoStateGreen {
            get {
                return RGB(0.02f, 0.22f, 0.15f);
            }
        }
        public static Color4 SaddleBrown {
            get {
                return RGB(0.55f, 0.27f, 0.07f);
            }
        }
        public static Color4 SafetyOrange {
            get {
                return RGB(1f, 0.47f, 0f);
            }
        }
        public static Color4 SafetyOrangeBlazeOrange {
            get {
                return RGB(1f, 0.4f, 0f);
            }
        }
        public static Color4 SafetyYellow {
            get {
                return RGB(0.93f, 0.82f, 0.01f);
            }
        }
        public static Color4 Saffron {
            get {
                return RGB(0.96f, 0.77f, 0.19f);
            }
        }
        public static Color4 Sage {
            get {
                return RGB(0.74f, 0.72f, 0.54f);
            }
        }
        public static Color4 StPatrickSBlue {
            get {
                return RGB(0.14f, 0.16f, 0.48f);
            }
        }
        public static Color4 Salmon {
            get {
                return RGB(0.98f, 0.5f, 0.45f);
            }
        }
        public static Color4 SalmonPink {
            get {
                return RGB(1f, 0.57f, 0.64f);
            }
        }
        public static Color4 Sand {
            get {
                return RGB(0.76f, 0.7f, 0.5f);
            }
        }
        public static Color4 SandDune {
            get {
                return RGB(0.59f, 0.44f, 0.09f);
            }
        }
        public static Color4 SandyBrown {
            get {
                return RGB(0.96f, 0.64f, 0.38f);
            }
        }
        public static Color4 SapGreen {
            get {
                return RGB(0.31f, 0.49f, 0.16f);
            }
        }
        public static Color4 Sapphire {
            get {
                return RGB(0.06f, 0.32f, 0.73f);
            }
        }
        public static Color4 SapphireBlue {
            get {
                return RGB(0f, 0.4f, 0.65f);
            }
        }
        public static Color4 SapphireCrayola {
            get {
                return RGB(0.18f, 0.36f, 0.63f);
            }
        }
        public static Color4 SatinSheenGold {
            get {
                return RGB(0.8f, 0.63f, 0.21f);
            }
        }
        public static Color4 Scarlet {
            get {
                return RGB(1f, 0.14f, 0f);
            }
        }
        public static Color4 SchaussPink {
            get {
                return RGB(1f, 0.57f, 0.69f);
            }
        }
        public static Color4 SchoolBusYellow {
            get {
                return RGB(1f, 0.85f, 0f);
            }
        }
        public static Color4 ScreaminGreen {
            get {
                return RGB(0.4f, 1f, 0.4f);
            }
        }
        public static Color4 SeaGreen {
            get {
                return RGB(0.18f, 0.55f, 0.34f);
            }
        }
        public static Color4 SeaGreenCrayola {
            get {
                return RGB(0f, 1f, 0.8f);
            }
        }
        public static Color4 SealBrown {
            get {
                return RGB(0.2f, 0.08f, 0.08f);
            }
        }
        public static Color4 Seashell {
            get {
                return RGB(1f, 0.96f, 0.93f);
            }
        }
        public static Color4 SelectiveYellow {
            get {
                return RGB(1f, 0.73f, 0f);
            }
        }
        public static Color4 Sepia {
            get {
                return RGB(0.44f, 0.26f, 0.08f);
            }
        }
        public static Color4 Shadow {
            get {
                return RGB(0.54f, 0.47f, 0.36f);
            }
        }
        public static Color4 ShadowBlue {
            get {
                return RGB(0.47f, 0.55f, 0.65f);
            }
        }
        public static Color4 ShamrockGreen {
            get {
                return RGB(0f, 0.62f, 0.38f);
            }
        }
        public static Color4 SheenGreen {
            get {
                return RGB(0.56f, 0.83f, 0f);
            }
        }
        public static Color4 ShimmeringBlush {
            get {
                return RGB(0.85f, 0.53f, 0.58f);
            }
        }
        public static Color4 ShinyShamrock {
            get {
                return RGB(0.37f, 0.65f, 0.47f);
            }
        }
        public static Color4 ShockingPink {
            get {
                return RGB(0.99f, 0.06f, 0.75f);
            }
        }
        public static Color4 ShockingPinkCrayola {
            get {
                return RGB(1f, 0.44f, 1f);
            }
        }
        public static Color4 Sienna {
            get {
                return RGB(0.53f, 0.18f, 0.09f);
            }
        }
        public static Color4 Silver {
            get {
                return RGB(0.75f, 0.75f, 0.75f);
            }
        }
        public static Color4 SilverCrayola {
            get {
                return RGB(0.79f, 0.75f, 0.73f);
            }
        }
        public static Color4 SilverMetallic {
            get {
                return RGB(0.67f, 0.66f, 0.68f);
            }
        }
        public static Color4 SilverChalice {
            get {
                return RGB(0.67f, 0.67f, 0.67f);
            }
        }
        public static Color4 SilverPink {
            get {
                return RGB(0.77f, 0.68f, 0.68f);
            }
        }
        public static Color4 SilverSand {
            get {
                return RGB(0.75f, 0.76f, 0.76f);
            }
        }
        public static Color4 Sinopia {
            get {
                return RGB(0.8f, 0.25f, 0.04f);
            }
        }
        public static Color4 SizzlingRed {
            get {
                return RGB(1f, 0.22f, 0.33f);
            }
        }
        public static Color4 SizzlingSunrise {
            get {
                return RGB(1f, 0.86f, 0f);
            }
        }
        public static Color4 Skobeloff {
            get {
                return RGB(0f, 0.45f, 0.45f);
            }
        }
        public static Color4 SkyBlue {
            get {
                return RGB(0.53f, 0.81f, 0.92f);
            }
        }
        public static Color4 SkyBlueCrayola {
            get {
                return RGB(0.46f, 0.84f, 0.92f);
            }
        }
        public static Color4 SkyMagenta {
            get {
                return RGB(0.81f, 0.44f, 0.69f);
            }
        }
        public static Color4 SlateBlue {
            get {
                return RGB(0.42f, 0.35f, 0.8f);
            }
        }
        public static Color4 SlateGray {
            get {
                return RGB(0.44f, 0.5f, 0.56f);
            }
        }
        public static Color4 SlimyGreen {
            get {
                return RGB(0.16f, 0.59f, 0.09f);
            }
        }
        public static Color4 Smitten {
            get {
                return RGB(0.78f, 0.25f, 0.53f);
            }
        }
        public static Color4 SmokyBlack {
            get {
                return RGB(0.06f, 0.05f, 0.03f);
            }
        }
        public static Color4 Snow {
            get {
                return RGB(1f, 0.98f, 0.98f);
            }
        }
        public static Color4 SolidPink {
            get {
                return RGB(0.54f, 0.22f, 0.26f);
            }
        }
        public static Color4 SonicSilver {
            get {
                return RGB(0.46f, 0.46f, 0.46f);
            }
        }
        public static Color4 SpaceCadet {
            get {
                return RGB(0.11f, 0.16f, 0.32f);
            }
        }
        public static Color4 SpanishBistre {
            get {
                return RGB(0.5f, 0.46f, 0.2f);
            }
        }
        public static Color4 SpanishBlue {
            get {
                return RGB(0f, 0.44f, 0.72f);
            }
        }
        public static Color4 SpanishCarmine {
            get {
                return RGB(0.82f, 0f, 0.28f);
            }
        }
        public static Color4 SpanishGray {
            get {
                return RGB(0.6f, 0.6f, 0.6f);
            }
        }
        public static Color4 SpanishGreen {
            get {
                return RGB(0f, 0.57f, 0.31f);
            }
        }
        public static Color4 SpanishOrange {
            get {
                return RGB(0.91f, 0.38f, 0f);
            }
        }
        public static Color4 SpanishPink {
            get {
                return RGB(0.97f, 0.75f, 0.75f);
            }
        }
        public static Color4 SpanishRed {
            get {
                return RGB(0.9f, 0f, 0.15f);
            }
        }
        public static Color4 SpanishSkyBlue {
            get {
                return RGB(0f, 1f, 1f);
            }
        }
        public static Color4 SpanishViolet {
            get {
                return RGB(0.3f, 0.16f, 0.51f);
            }
        }
        public static Color4 SpanishViridian {
            get {
                return RGB(0f, 0.5f, 0.36f);
            }
        }
        public static Color4 SpringBud {
            get {
                return RGB(0.65f, 0.99f, 0f);
            }
        }
        public static Color4 SpringFrost {
            get {
                return RGB(0.53f, 1f, 0.16f);
            }
        }
        public static Color4 SpringGreen {
            get {
                return RGB(0f, 1f, 0.5f);
            }
        }
        public static Color4 SpringGreenCrayola {
            get {
                return RGB(0.93f, 0.92f, 0.74f);
            }
        }
        public static Color4 StarCommandBlue {
            get {
                return RGB(0f, 0.48f, 0.72f);
            }
        }
        public static Color4 SteelBlue {
            get {
                return RGB(0.27f, 0.51f, 0.71f);
            }
        }
        public static Color4 SteelPink {
            get {
                return RGB(0.8f, 0.2f, 0.8f);
            }
        }
        public static Color4 SteelTeal {
            get {
                return RGB(0.37f, 0.54f, 0.55f);
            }
        }
        public static Color4 StilDeGrainYellow {
            get {
                return RGB(0.98f, 0.85f, 0.37f);
            }
        }
        public static Color4 Straw {
            get {
                return RGB(0.89f, 0.85f, 0.44f);
            }
        }
        public static Color4 Strawberry {
            get {
                return RGB(0.98f, 0.31f, 0.33f);
            }
        }
        public static Color4 StrawberryBlonde {
            get {
                return RGB(1f, 0.58f, 0.38f);
            }
        }
        public static Color4 SugarPlum {
            get {
                return RGB(0.57f, 0.31f, 0.46f);
            }
        }
        public static Color4 Sunglow {
            get {
                return RGB(1f, 0.8f, 0.2f);
            }
        }
        public static Color4 Sunray {
            get {
                return RGB(0.89f, 0.67f, 0.34f);
            }
        }
        public static Color4 Sunset {
            get {
                return RGB(0.98f, 0.84f, 0.65f);
            }
        }
        public static Color4 SuperPink {
            get {
                return RGB(0.81f, 0.42f, 0.66f);
            }
        }
        public static Color4 SweetBrown {
            get {
                return RGB(0.66f, 0.22f, 0.19f);
            }
        }
        public static Color4 SyracuseOrange {
            get {
                return RGB(0.83f, 0.27f, 0f);
            }
        }
        public static Color4 Tan {
            get {
                return RGB(0.82f, 0.71f, 0.55f);
            }
        }
        public static Color4 TanCrayola {
            get {
                return RGB(0.85f, 0.6f, 0.42f);
            }
        }
        public static Color4 Tangerine {
            get {
                return RGB(0.95f, 0.52f, 0f);
            }
        }
        public static Color4 TangoPink {
            get {
                return RGB(0.89f, 0.44f, 0.48f);
            }
        }
        public static Color4 TartOrange {
            get {
                return RGB(0.98f, 0.3f, 0.27f);
            }
        }
        public static Color4 Taupe {
            get {
                return RGB(0.28f, 0.24f, 0.2f);
            }
        }
        public static Color4 TaupeGray {
            get {
                return RGB(0.55f, 0.52f, 0.54f);
            }
        }
        public static Color4 TeaGreen {
            get {
                return RGB(0.82f, 0.94f, 0.75f);
            }
        }
        public static Color4 TeaOrange {
            get {
                return RGB(0.97f, 0.51f, 0.47f);
            }
        }
        public static Color4 TeaRose {
            get {
                return RGB(0.96f, 0.76f, 0.76f);
            }
        }
        public static Color4 Teal {
            get {
                return RGB(0f, 0.5f, 0.5f);
            }
        }
        public static Color4 TealBlue {
            get {
                return RGB(0.21f, 0.46f, 0.53f);
            }
        }
        public static Color4 Telemagenta {
            get {
                return RGB(0.81f, 0.2f, 0.46f);
            }
        }
        public static Color4 TenneTawny {
            get {
                return RGB(0.8f, 0.34f, 0f);
            }
        }
        public static Color4 TerraCotta {
            get {
                return RGB(0.89f, 0.45f, 0.36f);
            }
        }
        public static Color4 Thistle {
            get {
                return RGB(0.85f, 0.75f, 0.85f);
            }
        }
        public static Color4 ThulianPink {
            get {
                return RGB(0.87f, 0.44f, 0.63f);
            }
        }
        public static Color4 TickleMePink {
            get {
                return RGB(0.99f, 0.54f, 0.67f);
            }
        }
        public static Color4 TiffanyBlue {
            get {
                return RGB(0.04f, 0.73f, 0.71f);
            }
        }
        public static Color4 Timberwolf {
            get {
                return RGB(0.86f, 0.84f, 0.82f);
            }
        }
        public static Color4 TitaniumYellow {
            get {
                return RGB(0.93f, 0.9f, 0f);
            }
        }
        public static Color4 Tomato {
            get {
                return RGB(1f, 0.39f, 0.28f);
            }
        }
        public static Color4 TropicalRainforest {
            get {
                return RGB(0f, 0.46f, 0.37f);
            }
        }
        public static Color4 TrueBlue {
            get {
                return RGB(0f, 0.45f, 0.81f);
            }
        }
        public static Color4 TrypanBlue {
            get {
                return RGB(0.11f, 0.02f, 0.7f);
            }
        }
        public static Color4 TuftsBlue {
            get {
                return RGB(0.24f, 0.56f, 0.87f);
            }
        }
        public static Color4 Tumbleweed {
            get {
                return RGB(0.87f, 0.67f, 0.53f);
            }
        }
        public static Color4 Turquoise {
            get {
                return RGB(0.25f, 0.88f, 0.82f);
            }
        }
        public static Color4 TurquoiseBlue {
            get {
                return RGB(0f, 1f, 0.94f);
            }
        }
        public static Color4 TurquoiseGreen {
            get {
                return RGB(0.63f, 0.84f, 0.71f);
            }
        }
        public static Color4 TurtleGreen {
            get {
                return RGB(0.54f, 0.6f, 0.36f);
            }
        }
        public static Color4 Tuscan {
            get {
                return RGB(0.98f, 0.84f, 0.65f);
            }
        }
        public static Color4 TuscanBrown {
            get {
                return RGB(0.44f, 0.31f, 0.22f);
            }
        }
        public static Color4 TuscanRed {
            get {
                return RGB(0.49f, 0.28f, 0.28f);
            }
        }
        public static Color4 TuscanTan {
            get {
                return RGB(0.65f, 0.48f, 0.36f);
            }
        }
        public static Color4 Tuscany {
            get {
                return RGB(0.75f, 0.6f, 0.6f);
            }
        }
        public static Color4 TwilightLavender {
            get {
                return RGB(0.54f, 0.29f, 0.42f);
            }
        }
        public static Color4 TyrianPurple {
            get {
                return RGB(0.4f, 0.01f, 0.24f);
            }
        }
        public static Color4 UaBlue {
            get {
                return RGB(0f, 0.2f, 0.67f);
            }
        }
        public static Color4 UaRed {
            get {
                return RGB(0.85f, 0f, 0.3f);
            }
        }
        public static Color4 Ultramarine {
            get {
                return RGB(0.07f, 0.04f, 0.56f);
            }
        }
        public static Color4 UltramarineBlue {
            get {
                return RGB(0.25f, 0.4f, 0.96f);
            }
        }
        public static Color4 UltraPink {
            get {
                return RGB(1f, 0.44f, 1f);
            }
        }
        public static Color4 UltraRed {
            get {
                return RGB(0.99f, 0.42f, 0.52f);
            }
        }
        public static Color4 Umber {
            get {
                return RGB(0.39f, 0.32f, 0.28f);
            }
        }
        public static Color4 UnbleachedSilk {
            get {
                return RGB(1f, 0.87f, 0.79f);
            }
        }
        public static Color4 UnitedNationsBlue {
            get {
                return RGB(0.36f, 0.57f, 0.9f);
            }
        }
        public static Color4 UniversityOfPennsylvaniaRed {
            get {
                return RGB(0.65f, 0f, 0.13f);
            }
        }
        public static Color4 UnmellowYellow {
            get {
                return RGB(1f, 1f, 0.4f);
            }
        }
        public static Color4 UpForestGreen {
            get {
                return RGB(0f, 0.27f, 0.13f);
            }
        }
        public static Color4 UpMaroon {
            get {
                return RGB(0.48f, 0.07f, 0.07f);
            }
        }
        public static Color4 UpsdellRed {
            get {
                return RGB(0.68f, 0.13f, 0.16f);
            }
        }
        public static Color4 UranianBlue {
            get {
                return RGB(0.69f, 0.86f, 0.96f);
            }
        }
        public static Color4 UsafaBlue {
            get {
                return RGB(0f, 0.31f, 0.6f);
            }
        }
        public static Color4 VanDykeBrown {
            get {
                return RGB(0.4f, 0.26f, 0.16f);
            }
        }
        public static Color4 Vanilla {
            get {
                return RGB(0.95f, 0.9f, 0.67f);
            }
        }
        public static Color4 VanillaIce {
            get {
                return RGB(0.95f, 0.56f, 0.66f);
            }
        }
        public static Color4 VegasGold {
            get {
                return RGB(0.77f, 0.7f, 0.35f);
            }
        }
        public static Color4 VenetianRed {
            get {
                return RGB(0.78f, 0.03f, 0.08f);
            }
        }
        public static Color4 Verdigris {
            get {
                return RGB(0.26f, 0.7f, 0.68f);
            }
        }
        public static Color4 Vermilion1 {
            get {
                return RGB(0.89f, 0.26f, 0.2f);
            }
        }
        public static Color4 Vermilion2 {
            get {
                return RGB(0.85f, 0.22f, 0.12f);
            }
        }
        public static Color4 Veronica {
            get {
                return RGB(0.63f, 0.13f, 0.94f);
            }
        }
        public static Color4 Violet {
            get {
                return RGB(0.56f, 0f, 1f);
            }
        }
        public static Color4 VioletColorWheel {
            get {
                return RGB(0.5f, 0f, 1f);
            }
        }
        public static Color4 VioletCrayola {
            get {
                return RGB(0.59f, 0.24f, 0.5f);
            }
        }
        public static Color4 VioletRyb {
            get {
                return RGB(0.53f, 0f, 0.69f);
            }
        }
        public static Color4 VioletWeb {
            get {
                return RGB(0.93f, 0.51f, 0.93f);
            }
        }
        public static Color4 VioletBlue {
            get {
                return RGB(0.2f, 0.29f, 0.7f);
            }
        }
        public static Color4 VioletBlueCrayola {
            get {
                return RGB(0.46f, 0.43f, 0.78f);
            }
        }
        public static Color4 VioletRed {
            get {
                return RGB(0.97f, 0.33f, 0.58f);
            }
        }
        public static Color4 Viridian {
            get {
                return RGB(0.25f, 0.51f, 0.43f);
            }
        }
        public static Color4 ViridianGreen {
            get {
                return RGB(0f, 0.59f, 0.6f);
            }
        }
        public static Color4 VividBurgundy {
            get {
                return RGB(0.62f, 0.11f, 0.21f);
            }
        }
        public static Color4 VividSkyBlue {
            get {
                return RGB(0f, 0.8f, 1f);
            }
        }
        public static Color4 VividTangerine {
            get {
                return RGB(1f, 0.63f, 0.54f);
            }
        }
        public static Color4 VividViolet {
            get {
                return RGB(0.62f, 0f, 1f);
            }
        }
        public static Color4 Volt {
            get {
                return RGB(0.8f, 1f, 0f);
            }
        }
        public static Color4 WarmBlack {
            get {
                return RGB(0f, 0.26f, 0.26f);
            }
        }
        public static Color4 WeezyBlue {
            get {
                return RGB(0.09f, 0.61f, 0.8f);
            }
        }
        public static Color4 Wheat {
            get {
                return RGB(0.96f, 0.87f, 0.7f);
            }
        }
        public static Color4 White {
            get {
                return RGB(1f, 1f, 1f);
            }
        }
        public static Color4 WildBlueYonder {
            get {
                return RGB(0.64f, 0.68f, 0.82f);
            }
        }
        public static Color4 WildOrchid {
            get {
                return RGB(0.83f, 0.44f, 0.64f);
            }
        }
        public static Color4 WildStrawberry {
            get {
                return RGB(1f, 0.26f, 0.64f);
            }
        }
        public static Color4 WildWatermelon {
            get {
                return RGB(0.99f, 0.42f, 0.52f);
            }
        }
        public static Color4 WindsorTan {
            get {
                return RGB(0.65f, 0.33f, 0.01f);
            }
        }
        public static Color4 Wine {
            get {
                return RGB(0.45f, 0.18f, 0.22f);
            }
        }
        public static Color4 WineDregs {
            get {
                return RGB(0.4f, 0.19f, 0.28f);
            }
        }
        public static Color4 WinterSky {
            get {
                return RGB(1f, 0f, 0.49f);
            }
        }
        public static Color4 WintergreenDream {
            get {
                return RGB(0.34f, 0.53f, 0.49f);
            }
        }
        public static Color4 Wisteria {
            get {
                return RGB(0.79f, 0.63f, 0.86f);
            }
        }
        public static Color4 WoodBrown {
            get {
                return RGB(0.76f, 0.6f, 0.42f);
            }
        }
        public static Color4 Xanadu {
            get {
                return RGB(0.45f, 0.53f, 0.47f);
            }
        }
        public static Color4 Xanthic {
            get {
                return RGB(0.93f, 0.93f, 0.04f);
            }
        }
        public static Color4 Xanthous {
            get {
                return RGB(0.95f, 0.71f, 0.18f);
            }
        }
        public static Color4 YaleBlue {
            get {
                return RGB(0f, 0.21f, 0.42f);
            }
        }
        public static Color4 Yellow {
            get {
                return RGB(1f, 1f, 0f);
            }
        }
        public static Color4 YellowCrayola {
            get {
                return RGB(0.99f, 0.91f, 0.51f);
            }
        }
        public static Color4 YellowMunsell {
            get {
                return RGB(0.94f, 0.8f, 0f);
            }
        }
        public static Color4 YellowNcs {
            get {
                return RGB(1f, 0.83f, 0f);
            }
        }
        public static Color4 YellowPantone {
            get {
                return RGB(1f, 0.87f, 0f);
            }
        }
        public static Color4 YellowProcess {
            get {
                return RGB(1f, 0.94f, 0f);
            }
        }
        public static Color4 YellowRyb {
            get {
                return RGB(1f, 1f, 0.2f);
            }
        }
        public static Color4 YellowGreen {
            get {
                return RGB(0.6f, 0.8f, 0.2f);
            }
        }
        public static Color4 YellowGreenCrayola {
            get {
                return RGB(0.77f, 0.89f, 0.52f);
            }
        }
        public static Color4 YellowGreenColorWheel {
            get {
                return RGB(0.19f, 0.7f, 0.1f);
            }
        }
        public static Color4 YellowOrange {
            get {
                return RGB(1f, 0.68f, 0.26f);
            }
        }
        public static Color4 YellowOrangeColorWheel {
            get {
                return RGB(1f, 0.58f, 0.02f);
            }
        }
        public static Color4 YellowSunshine {
            get {
                return RGB(1f, 0.97f, 0f);
            }
        }
        public static Color4 YinmnBlue {
            get {
                return RGB(0.18f, 0.31f, 0.56f);
            }
        }
        public static Color4 Zaffre {
            get {
                return RGB(0f, 0.08f, 0.66f);
            }
        }
        public static Color4 Zomp {
            get {
                return RGB(0.22f, 0.65f, 0.56f);
            }
        }
        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Datatypes
{
    public struct Rect2D
    {
        public float X0;
        public float Y0;
        public float X1;
        public float Y1;

        public Rect2D(float x0, float y0, float x1, float y1)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
        }

        public float Left { get { return X0 < X1 ? X0 : X1; } }
        public float Right { get { return X0 < X1 ? X1 : X0; } }
        public float Bottom { get { return Y0 < Y1 ? Y0 : Y1; } }
        public float Top { get { return Y0 < Y1 ? Y1 : Y0; } }

        public float CenterX { get { return X0 + (X1 - X0) * 0.5f; } }

        public void Rectify()
        {
            if (X0 > X1)
            {
                float temp = X1;
                X1 = X0;
                X0 = temp;
            }

            if (Y0 > Y1)
            {
                float temp = Y1;
                Y1 = Y0;
                Y0 = temp;
            }
        }

        public float CenterY { get { return Y0 + (Y1 - Y0) * 0.5f; } }

        public float SmallerDimension { get { return Width < Height ? Width : Height; } }

        public float LargerDimension { get { return Width > Height ? Width : Height; } }

        public float Width {
            get {
                return Right - Left;
            }
        }


        public float Height {
            get {
                return Top - Bottom;
            }
        }

        internal bool IsInverted()
        {
            return (X0 > X1 || Y0 > Y1);
        }
    }
}

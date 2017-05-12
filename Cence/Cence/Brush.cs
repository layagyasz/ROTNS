using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public class Brush
    {
        float[,] Matrix;
        int Radius;
        public BlendFilter BlendMode;
        public FloatingColor Color;

        public Brush(int Radius, Interpolator.InterpolatorFunction Interpolator, BlendFilter BlendMode, FloatingColor Color)
        {
            this.Radius = Radius;
            int Width = Radius * 2 + 1;
            this.Color = Color;
            this.BlendMode = BlendMode;
            Matrix = new float[Width, Width];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    int X = i - Radius;
                    int Y = j - Radius;
                    double Dist = Math.Sqrt(X * X + Y * Y) / Radius;
                    if (Dist > 1) Dist = 1;
                    Matrix[i, j] = (float)Interpolator(Dist);
                }
            }
        }

        public void Paint(int X, int Y, FloatingImage Image)
        {
            int Width = Radius * 2 + 1;
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    int x = i - Radius;
                    int y = j - Radius;
                    Image[X + x, Y + y] = BlendMode.Filter(Image[X + x, Y + y], Color * Matrix[i, j]);
                }
            }
        }
    }
}

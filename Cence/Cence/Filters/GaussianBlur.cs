using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    class GaussianBlur : Filter
    {
        float[,] Matrix;
        int Radius;

        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }

        public GaussianBlur(int Radius, float StandardDeviation)
        {
            this.Radius = Radius;
            int Width = Radius * 2 + 1;
            Matrix = new float[Width, Width];
            float Sum = 0;
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    int X = i - Radius;
                    int Y = j - Radius;
                    Matrix[i, j] = (float)(1 / (2 * Math.PI * StandardDeviation * StandardDeviation) *
                        Math.Exp(-(X * X + Y * Y) / (2 * StandardDeviation * StandardDeviation)));
                    Sum += Matrix[i, j];
                }
            }
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    Matrix[i, j] /= Sum;
                }
            }
        }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            int Width = Radius * 2 + 1;
            FloatingColor R = new FloatingColor(0, 0, 0);
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    int x = i - Radius;
                    int y = j - Radius;
                    R = R + Matrix[i, j] * Image[X + x, Y + y];
                }
            }

            return R;
        }
    }
}

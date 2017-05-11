using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities
{
    public class Filter
    {
        double[,] _Filter;
        double _Factor = 1;
        double _Bias = 0;

        public double Factor { get { return _Factor; } }
        public double Bias { get { return _Bias; } }
        public int Height { get { return _Filter.GetLength(1); } }
        public int Width { get { return _Filter.GetLength(0); } }

        public double this[int x, int y] { get { return _Filter[x, y]; } }

        public Filter(double[,] Filter)
        {
            _Filter = Filter;
        }

        public Filter(double[,] Filter, double Factor, double Bias)
            : this(Filter)
        {
            _Factor = Factor;
            _Bias = Bias;
        }

        public float[,] Apply(float[,] Map)
        {
            float[,] R = new float[Map.GetLength(0), Map.GetLength(1)];
            int w = Map.GetLength(0);
            int h = Map.GetLength(1);


            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        for (int y = 0; y < Height; ++y)
                        {
                            int iX = (i - Width / 2 + x + w) % w;
                            int iY = (j - Height / 2 + y + h) % h;
                            R[i,j] += (float)(Map[iX,iY] * this[x, y]);
                        }
                    }
                    R[i,j] = (float)Math.Min(Math.Max(Factor * R[i,j] + Bias, 0), 1);
                }
            }

            return R;
        }
    }
}

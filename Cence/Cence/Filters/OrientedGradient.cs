using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class OrientedGradient : Filter
    {
        public float Bias { get { return .5f; } }
        public float Factor { get { return 1; } }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            FloatingColor C = (Image[X, Y + 1] + Image[X + 1, Y]) - (Image[X - 1, Y] + Image[X, Y - 1]);
            return C;
        }
    }
}

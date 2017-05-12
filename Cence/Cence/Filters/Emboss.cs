using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Emboss : Filter
    {
        public float Bias { get { return .5f; } }
        public float Factor { get { return 1; } }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return (Image[X + 1, Y + 1] + Image[X, Y + 1] + Image[X + 1, Y]) - (Image[X - 1, Y - 1] + Image[X - 1, Y] + Image[X, Y - 1]);
        }
    }
}

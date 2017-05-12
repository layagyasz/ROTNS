using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Sharpen : Filter
    {
        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return 5 * Image[X, Y] - Image[X, Y - 1] - Image[X, Y + 1] - Image[X - 1, Y] - Image[X + 1, Y];
        }
    }
}

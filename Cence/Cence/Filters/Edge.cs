using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Edge : Filter
    {

        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            FloatingColor x = ((Image[X + 1, Y + 1] - Image[X - 1, Y + 1]) + 2 * (Image[X + 1,Y] - Image[X -1, Y]) + (Image[X+1, Y-1] - Image[X-1, Y-1]));
            FloatingColor y =((Image[X - 1, Y - 1] - Image[X - 1, Y + 1]) + 2 * (Image[X, Y - 1] - Image[X, Y + 1]) + (Image[X + 1, Y - 1] - Image[X + 1, Y + 1]));
            x *= x;
            y *= y;

            return new FloatingColor(
                (float)Math.Sqrt(x.R + y.R),
                (float)Math.Sqrt(x.G + y.G),
                (float)Math.Sqrt(x.B + y.B));
        }
    }
}

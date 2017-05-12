using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Clamp : Filter
    {
        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return new FloatingColor()
            {
                R = Math.Min(1, Math.Max(Image[X, Y].R, 0)),
                G = Math.Min(1, Math.Max(Image[X, Y].G, 0)),
                B = Math.Min(1, Math.Max(Image[X, Y].B, 0)),
                A = Math.Min(1, Math.Max(Image[X, Y].A, 0))
            };
        }
    }
}

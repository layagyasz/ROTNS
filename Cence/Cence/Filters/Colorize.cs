using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
   	public  class Colorize : Filter
    {
        public float Bias { get { return 0; } }
        public float Factor { get { return 1; } }

        FloatingColor Color;

        public Colorize(FloatingColor Color)
        {
            this.Color = Color;
        }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return Color * Image[X, Y];
        }
    }
}

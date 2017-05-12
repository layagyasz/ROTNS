using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Contrast : Filter
    {
        public float Amount = 2;

        public float Bias { get { return .5f - Amount * .5f; } }
        public float Factor { get { return Amount; } }

        public Contrast(float Amount)
        {
            this.Amount = Amount;
        }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return Image[X, Y];
        }
    }
}

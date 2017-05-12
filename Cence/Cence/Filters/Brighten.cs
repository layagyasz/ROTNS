using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Brighten : Filter
    {
        float Amount = .5f;

        public float Bias { get { return Amount; } }
        public float Factor { get { return 1; } }

        public Brighten(float Amount)
        {
            this.Amount = Amount;
        }

        public FloatingColor Filter(int X, int Y, FloatingImage Image)
        {
            return Image[X, Y];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Difference : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            return new FloatingColor()
            {
                R = Math.Abs(Base.R - Blend.R),
                G = Math.Abs(Base.G - Blend.G),
                B = Math.Abs(Base.B - Blend.B),
                A = Base.A
            };
        }
    }
}

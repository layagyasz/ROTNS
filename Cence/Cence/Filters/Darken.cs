using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Darken : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            return new FloatingColor()
            {
                R = Math.Min(Base.R, Blend.R),
                G = Math.Min(Base.G, Blend.G),
                B = Math.Min(Base.B, Blend.B),
                A = Base.A
            };
        }
    }
}

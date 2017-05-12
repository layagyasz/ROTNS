using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
   	public  class Lighten : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            return new FloatingColor()
            {
                R = Math.Max(Base.R, Blend.R),
                G = Math.Max(Base.G, Blend.G),
                B = Math.Max(Base.B, Blend.B),
                A = Base.A
            };
        }
    }
}

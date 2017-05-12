using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class PinLight : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            if (Blend.Luminosity() > .5)
                return new FloatingColor()
                {
                    R = Math.Max(Base.R, Blend.R),
                    G = Math.Max(Base.G, Blend.G),
                    B = Math.Max(Base.B, Blend.B),
                    A = Base.A
                };
            else
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

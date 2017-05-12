using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Overlay : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            if (Blend.Luminosity() > .5) return 1 + 2 * Blend * (1 - Base);
            else return 2 * Base * Blend;
        }
    }
}

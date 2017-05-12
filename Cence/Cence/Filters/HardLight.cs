using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class HardLight : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            if (Blend.Luminosity() > .5) return 1 - (1 - Blend) * (-2 * Base);
            else return 2 * Blend * Base;
        }
    }
}

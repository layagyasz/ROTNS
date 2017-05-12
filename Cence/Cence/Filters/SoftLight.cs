using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    class SoftLight : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            if (Blend.Luminosity() > .5) return 1 - (1 - Blend) * (1.5f - Base);
            else return Blend * (Base + .5f);
        }
    }
}

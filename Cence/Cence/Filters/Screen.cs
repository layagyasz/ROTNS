using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Screen : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            return 1 - (1 - Blend) * (1 - Base);
        }
    }
}

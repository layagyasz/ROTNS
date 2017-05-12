using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Exclusion : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            return .5f - 2 * (Base - .5f) * (Blend - .5f);
        }
    }
}

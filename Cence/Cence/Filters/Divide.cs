using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence.Filters
{
    public class Divide : BlendFilter
    {
        public FloatingColor Filter(FloatingColor Base, FloatingColor Blend)
        {
            return Base / Blend;
        }
    }
}

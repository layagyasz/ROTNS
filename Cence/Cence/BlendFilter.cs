using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public interface BlendFilter
    {
        FloatingColor Filter(FloatingColor Base, FloatingColor Blend);
    }
}

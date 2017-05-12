using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public interface Filter
    {
        float Bias { get; }
        float Factor { get; }
        FloatingColor Filter(int X, int Y, FloatingImage Image);
    }
}

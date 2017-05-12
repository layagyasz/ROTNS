using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.KDT
{
    public interface HyperGon<T> where T : IMultiDimensionComparable
    {
        bool Contains(T Point);
        bool Contains(HyperRect<T> Rect);
        bool Intersects(HyperRect<T> Rect);
    }
}

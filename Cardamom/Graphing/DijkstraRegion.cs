using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    public interface DijkstraRegion<T>
    {
        T Center { get; }
        double StartDistance { get; }
        void Add(T Value);
    }
}

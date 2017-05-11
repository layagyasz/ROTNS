using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    public interface Pathable<T>
    {
        bool Passable { get; }
        double DistanceTo(T Node);
        double HeuristicDistanceTo(T Node);
        IEnumerable<T> Neighbors();
    }
}

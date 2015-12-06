using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    public interface Pathable
    {
        bool Passable { get; }
        double DistanceTo(Pathable Node);
        IEnumerator<Pathable> Neighbors();
    }
}

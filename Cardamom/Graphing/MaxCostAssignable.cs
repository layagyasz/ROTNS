using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Graphing
{
    public interface MaxCostAssignable
    {
        double DistanceTo(MaxCostAssignable Node);
        IEnumerable<MaxCostAssignable> Neighbors();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROTNS.Core
{
    public abstract class Administration
    {
        Agent _Agent;

        public Agent Agent { get { return _Agent; } set { _Agent = value; } }
    }
}

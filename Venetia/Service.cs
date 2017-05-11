using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Venetia
{
    public class Service : Tangible
    {
        public Service(string Name, double Coefficient, double Exponent)
            : base(Name, Coefficient, Exponent) { }

        public Service(string Name, double Coefficient, double Exponent, double Minimum)
            : base(Name, Coefficient, Exponent, Minimum) { }

        public Service(ParseBlock Block)
            : base(Block) { }
    }
}

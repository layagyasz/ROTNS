using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Venetia
{
    public class Resource : Tangible
    {
        public Resource(string Name, double Coefficient, double Exponent, double Minimum)
            : base(Name, Coefficient, Exponent,1,  Minimum) { }

        public Resource(string Name, double Coefficient, double Exponent)
            : base(Name, Coefficient, Exponent, 1) { }

        public Resource(ParseBlock Block)
            : base(Block) { }
    }
}

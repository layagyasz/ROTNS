using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Venetia
{
    public class Good : Tangible
    {
        public Good(string Name, double Coefficient, double Exponent)
            : base(Name, Coefficient, Exponent) { }

        public Good(string Name, double Coefficient, double Exponent, double Minimum)
            : base(Name, Coefficient, Exponent, Minimum) { }

        public Good(ParseBlock Block)
            : base(Block) { }
    }
}

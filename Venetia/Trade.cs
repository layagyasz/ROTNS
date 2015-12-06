using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;

namespace Venetia
{
    public class Trade
    {
        Good _Good1;
        Good _Good2;
        double _Amount1;
        double _Amount2;
        Zone _Zone1;
        Zone _Zone2;

        public Good Good1 { get { return _Good1; } }
        public Good Good2 { get { return _Good2; } }
        public double Amount1 { get { return _Amount1; } }
        public double Amount2 { get { return _Amount2; } }
        public Zone Zone1 { get { return _Zone1; } }
        public Zone Zone2 { get { return _Zone2; } }

        public double Flow1 { get { return _Amount1 * _Zone1.SupplyPrice(_Good1, -_Amount1) + _Amount2 * _Zone1.SupplyPrice(_Good2, _Amount2); } }
        public double Flow2 { get { return _Amount1 * _Zone2.SupplyPrice(_Good1, _Amount1) + _Amount2 * _Zone2.SupplyPrice(_Good2, -_Amount2); } }
        public double Profit { get { return _Amount2 * _Zone1.SupplyPrice(_Good2, _Amount2) - _Amount1 * _Zone1.SupplyPrice(_Good1, -Amount1); } }

        public Trade(Good Good1, Good Good2, double Amount1, double Amount2, Zone Zone1, Zone Zone2)
        {
            _Good1 = Good1;
            _Good2 = Good2;
            _Amount1 = Amount1;
            _Amount2 = Amount2;
            _Zone1 = Zone1;
            _Zone2 = Zone2;
        }
    }
}

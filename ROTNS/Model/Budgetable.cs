using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROTNS.Model
{
    abstract class Budgetable
    {
        protected double _Spending;

        public double Spending { get { return _Spending; } set { _Spending = value; } }
    }
}

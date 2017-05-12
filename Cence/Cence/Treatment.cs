using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public class Treatment
    {
        public delegate double TreatmentFunction(double Value);

        public static readonly TreatmentFunction Billow = Math.Abs;

        public static readonly TreatmentFunction Rig = Value => -Math.Abs(Value);

        public static readonly TreatmentFunction SemiRig = Value => Math.Min(0, Value);

        public static readonly TreatmentFunction None = Value => Value;
    }
}

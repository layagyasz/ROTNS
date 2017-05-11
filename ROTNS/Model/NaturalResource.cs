using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Venetia;
using Cardamom.Serialization;

namespace ROTNS.Model
{
    public class NaturalResource : Resource
    {
        bool _Noisy;
        Func<float, MicroRegion, float> _Distributor;

        public bool Noisy { get { return _Noisy; } }

        public NaturalResource(string Name, double Coeffient, double Exponent, double Minimum, bool Noisy, Func<float, MicroRegion, float> Distributor)
            : base(Name, Coeffient, Exponent, Minimum)
        {
            _Noisy = Noisy;
            _Distributor = Distributor;
        }

        public NaturalResource(string Name, double Coefficient, double Exponent, bool Noisy, Func<float, MicroRegion, float> Distributor)
            : this(Name, Coefficient, Exponent, 0, Noisy, Distributor) { }

        public float Distribute(float Noise, MicroRegion Region)
        {
            return _Distributor.Invoke(Noise, Region);
        }
    }
}

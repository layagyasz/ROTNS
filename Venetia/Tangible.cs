using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace Venetia
{
    public class Tangible
    {
        string _Name;
        double _Coefficient;
        double _Exponent;
        double _Minimum;
        double _Decay = 1;

        public string Name { get { return _Name; } set { _Name = value; } }
        public double Coefficient { get { return _Coefficient; } set { _Coefficient = value; } }
        public double Exponent { get { return _Exponent; } set { _Exponent = value; } }
        public double Minimum { get { return _Minimum; } set { _Minimum = value; } }
        public double Decay { get { return _Decay; } set { _Decay = value; } }

        public Tangible(string Name, double Coefficient, double Exponent) { _Name = Name; _Coefficient = Coefficient; _Exponent = Exponent; }

        public Tangible(string Name, double Coefficient, double Exponent, double Decay)
            : this(Name, Coefficient, Exponent) { _Decay = Decay; }

        public Tangible(string Name, double Coefficient, double Exponent, double Decay, double Minimum)
            : this(Name, Coefficient, Exponent, Decay) { _Minimum = Minimum; }

        public Tangible(ParseBlock Block)
        {
            _Name = Block.Name;
            string[] def = Block.String.Split(' ');
            _Coefficient = Convert.ToDouble(def[0], System.Globalization.CultureInfo.InvariantCulture);
            _Exponent = Convert.ToDouble(def[1], System.Globalization.CultureInfo.InvariantCulture);
            if(def.Length > 2) _Decay = Convert.ToDouble(def[2], System.Globalization.CultureInfo.InvariantCulture);
            if(def.Length > 3) _Minimum = Convert.ToDouble(def[3], System.Globalization.CultureInfo.InvariantCulture);

            Console.WriteLine(this);
        }

        public override string ToString()
        {
            return String.Format("{0}: {1} {2} {3} {4}", _Name, _Coefficient, _Exponent, _Decay, _Minimum);
        }
    }
}

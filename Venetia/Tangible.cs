using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;
using Cardamom.Utilities;

namespace Venetia
{
	public class Tangible
	{
		private enum Attribute { NAME, COEFFICIENT, EXPONENT, MINIMUM, DECAY };

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
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_Name = (string)attributes[(int)Attribute.NAME];
			_Coefficient = (double)attributes[(int)Attribute.COEFFICIENT];
			_Exponent = (double)attributes[(int)Attribute.EXPONENT];
			_Decay = (double)attributes[(int)Attribute.DECAY];
			_Minimum = (double)attributes[(int)Attribute.MINIMUM];
		}

		public override string ToString()
		{
			return String.Format("{0}: {1} {2} {3} {4}", _Name, _Coefficient, _Exponent, _Decay, _Minimum);
		}
	}
}

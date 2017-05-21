using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII
{
	public class SoundClass
	{
		private enum Attribute { FREQUENCY, IDENTIFIERS };

		double _Frequency;
		string[] _Identifiers;

		public double Frequency { get { return _Frequency; } set { _Frequency = value; } }
		public string[] Identifiers { get { return _Identifiers; } set { _Identifiers = value; } }

		public SoundClass(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_Frequency = (double)attributes[(int)Attribute.FREQUENCY];
			_Identifiers = (string[])attributes[(int)Attribute.IDENTIFIERS];
		}
	}
}

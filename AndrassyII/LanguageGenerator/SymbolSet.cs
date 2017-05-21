using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII.LanguageGenerator
{
	class SymbolSet
	{
		private enum Attribute { FREQUENCY, SYMBOLS }

		List<Matched> _Symbols = new List<Matched>();
		double _Frequency;

		public List<Matched> Symbols { get { return _Symbols; } }
		public double Frequency { get { return _Frequency; } set { _Frequency = value; } }

		public SymbolSet() { }

		public SymbolSet(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_Frequency = (double)attributes[(int)Attribute.FREQUENCY];
			_Symbols = (List<Matched>)attributes[(int)Attribute.SYMBOLS];
		}
	}
}

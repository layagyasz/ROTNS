using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII.LanguageGenerator
{
    class SymbolSet
    {
        List<Matched> _Symbols = new List<Matched>();
        double _Frequency;
        double _Drop;

        public List<Matched> Symbols { get { return _Symbols; } }
        public double Frequency { get { return _Frequency; } set { _Frequency = value; } }
        public double Drop { get { return _Drop; } set { _Drop = value; foreach (Matched M in _Symbols) M.Drop = value; } }

        public SymbolSet() { }

        public SymbolSet(ParseBlock Block)
        {
            string[] def = Block.Name.Split(':');
            _Frequency = Convert.ToDouble(def[0], System.Globalization.CultureInfo.InvariantCulture);
            _Drop = Convert.ToDouble(def[1], System.Globalization.CultureInfo.InvariantCulture);
            foreach (ParseBlock B in Block.Break())
            {
                Matched M = new Matched(new Sound(B));
                M.Drop = _Drop;
                _Symbols.Add(M);
            }
        }
    }
}

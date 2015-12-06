using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII
{
    public class Sound : Generator<Sound>
    {
        string _Sound;
        string[] _Identifiers;
        double _Frequency;

        public double Frequency { get { return _Frequency; } }
        public string[] Identifiers { get { return _Identifiers; } }

        public Sound(ParseBlock Block)
        {
            string[] head = Block.Name.Split('*');
            if (head.Length == 2)
            {
                _Sound = head[1].Trim();
                _Frequency = Convert.ToDouble(head[0], System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                _Sound = head[0];
                _Frequency = 1;
            }
            _Identifiers = Block.String.Split(':');
            for (int i = 0; i < _Identifiers.Length; ++i) _Identifiers[i] = _Identifiers[i].Trim();
        }

        public bool HasIdentifier(string Identifier) { return _Identifiers.Contains(Identifier); }

        public Generated<Sound> Generate(Random Random)
        {
            return new Generated<Sound>(this);
        }

        public override string ToString()
        {
            return _Sound;
        }
    }
}

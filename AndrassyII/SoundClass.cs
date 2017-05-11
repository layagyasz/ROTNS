using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII
{
    public class SoundClass
    {
        double _Frequency;
        string[] _Identifiers;

        public double Frequency { get { return _Frequency; } set { _Frequency = value; } }
        public string[] Identifiers { get { return _Identifiers; } set { _Identifiers = value; } }

        public SoundClass(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.Trim().ToLower())
                {
                    case "frequency": _Frequency = Convert.ToDouble(B.String, System.Globalization.CultureInfo.InvariantCulture); break;
                    case "identifiers": _Identifiers = B.String.Split(' '); break;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities.StableMatching;

namespace AndrassyII.LanguageGenerator
{
    class ModifiedMatched : Matched
    {
        Matched _Base;
        Matched _Modifier;

        public Matched Base { get { return _Base; } }
        public Matched Modifier { get { return _Modifier; } }

        public ModifiedMatched(Matched Base, Matched Modifier)
        {
            _Base = Base;
            _Modifier = Modifier;
        }

        public override double GetPreference(Matched Matched)
        {
            return (base.GetPreference(Matched) + _Base.GetPreference(Matched)) / 2 - .1;
        }
    }
}

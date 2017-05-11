using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndrassyII
{
    public class Word : Generated<Sound>
    {
        string _Orthography;

        public string Orthography { get { return _Orthography; } internal set { _Orthography = value; } }

        internal Word(Generated<Sound> Generated, Random Random, List<PrintRule<Sound>> PrintRules)
        {
            foreach (Sound S in Generated) _Sounds.Add(S);
        }
    }
}

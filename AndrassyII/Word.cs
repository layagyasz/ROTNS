using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndrassyII
{
    public class Word : Generated<Sound>
    {
        public Word(Generated<Sound> Generated)
        {
            foreach (Sound S in Generated) _Sounds.Add(S);
        }

        public override string ToString()
        {
            string R = "";
            foreach (Sound S in _Sounds) R += S.ToString();
            return R;
        }
    }
}

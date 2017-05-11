using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities
{
    public struct Pair<T, K>
    {
        T _First;
        K _Second;

        public T First { get { return _First; } set { _First = value; } }
        public K Second { get { return _Second; } set { _Second = value; } }

        public Pair(T First, K Second)
        {
            _First = First;
            _Second = Second;
        }
    }
}

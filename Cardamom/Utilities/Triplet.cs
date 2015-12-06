using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities
{
    public struct Triplet<T, K, L>
    {
        T _First;
        K _Second;
        L _Third;

        public T First { get { return _First; } set { _First = value; } }
        public K Second { get { return _Second; } set { _Second = value; } }
        public L Third { get { return _Third; } set { _Third = value; } }

        public Triplet(T First, K Second, L Third)
        {
            _First = First;
            _Second = Second;
            _Third = Third;
        }
    }
}

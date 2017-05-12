using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
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

        public override string ToString()
        {
            return String.Format("[Pair]First={0},Second={1}", _First, _Second);
        }

        public override bool Equals(object obj)
        {
            Pair<T, K> P = (Pair<T, K>)obj;
            return _First.Equals(P.First) && _Second.Equals(P.Second);
        }

        public override int GetHashCode()
        {
            return unchecked(_First.GetHashCode() * 31 + _Second.GetHashCode());
        }
    }
}

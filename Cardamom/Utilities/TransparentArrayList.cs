using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities
{
    public class TransparentArrayList<T> : IEnumerable<T>
    {
        T[] _Values;
        int _Length;
        int _Alloc;

        public int Length { get { return _Length; } }
        public T[] Values { get { return _Values; } }

        public IEnumerator<T> GetEnumerator() { for (int i = 0; i < _Length; ++i) yield return _Values[i]; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TransparentArrayList()
        {
            _Length = 0;
            _Alloc = 1;
            _Values = new T[1];
        }

        public void Clear() { _Length = 0; }

        public void Add(T Value)
        {
            if (_Length + 1 == _Alloc)
            {
                _Alloc *= 2;
                T[] newA = new T[_Alloc];
                double[] newK = new double[_Alloc];
                Array.Copy(_Values, newA, _Values.Length);
                _Values = newA;
            }
            _Values[_Length] = Value;
            ++_Length;
        }
    }
}

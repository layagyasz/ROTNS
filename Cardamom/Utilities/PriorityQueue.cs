using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities
{
    class PriorityQueue<T, K> : Heap<T, K> where K : IComparable
    {
        public void Remove(T Value)
        {
            int i;
            for (i = 0; i < _Length; ++i)
            {
                if (_Values[i].First.Equals(Value)) break;
            }
            if (i < _Length)
            {
                _Length--;
                if (_Length == -1) _Length = 0;
                _Values[i] = _Values[_Length];
                if (_Length == _Values.Length / 4 && _Length > 1)
                {
                    Pair<T, K>[] R = new Pair<T, K>[_Length];
                    for (int j = 0; j < _Length; ++j) R[j] = _Values[j];
                    _Values = R;
                }
                HeapifyDown(i);
            }
        }

        private Pair<T, K> RemoveItem(T Item)
        {
            int i;
            for (i = 0; i < _Length; ++i)
            {
                if (_Values[i].First.Equals(Item)) break;
            }
            Pair<T, K> E = _Values[i];

            if (i < _Length)
            {
                _Length--;
                if (_Length == -1) _Length = 0;
                _Values[i] = _Values[_Length];
                if (_Length == _Values.Length / 4)
                {
                    Pair<T, K>[] R = new Pair<T, K>[_Length];
                    for (int j = 0; j < _Length; ++j) R[j] = _Values[j];
                    _Values = R;
                }
                HeapifyDown(i);
            }
            return E;
        }

        public void UpdatePriority(T Value, K Priority)
        {
            Pair<T, K> E = RemoveItem(Value);
            E.Second = Priority;
            Push(E);
        }
    }
}

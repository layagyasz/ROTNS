using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities
{
    public class Heap<T, K> where K : IComparable
    {
        protected Pair<T, K>[] _Values;
        protected int _Length;

        public int Count { get { return _Length; } }

        public Heap()
        {
            _Values = new Pair<T,K>[1];
        }

        public void Push(T Value, K Priority)
        {
            Push(new Pair<T, K>(Value, Priority));
        }

        protected void Push(Pair<T, K> Pair)
        {
            if (_Length == _Values.Length)
            {
                Pair<T, K>[] R = new Pair<T, K>[_Length * 2];
                for (int i = 0; i < _Length; ++i) R[i] = _Values[i];
                _Values = R;
            }
            _Values[_Length] = Pair;

            HeapifyUp(_Length);
            _Length++;
        }

        public T Peek() { return _Values[0].First; }

        public T Pop()
        {
            T v = _Values[0].First;
            _Length--;
            if (_Length == -1) _Length = 0;
            _Values[0] = _Values[_Length];
            if (_Length == _Values.Length / 4 && _Length > 1)
            {
                Pair<T,K>[] R = new Pair<T,K>[_Length];
                for (int i = 0; i < _Length; ++i) R[i] = _Values[i];
                _Values = R;
            }
            HeapifyDown(0);

            return v;
        }

        private void Swap(int i1, int i2)
        {
            Pair<T,K> temp = _Values[i1];
            _Values[i1] = _Values[i2];
            _Values[i2] = temp;
        }

        protected void HeapifyDown(int Index)
        {
            int c1 = Index * 2 + 1;
            int c2 = Index * 2 + 2;
            if (c1 >= _Length && c2 >= _Length) return;
            else if (c1 >= _Length && c2 < _Length)
            {
                if (_Values[c2].Second.CompareTo(_Values[Index].Second) < 0) { Swap(c2, Index); HeapifyDown(c2); }
            }
            else if (c1 < _Length && c2 >= _Length)
            {
                if (_Values[c1].Second.CompareTo(_Values[Index].Second) < 0) { Swap(c1, Index); HeapifyDown(c1); }
            }
            else
            {
                if (_Values[c1].Second.CompareTo(_Values[Index].Second) < 0 && _Values[c2].Second.CompareTo(_Values[Index].Second) >= 0) { Swap(c1, Index); HeapifyDown(c1); }
                else if (_Values[c1].Second.CompareTo(_Values[Index].Second) >= 0 && _Values[c2].Second.CompareTo(_Values[Index].Second) < 0) { Swap(c2, Index); HeapifyDown(c2); }
                else
                {
                    int i = c1;
                    if (_Values[c1].Second.CompareTo(_Values[c2].Second) < 0) i = c1;
                    else i = c2;
                    Swap(i, Index);
                    HeapifyDown(i);
                }
            }
        }

        protected void HeapifyUp(int Index)
        {
            int p = (Index - 1) / 2;
            if (_Values[Index].Second.CompareTo(_Values[p].Second) < 0)
            {
                Swap(Index, p);
                HeapifyUp(p);
            }
        }
    }
}

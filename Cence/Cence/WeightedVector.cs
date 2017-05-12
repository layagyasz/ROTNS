using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public class WeightedVector<T> : IEnumerable<T>
    {
        T[] _Values;
        double[] _Keys;
        double _Total;

        int _Length;
        public int Length { get { return _Length; } }
        int _Alloc;

        public WeightedVector()
        {
            _Length = 0;
            _Alloc = 1;
            _Values = new T[1];
            _Keys = new double[1];
        }

        public WeightedVector(WeightedVector<T> Copy)
        {
            this._Length = Copy._Length;
            this._Alloc = Copy._Alloc;
            this._Values = new T[_Alloc];
            this._Keys = new double[_Alloc];
            this._Total = Copy._Total;
            for (int i = 0; i < _Alloc; i++)
            {
                this._Keys[i] = Copy._Keys[i];
                this._Values[i] = Copy._Values[i];
            }
        }

        public IEnumerator<T> GetEnumerator() { for (int i = 0; i < _Length; ++i) yield return _Values[i]; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(double Weight, T Value)
        {
            if (_Length + 1 == _Alloc)
            {
                _Alloc *= 2;
                T[] newA = new T[_Alloc];
                double[] newK = new double[_Alloc];
                Array.Copy(_Values, newA, _Values.Length);
                Array.Copy(_Keys, newK, _Keys.Length);
                _Values = newA;
                _Keys = newK;
            }
            _Total += Weight;
            _Values[_Length] = Value;
            _Keys[_Length + 1] = _Keys[_Length] + Weight;
            ++_Length;
        }

        public void Add(WeightedVector<T> Vector)
        {
        }

        private int IndexOf(double V)
        {
            if (_Length == 0) throw new Exception("Index on Empty WeightVector<T>");
            if (_Length == 1) return 0;
            int i = 0;
            int j = _Length - 1;
            int c = (j + i) / 2;
            while (j - i > 1)
            {
                if (V > _Keys[c])
                {
                    i = c;
                    c = (j + i) / 2;
                }
                else if (V < _Keys[c])
                {
                    j = c;
                    c = (j + i) / 2;
                }
                else break;
            }
            if (j - i == 1 && _Keys[c] > V) c--;
            else if (j - i == 1 && _Keys[c + 1] < V) c++;
            return c;
        }

        public T this[double V]
        {
            get
            {
                return _Values[IndexOf(V * _Total)];
            }
            set
            {
                _Values[IndexOf(V * _Total)] = value;
            }
        }
    }
}

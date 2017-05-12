using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.KDT
{
    public class KV<T, K> : IMultiDimensionComparable where T : IMultiDimensionComparable
    {
        T _First;
        K _Second;

        public int Dimensions { get { return _First.Dimensions; } }
        public T First { get { return _First; } set { _First = value; } }
        public K Second { get { return _Second; } set { _Second = value; } }

        public KV(T First, K Second)
        {
            _First = First;
            _Second = Second;
        }

        public IComparable GetDimension(int Dimension) { return _First.GetDimension(Dimension); }

        public void SetDimension(int Dimension, IComparable Value) { _First.SetDimension(Dimension, Value); }

        public IMultiDimensionComparable Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IMultiDimensionComparable Obj, int Dimension)
        {
            return _First.CompareTo(((KV<T, K>)Obj).First, Dimension);
        }

        public double DistanceSquared(IMultiDimensionComparable Obj)
        {
            return _First.DistanceSquared(Obj);
        }

        public double Subtract(IMultiDimensionComparable Obj, int Dimension)
        {
            return _First.Subtract(Obj, Dimension);
        }
    }
}

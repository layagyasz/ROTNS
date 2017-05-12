using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.KDT
{
    public interface IMultiDimensionComparable
    {
        int Dimensions { get; }
        int CompareTo(IMultiDimensionComparable Obj, int Dimension);
        IComparable GetDimension(int Dimension);
        void SetDimension(int Dimension, IComparable Value);
        IMultiDimensionComparable Clone();
        double DistanceSquared(IMultiDimensionComparable Obj);
        double Subtract(IMultiDimensionComparable Obj, int Dimension);
    }
}

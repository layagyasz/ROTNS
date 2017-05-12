using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.KDT
{
    public struct HyperSphere<T> : HyperGon<T> where T : IMultiDimensionComparable
    {
        T _Center;
        double _RadiusSquared;

        public T Center { get { return _Center; } set { _Center = value; } }
        public double RadiusSquared { get { return _RadiusSquared; } set { _RadiusSquared = value; } }

        public HyperSphere(T Center, double RadiusSquared)
        {
            _Center = Center;
            _RadiusSquared = RadiusSquared;
        }

        public bool Contains(T Point)
        {
            return _Center.DistanceSquared(Point) <= _RadiusSquared;
        }

        public bool Intersects(HyperRect<T> Rect)
        {
            double Sum = 0;
            for (int i = 0; i < Rect.Left.Dimensions; ++i)
            {
                if (_Center.CompareTo(Rect.Left, i) < 0)
                {
                    double S = _Center.Subtract(Rect.Left, i);
                    Sum += S * S;
                }
                else if (_Center.CompareTo(Rect.Right, i) > 0)
                {
                    double S = _Center.Subtract(Rect.Right, i);
                    Sum += S * S;
                }
            }
            return Sum <= _RadiusSquared;
        }

        public bool Contains(HyperRect<T> Rect)
        {
            return _Center.DistanceSquared(Rect.Left) <= _RadiusSquared &&
                _Center.DistanceSquared(Rect.Right) <= _RadiusSquared;
        }
    }
}

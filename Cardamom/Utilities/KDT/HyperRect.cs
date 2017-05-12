using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.KDT
{
    public struct HyperRect<T> : HyperGon<T> where T : IMultiDimensionComparable
    {
        T _Left;
        T _Right;

        public T Left { get { return _Left; } }
        public T Right { get { return _Right; } }

        public HyperRect(T Left, T Right)
        {
            _Left = Left;
            _Right = Right;
        }

        public void Normalize()
        {
            for (int i = 0; i < _Left.Dimensions; ++i)
            {
                if (_Left.CompareTo(_Right, i) > 0)
                {
                    IComparable temp = _Left.GetDimension(i);
                    _Left.SetDimension(i, _Right.GetDimension(i));
                    _Right.SetDimension(i, temp);
                }
            }
        }

        public Pair<HyperRect<T>, HyperRect<T>> Split(IMultiDimensionComparable Point, int Dimension)
        {
            T NewRight = (T)_Right.Clone();
            NewRight.SetDimension(Dimension, Point.GetDimension(Dimension));

            T NewLeft = (T)_Left.Clone();
            NewLeft.SetDimension(Dimension, Point.GetDimension(Dimension));

            return new Pair<HyperRect<T>, HyperRect<T>>(
                new HyperRect<T>(_Left, NewRight),
                new HyperRect<T>(NewLeft, _Right));
        }

        public HyperRect<T> Join(HyperRect<T> Second)
        {
            T L = Left;
            T R = Second.Right;

            for (int i = 0; i < L.Dimensions; ++i)
            {
                L.SetDimension(i,
                    Left.CompareTo(Second.Left, i) < 0 ? Left.GetDimension(i) : Second.Left.GetDimension(i));
                R.SetDimension(i,
                    Right.CompareTo(Second.Left, i) > 0 ? Right.GetDimension(i) : Second.Right.GetDimension(i));
            }
            return new HyperRect<T>(L, R);
        }

        public bool Contains(T Point)
        {
            for (int i = 0; i < _Left.Dimensions; ++i)
            {
                if (_Left.CompareTo(Point, i) > 0 || _Right.CompareTo(Point, i) < 0) return false;
            }
            return true;
        }

        public bool Intersects(HyperRect<T> Rect)
        {
            for (int i = 0; i < _Left.Dimensions; ++i)
            {
                if (_Left.CompareTo(Rect._Right, i) > 0 || Rect._Left.CompareTo(_Right, i) > 0) return false;
            }
            return true;
        }

        public bool Contains(HyperRect<T> Rect)
        {
            for (int i = 0; i < _Left.Dimensions; ++i)
            {
                if (Rect._Left.CompareTo(_Left, i) < 0 || Rect._Right.CompareTo(_Right, i) > 0) return false;
            }
            return true;
        }

        public static HyperRect<A> Bound<A>(IEnumerable<A> Points) where A : IMultiDimensionComparable
        {
            A Left = (A)Points.First().Clone();
            A Right = (A)Points.First().Clone();
            for (int i = 0; i < Left.Dimensions; ++i)
            {
                Left.SetDimension(i, Points.Min(P => P.GetDimension(i)));
                Right.SetDimension(i, Points.Max(P => P.GetDimension(i)));
            }
            return new HyperRect<A>(Left, Right);
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", _Left, _Right);
        }
    }
}

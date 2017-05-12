using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.KDT
{
    public class KDTree<T, K> where T : IMultiDimensionComparable
    {
        KV<T, K> _Value;
        KDTree<T, K> _Left;
        KDTree<T, K> _Right;
        HyperRect<T> _Region;

        public KV<T, K> Value { get { return _Value; } }

        public KDTree(IEnumerable<KV<T, K>> KeysAndValues)
        {
            KV<T, K>[] V = KeysAndValues.ToArray();
            Shuffle(V);
            Build(V, 0, V.Length - 1, HyperRect<T>.Bound(V.Select(i => i.First)), 0);
        }

        public KDTree(KV<T, K>[] KeysAndValues, int Low, int High, HyperRect<T> Region, int Depth)
        {
            Build(KeysAndValues, Low, High, Region, Depth);
        }

        private void Build(KV<T, K>[] KeysAndValues, int Low, int High, HyperRect<T> Region, int Depth)
        {

            int Dimension = Depth % KeysAndValues[Low].Dimensions;
            int N = Ninther(KeysAndValues, Low, High, Dimension);
            _Value = KeysAndValues[N];
            _Region = Region;

            Swap(KeysAndValues, N, High);
            int j = Low;
            for (int i = Low; i < High; ++i)
            {
                if (KeysAndValues[i].CompareTo(KeysAndValues[High], Dimension) < 0) Swap(KeysAndValues, i, j++);
            }
            Pair<HyperRect<T>, HyperRect<T>> Subregions = _Region.Split(_Value.First, Dimension);
            if (Low <= j - 1) _Left = new KDTree<T, K>(KeysAndValues, Low, j - 1, Subregions.First, Depth + 1);
            if (j <= High - 1) _Right = new KDTree<T, K>(KeysAndValues, j, High - 1, Subregions.Second, Depth + 1);
        }

        public IEnumerable<KV<T, K>> Traverse()
        {
            yield return _Value;
            if (_Left != null) foreach (KV<T, K> Item in _Left.Traverse()) yield return Item;
            if (_Right != null) foreach (KV<T, K> Item in _Right.Traverse()) yield return Item;
        }

        public IEnumerable<KV<T, K>> Traverse(HyperGon<T> Range)
        {
            if (Range.Contains(_Region)) foreach (KV<T, K> Item in Traverse()) yield return Item;
            else
            {
                if (_Left != null && Range.Intersects(_Left._Region)) foreach (KV<T, K> Item in _Left.Traverse(Range)) yield return Item;
                if (_Right != null && Range.Intersects(_Right._Region)) foreach (KV<T, K> Item in _Right.Traverse(Range)) yield return Item;
            }
        }

        public KDTree<T, K> GetSubtree(T Key)
        {
            return GetSubtree(Key, 0);
        }

        private KDTree<T, K> GetSubtree(T Key, int Depth)
        {
            int Compare = _Value.CompareTo(Key, Depth % Key.Dimensions);
            if (Compare == 0) return this;
            else if (Compare < 0) return _Left != null ? _Left.GetSubtree(Key, Depth + 1) : null;
            else return _Right != null ? _Right.GetSubtree(Key, Depth + 1) : null;
        }

        public KDTree<T, K> GetClosestSubtree(T Key)
        {
            return GetClosestSubtree(new HyperSphere<T>(Key, _Value.First.DistanceSquared(Key) + 1), 0);
        }

        private KDTree<T, K> GetClosestSubtree(HyperSphere<T> Bound, int Depth)
        {
            KDTree<T, K> Current = null;

            double D = _Value.First.DistanceSquared(Bound.Center);
            if (D < Bound.RadiusSquared)
            {
                Bound.RadiusSquared = D;
                Current = this;
            }
            else if (_Left == null && _Right == null) return null;

            int Compare = _Value.First.CompareTo(Bound.Center, Depth % Bound.Center.Dimensions);
            if (Compare == 0) Current = this;
            else if (Compare > 0)
            {
                if (_Left != null)
                {
                    KDTree<T, K> ClosestLeft = _Left.GetClosestSubtree(Bound, Depth + 1);
                    if (ClosestLeft != null) Current = ClosestLeft;
                }

                if (_Right != null && Bound.Intersects(_Right._Region))
                {
                    KDTree<T, K> ClosestRight = _Right.GetClosestSubtree(Bound, Depth + 1);
                    if (ClosestRight != null) Current = ClosestRight;
                }
            }
            else
            {
                if (_Right != null)
                {
                    KDTree<T, K> ClosestRight = _Right.GetClosestSubtree(Bound, Depth + 1);
                    if (ClosestRight != null) Current = ClosestRight;
                }

                if (_Left != null && Bound.Intersects(_Left._Region))
                {
                    KDTree<T, K> ClosestLeft = _Left.GetClosestSubtree(Bound, Depth + 1);
                    if (ClosestLeft != null) Current = ClosestLeft;
                }
            }
            return Current;
        }

        private static int Ninther<A>(A[] KeysAndValues, int Low, int High, int Dimension) where A : IMultiDimensionComparable
        {
            int c1 = Low + (High - Low) / 3;
            int c2 = Low + 2 * (High - Low) / 3;
            return Median(
                KeysAndValues,
                Median(KeysAndValues, Low, c1 - 1, Dimension),
                Median(KeysAndValues, c1, c2 - 1, Dimension),
                Median(KeysAndValues, c2, High, Dimension), Dimension);
        }

        private static int Median<A>(A[] KeysAndValues, int Low, int High, int Dimension) where A : IMultiDimensionComparable
        {
            if (High - Low > 1) return Median(KeysAndValues, Low, (Low + High) / 2, High, Dimension);
            else if (High - Low == 1) return MaxIndex(KeysAndValues, Low, High, Dimension);
            else return Low;
        }

        private static int Median<A>(A[] Arr, int a, int b, int c, int dim) where A : IMultiDimensionComparable
        {
            if (MaxIndex(Arr, a, b, dim) == a)
            {
                int tmp = a;
                a = b;
                b = tmp;
            }
            return MaxIndex(Arr, MinIndex(Arr, b, c, dim), a, dim);
        }

        private static void Swap<A>(A[] Arr, int i, int j)
        {
            A tmp = Arr[j];
            Arr[j] = Arr[i];
            Arr[i] = tmp;
        }

        private static int MaxIndex<A>(A[] Arr, int a, int b, int dim) where A : IMultiDimensionComparable
        {
            if (Arr[a].CompareTo(Arr[b], dim) > 0) return a;
            else return b;
        }

        private static int MinIndex<A>(A[] Arr, int a, int b, int dim) where A : IMultiDimensionComparable
        {
            if (Arr[a].CompareTo(Arr[b], dim) < 0) return a;
            else return b;
        }

        private static void Shuffle<A>(A[] Arr)
        {
            Random Random = new Random();
            for (int i = 0; i < Arr.Length; ++i)
            {
                Swap(Arr, i, Random.Next(0, Arr.Length));
            }
        }
    }
}

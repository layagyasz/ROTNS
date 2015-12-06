using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;
using Cardamom.Serialization;

namespace AndrassyII
{
    class Set<T> : IEnumerable<T>, Generator<T> where T : Generator<T>
    {
        HashSet<T> _Elements = new HashSet<T>();
        WeightedVector<T> _Selector = new WeightedVector<T>();

        public double Frequency { get { return 1; } }
        public IEnumerator<T> GetEnumerator() { return _Elements.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Set() { }

        public Set(string Source, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Sets)
        {
            foreach (T V in ParseSet(Source, Operators, Sets)) Add(V, V.Frequency);
        }

        public static Set<T> ParseSet(string Source, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Sets)
        {
            return new ParseTree<T>(Source, Operators, Sets).Traverse();
        }

        public Set(ParseBlock Block, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Sets, Dictionary<string, T> Sounds)
        {
            foreach (string S in Block.String.Split(':'))
            {
                bool c = false;
                foreach (Operator<T> O in Operators)
                {
                    if (S.Contains(O.Symbol)) { c = true; break; }
                }
                if (c)
                {
                    foreach (T V in ParseSet(S, Operators, Sets)) Add(V, V.Frequency);
                }
                else
                {
                    T s = Sounds[S.Trim()];
                    Add(s, s.Frequency);
                }
            }
        }

        public void Add(T Value, double Weight)
        {
            if (!_Elements.Contains(Value))
            {
                _Elements.Add(Value);
                _Selector.Add(Weight, Value);
            }
        }

        public bool Contains(T Value) { return _Elements.Contains(Value); }

        public Generated<T> Generate(Random Random)
        {
            if (_Selector.Length > 0) return _Selector[Random.NextDouble()].Generate(Random);
            else return null;
        }

        public T Random(Random Random)
        {
            return _Selector[Random.NextDouble()];
        }

        public static Set<T> operator +(Set<T> s1, Set<T> s2)
        {
            Set<T> S = new Set<T>();
            foreach (T V in s1) S.Add(V, V.Frequency);
            foreach (T V in s2) S.Add(V, V.Frequency);
            return S;
        }

        public static Set<T> operator *(Set<T> s1, Set<T> s2)
        {
            Set<T> S = new Set<T>();
            foreach (T V in s1)
            {
                if (s2.Contains(V)) S.Add(V, V.Frequency);
            }
            return S;
        }

        public static Set<T> operator -(Set<T> s1, Set<T> s2)
        {
            Set<T> S = new Set<T>();
            foreach (T V in s1)
            {
                if (!s2.Contains(V)) S.Add(V, V.Frequency);
            }
            return S;
        }
    }
}

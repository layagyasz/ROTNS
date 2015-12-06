using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII
{
    class MatchRule<T> where T : Generator<T>
    {
        public static readonly Set<T> StartGenerated = new Set<T>();
        public static readonly Set<T> EndGenerated = new Set<T>();

        List<Set<T>> _Sets = new List<Set<T>>();
        int _Length;

        public int Length { get { return _Length; } }

        public MatchRule(string Source, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Generators)
        {
            foreach (string S in Source.Split(':'))
            {
                if (S.Trim() == "$")
                {
                    if (_Sets.Count == 0) _Sets.Add(StartGenerated);
                    else _Sets.Add(EndGenerated);
                }
                else
                {
                    _Sets.Add(Set<T>.ParseSet(S, Operators, Generators));
                    _Length++;
                }
            }
        }

        public bool Match(Generated<T> CompareTo, int Index)
        {
            for (int i = 0; i < _Sets.Count && Index + i <= CompareTo.Length; ++i)
            {
                if (Index + i == -1 && _Sets[i] != StartGenerated) return false;
                else if (Index + i == CompareTo.Length && _Sets[i] != EndGenerated) return false;
                else if (Index + i > -1 && Index + i < CompareTo.Length && !_Sets[i].Contains(CompareTo[Index + i])) return false;
            }
            return true;
        }
    }
}

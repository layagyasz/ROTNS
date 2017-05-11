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
        int _StartIndex = 0;
        int _EndIndex = -1;

        public int Length { get { return _Length; } }
        public int StartIndex { get { return _StartIndex; } }
        public int EndIndex { get { return _EndIndex; } }

        public MatchRule(string Source, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Generators)
        {
            string[] s = Source.Split(':');
            for(int i=0; i<s.Length;++i)
            {
                string S = s[i];
                if (S.Trim() == "$")
                {
                    if (_Sets.Count == 0)
                    {
                        _Sets.Add(StartGenerated);
                        _StartIndex = 1;
                    }
                    else
                    {
                        _Sets.Add(EndGenerated);
                        _Length--;
                    }
                }
                else
                {
                    if (S.Contains('('))
                    {
                        _StartIndex = _Length;
                        S = S.Replace("(", "");
                    }
                    if (S.Contains(')'))
                    {
                        _EndIndex = _Length;
                        S = S.Replace(")", "");
                    }
                    _Sets.Add(Set<T>.ParseSet(S, Operators, Generators));
                }
                _Length++;
            }
            if (_EndIndex == -1) _EndIndex = _Length - 1;
            _Length = _EndIndex - _StartIndex + 1;
            Console.WriteLine("{0} {1} {2} {3}", Source, _StartIndex, _EndIndex, _Length);
        }

        public bool Match(Generated<T> CompareTo, int Index)
        {
            for (int i = -StartIndex; i + StartIndex < _Sets.Count; ++i)
            {
                if (Index + i < -1) return false;
                else if (Index + i > CompareTo.Length) return false;
                else if (Index + i == -1 && _Sets[i + StartIndex] != StartGenerated) return false;
                else if (Index + i == CompareTo.Length && _Sets[i + StartIndex] != EndGenerated) return false;
                else if (Index + i > -1 && Index + i < CompareTo.Length && !_Sets[i + StartIndex].Contains(CompareTo[Index + i])) return false;
            }
            return true;
        }
    }
}

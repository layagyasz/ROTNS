using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII
{
    class ReplaceRule<T> where T : Generator<T>
    {
        MatchRule<T> _Match;
        SumGenerator<T> _Replacement;

        public ReplaceRule(ParseBlock Block, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Generators)
        {
            string[] def = Block.String.Split(new string[] { "=>" }, StringSplitOptions.None);
            _Match = new MatchRule<T>(def[0], Operators, Generators);
            _Replacement = new SumGenerator<T>(def[1], Operators, Generators);
        }

        public void Replace(Random Random, Generated<T> ReplaceIn, int Index)
        {
            if (_Match.Match(ReplaceIn, Index))
            {
                ReplaceIn.Remove(Index > 0 ? Index : 0, _Match.Length);
                List<T> New = new List<T>();
                Generated<T> R = _Replacement.Generate(Random);
                foreach (T S in R) New.Add(S);

                if (Index == ReplaceIn.Length) Index = ReplaceIn.Length - 1;
                if (Index == -1) Index = 0;

                ReplaceIn.Insert(Index, New);
            }
        }
    }
}

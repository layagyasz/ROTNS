using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;
using Cardamom.Serialization;

namespace AndrassyII
{
    internal class PrintRule<T> where T : Generator<T>
    {
        MatchRule<T> _Match;
        WeightedVector<string> _Options = new WeightedVector<string>();

        public int Length { get { return _Match.Length; } }

        public PrintRule(ParseBlock Block, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Generators)
        {
            string[] def = Block.String.Split(new string[] { "=>" }, StringSplitOptions.None);
            _Match = new MatchRule<T>(def[0], Operators, Generators);
            string[] ops = def[1].Split(':');
            for (int i = 0; i < ops.Length; ++i)
            {
                if (ops[i].Contains('*'))
                {
                    string[] d = ops[i].Split('*');
                    _Options.Add(Convert.ToDouble(d[0], System.Globalization.CultureInfo.InvariantCulture), d[1].Trim());
                }
                else
                {
                    _Options.Add(1, ops[i].Trim());
                }
            }
        }

        public bool Match(Generated<T> MatchIn, int Index)
        {
            return _Match.Match(MatchIn, Index);
        }

        public string Get(double Index) { return _Options[Index]; }
    }
}

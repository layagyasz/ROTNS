using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;

namespace AndrassyII
{
    class SumGenerator<T> : Generator<T> where T : Generator<T>
    {
        Generator<T>[] _Generators;

        public double Frequency { get { return 1; } }

        public SumGenerator(string Source, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Generators)
        {
            string[] g = Source.Split(':');
            _Generators = new Generator<T>[g.Length];
            for(int i=0; i<g.Length; ++i)
            {
                bool c = false;
                foreach (Operator<T> O in Operators)
                {
                    if (g[i].Contains(O.Symbol)) { c = true; break; }
                }
                if (c) _Generators[i] = Set<T>.ParseSet(g[i], Operators, Generators);
                else _Generators[i] = Generators[g[i].Trim()];
            }
        }

        public Generated<T> Generate(Random Random)
        {
            Generated<T> Word = new Generated<T>();
            for (int i = 0; i < _Generators.Length; ++i)
            {
                Generated<T> w = _Generators[i].Generate(Random);
                if(w != null) Word.Combine(w);
            }
            return Word;
        }
    }
}

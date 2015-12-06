using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;
using Cardamom.Serialization;

namespace AndrassyII
{
    class SingleGenerator<T> : Generator<T> where T: Generator<T>
    {
        WeightedVector<Generator<T>> _Generators = new WeightedVector<Generator<T>>();

        public double Frequency { get { return 1; } }

        public SingleGenerator(ParseBlock Block, List<Operator<T>> Operators, Dictionary<string, Generator<T>> Generators)
        {
            string[] g = Block.String.Split('|');
            foreach (string G in g)
            {
                string[] d = G.Split('@');
                if (d.Length == 2)
                {
                    double w = Convert.ToDouble(d[0], System.Globalization.CultureInfo.InvariantCulture);
                    _Generators.Add(w, new SumGenerator<T>(d[1], Operators, Generators));
                }
                else _Generators.Add(1, new SumGenerator<T>(G, Operators, Generators));
            }
        }

        public Generated<T> Generate(Random Random)
        {
            return _Generators[Random.NextDouble()].Generate(Random);
        }
    }
}

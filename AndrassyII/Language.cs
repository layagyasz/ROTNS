using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Cardamom.Serialization;

namespace AndrassyII
{
    public class Language
    {
        static List<Operator<Sound>> Operators = new List<Operator<Sound>>()
        {
            new Operator<Sound>('+', 1, delegate(Generator<Sound> s1, Generator<Sound> s2) { return (Set<Sound>)s1 + (Set<Sound>)s2;}),
            new Operator<Sound>('-', 1, delegate(Generator<Sound> s1, Generator<Sound> s2) { return (Set<Sound>)s1 - (Set<Sound>)s2;}),
            new Operator<Sound>('*', 2, delegate(Generator<Sound> s1, Generator<Sound> s2) { return (Set<Sound>)s1 * (Set<Sound>)s2;})
        };

        Dictionary<string, Generator<Sound>> _Generators = new Dictionary<string, Generator<Sound>>() { { "None", new Set<Sound>() } };
        List<ReplaceRule<Sound>> _Replacers = new List<ReplaceRule<Sound>>();
        Generator<Sound> _Primary;

        public Language(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.ToLower())
                {
                    case "sounds": ReadSounds(B); break;
                    case "generators": ReadGenerators(B); break;
                    case "replacers": ReadReplacers(B); break;
                }
            }
        }

        public Language(string Path) : this(new ParseBlock(File.ReadAllText(Path))) { }

        private void ReadSounds(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                Sound S = new Sound(B);
                foreach (string I in S.Identifiers)
                {
                    if (!_Generators.ContainsKey(I)) _Generators.Add(I, new Set<Sound>());
                    ((Set<Sound>)_Generators[I]).Add(S, S.Frequency);
                }
                Set<Sound> NS = new Set<Sound>();
                NS.Add(S, S.Frequency);
                _Generators.Add(B.Name, NS);
            }
        }

        private void ReadGenerators(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                SingleGenerator<Sound> G = new SingleGenerator<Sound>(B, Operators, _Generators);
                _Generators.Add(B.Name, G);
                _Primary = G;
            }
        }

        private void ReadReplacers(ParseBlock Block)
        {
            foreach (ParseBlock B in Block.Break())
            {
                _Replacers.Add(new ReplaceRule<Sound>(B, Operators, _Generators));
            }
        }

        public Word Generate(Random Random)
        {
            Word W = new Word(_Primary.Generate(Random));
            W.RemoveDoubles();
            foreach (ReplaceRule<Sound> R in _Replacers)
            {
                for (int i = -1; i <= W.Length; ++i) R.Replace(Random, W, i);
            }
            W.RemoveDoubles();
            return W;
        }
    }
}

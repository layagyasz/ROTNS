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
		private enum Attribute { SOUNDS, GENERATORS, REPLACERS, ORTHOGRAPHY };

		static List<Operator<Sound>> OPERATORS = new List<Operator<Sound>>()
		{
			new Operator<Sound>('+', 1, delegate(Generator<Sound> s1, Generator<Sound> s2) { return (Set<Sound>)s1 + (Set<Sound>)s2;}),
			new Operator<Sound>('-', 1, delegate(Generator<Sound> s1, Generator<Sound> s2) { return (Set<Sound>)s1 - (Set<Sound>)s2;}),
			new Operator<Sound>('*', 2, delegate(Generator<Sound> s1, Generator<Sound> s2) { return (Set<Sound>)s1 * (Set<Sound>)s2;})
		};

		Dictionary<string, Generator<Sound>> _Generators = new Dictionary<string, Generator<Sound>>() { { "*", new Set<Sound>() }, { "none", new Set<Sound>() } };
		List<ReplaceRule<Sound>> _Replacers;
		List<PrintRule<Sound>> _Printers;
		Generator<Sound> _Primary;

		public Language(ParseBlock Block)
		{
			Block.AddParser<Sound>("sound",
			   i =>
			{
				var s = new Sound(i);
				AddSound(s);
				return s;
			});
			Block.AddParser<Generator<Sound>>("generator",
				  i =>
			{
				var g = new SingleGenerator<Sound>(i, OPERATORS, _Generators);
				_Generators.Add(i.Name, g);
				return g;
			});
			Block.AddParser<ReplaceRule<Sound>>("replacer", i => new ReplaceRule<Sound>(i, OPERATORS, _Generators));
			Block.AddParser<PrintRule<Sound>>("orthography", i => new PrintRule<Sound>(i, OPERATORS, _Generators));

			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute));
			_Replacers = (List<ReplaceRule<Sound>>)attributes[(int)Attribute.REPLACERS];
			_Printers = (List<PrintRule<Sound>>)attributes[(int)Attribute.ORTHOGRAPHY];
			_Primary = _Generators.Last().Value;
		}

		public Language(string Path) : this(new ParseBlock(File.ReadAllText(Path))) { }

		public void AddSound(Sound Sound)
		{
			foreach (string I in Sound.Identifiers)
			{
				if (!_Generators.ContainsKey(I)) _Generators.Add(I, new Set<Sound>());
				((Set<Sound>)_Generators[I]).Add(Sound, Sound.Frequency);
			}
			Set<Sound> NS = new Set<Sound>();
			NS.Add(Sound, Sound.Frequency);
			_Generators.Add(Sound.Symbol, NS);
			((Set<Sound>)_Generators["*"]).Add(Sound, Sound.Frequency);
		}

		public Word Generate(Random Random)
		{
			Word W = new Word(_Primary.Generate(Random), Random, _Printers);
			W.RemoveDoubles();
			foreach (ReplaceRule<Sound> R in _Replacers)
			{
				for (int i = 0; i <= W.Length; ++i) R.Replace(Random, W, i);
			}
			W.RemoveDoubles();
			MakePrintable(W, Random, _Printers);
			return W;
		}

		private void MakePrintable(Word Word, Random Random, List<PrintRule<Sound>> Rules)
		{
			string R = "";
			for (int i = 0; i < Word.Length;)
			{
				bool Matched = false;
				foreach (PrintRule<Sound> Rule in Rules)
				{
					if (Rule.Match(Word, i))
					{
						i += Rule.Length;
						R += Rule.Get(Random.NextDouble());
						Matched = true;
						break;
					}
				}
				if (!Matched) R += Word[i++].ToString();
			}
			Word.Orthography = R;
		}
	}
}

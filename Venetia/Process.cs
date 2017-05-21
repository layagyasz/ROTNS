using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;
using Cardamom.Serialization;

namespace Venetia
{
	public class Process
	{
		private enum Attribute { INPUT, OUTPUT };

		Tuple<Tangible, double>[] _Input;
		Tuple<Tangible, double>[] _Output;

		public Tuple<Tangible, double>[] Input { get { return _Input; } }
		public Tuple<Tangible, double>[] Output { get { return _Output; } }

		public Process(ParseBlock Block)
		{
			List<Tuple<object, object>>[] attributes =
				Block.BreakToAttributes<List<Tuple<object, object>>>(typeof(Attribute));
			_Input = attributes[(int)Attribute.INPUT]
				.Select(i => new Tuple<Tangible, double>((Tangible)i.Item1, (double)i.Item2))
				.ToArray();
			_Output = attributes[(int)Attribute.OUTPUT]
				.Select(i => new Tuple<Tangible, double>((Tangible)i.Item1, (double)i.Item2))
				.ToArray();
		}

		public Pair<double, double> OptimumSupply(Zone Zone)
		{
			Pair<EconomicAttributes, double>[] I = new Pair<EconomicAttributes, double>[_Input.Length];
			Pair<EconomicAttributes, double>[] O = new Pair<EconomicAttributes, double>[_Output.Length];
			for (int i = 0; i < I.Length; ++i) I[i] = new Pair<EconomicAttributes, double>(Zone[_Input[i].Item1], _Input[i].Item2);
			for (int i = 0; i < O.Length; ++i) O[i] = new Pair<EconomicAttributes, double>(Zone[_Output[i].Item1], _Output[i].Item2);
			return Calculator.OptimumSupply(I, O, Zone.Population);
		}

		public override string ToString()
		{
			return string.Format("[Process: Input={0}, Output={1}]",
				 string.Join<object>(",", Input), string.Join<object>(",", Output));
		}
	}
}

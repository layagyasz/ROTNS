using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;

namespace Venetia
{
	public class Producer
	{
		double _Scale;
		Process _Process;

		public double Scale { get { return _Scale; } }
		public Process Process { get { return _Process; } }

		public Producer(Process Process, double Scale) { _Process = Process; _Scale = Scale; }

		public Pair<double, double> Profit(Zone Zone)
		{
			double Revenue = 0;
			double Cost = 0;
			foreach (Tuple<Tangible, double> T in _Process.Input) Cost += Zone[T.Item1].Price(Zone.Population) * _Scale * T.Item2;
			foreach (Tuple<Tangible, double> T in _Process.Output) Revenue += Zone[T.Item1].Price(Zone.Population) * _Scale * T.Item2;
			return new Pair<double, double>(Revenue, Cost);
		}

		public override string ToString()
		{
			return string.Format("[Producer: Scale={0}, Process={1}]", Scale, Process);
		}
	}
}

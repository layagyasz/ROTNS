using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Graphing;
using Cardamom.Serialization;
using Cardamom.Utilities;

using SFML.Graphics;
using SFML.Window;

namespace ROTNS.Model.Flags
{
	public class FlagTemplate
	{
		private enum Attribute { FREQUENCY, PARTS, EDGES };

		double _Frequency;
		GraphNode<Vector2f[]>[] _Parts;

		public double Frequency
		{
			get
			{
				return _Frequency;
			}
		}

		public FlagTemplate(ParseBlock Block)
		{
			object[] attributes = Block.BreakToAttributes<object>(typeof(Attribute), true);
			_Frequency = (double)attributes[(int)Attribute.FREQUENCY];
			_Parts = ((Dictionary<string, GraphNode<Vector2f[]>>)attributes[(int)Attribute.PARTS])
				.Select(i => i.Value).ToArray();
			foreach (var N in (List<Tuple<object, object, object>>)attributes[(int)Attribute.EDGES])
				((GraphNode<Vector2f[]>)N.Item1).AddNeighbor((GraphNode<Vector2f[]>)N.Item2, (float)N.Item3);
		}

		private Pair<double, int> ColoringScore(Culture Culture, FlagColor[] Colors)
		{
			double Score = 0;
			double WorstPartScore = double.MaxValue;
			int WorstPart = -1;
			for (int i = 0; i < _Parts.Length; ++i)
			{
				GraphNode<Vector2f[]> part = _Parts[i];
				FlagColor Color = Colors[i];
				double Distance = 1 - Color.DistanceTo(Culture) / 2.2360679775;
				Distance *= Distance * Distance;
				double PartScore = Distance;
				foreach (var edge in part.Edges)
				{
					double M = 1 - Color.DistanceTo(Colors[Array.IndexOf(_Parts, edge.Second)]) / 1.73205080757;
					M = M * M * M * M;
					PartScore -= Distance * M * edge.First;
				}
				Score += PartScore;
				if (PartScore < WorstPartScore)
				{
					WorstPart = i;
					WorstPartScore = PartScore;
				}
			}
			return new Pair<double, int>(Score, WorstPart);
		}

		public Vertex[] MakeFlag(FlagColor[] Colors)
		{
			Vertex[] V = new Vertex[_Parts.Length * 4];

			for (int i = 0; i < _Parts.Length; ++i)
			{
				for (int j = 0; j < 4; ++j)
				{
					V[i * 4 + j] = new Vertex(_Parts[i].Value[j], Colors[i].Color);
				}
			}

			return V;
		}

		public FlagColor[] SelectColors(Culture Culture, FlagColorMap Colors, Random Random, int Iterations = 30)
		{
			FlagColor[] Chosen = Colors.Closest(Culture, _Parts.Length);

			Pair<double, int> P = ColoringScore(Culture, Chosen);

			for (int j = 0; j < Iterations; ++j)
			{
				P = ColoringScore(Culture, Chosen);
				int C = Random.NextDouble() > .5 ? P.Second : Random.Next(0, _Parts.Length);
				double Score = P.First;

				FlagColor Original = Chosen[C];
				// Find best new assignment
				FlagColor Best = Chosen[C];
				for (int i = 0; i < Colors.Colors.Length; ++i)
				{
					Chosen[C] = Colors.Colors[i];
					double thisScore = ColoringScore(Culture, Chosen).First;
					if (thisScore > Score)
					{
						Score = thisScore;
						Best = Colors.Colors[i];
					}
				}
				Chosen[C] = Best;
			}
			return Chosen;
		}
	}
}

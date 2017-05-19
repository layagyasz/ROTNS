using System;
using Cence;

namespace ROTNS.Model.WorldGeneration
{
	public class CultureMap
	{
		LatticeNoiseGenerator _Individualism;
		LatticeNoiseGenerator _Indulgence;
		LatticeNoiseGenerator _LongTermOrientation;
		LatticeNoiseGenerator _PowerDistance;
		LatticeNoiseGenerator _Toughness;
		LatticeNoiseGenerator _UncertaintyAvoidance;

		public CultureMap(Random Random, LatticeNoiseSettings Settings)
		{
			_Individualism = new LatticeNoiseGenerator(Random, Settings);
			_Indulgence = new LatticeNoiseGenerator(Random, Settings);
			_LongTermOrientation = new LatticeNoiseGenerator(Random, Settings);
			_PowerDistance = new LatticeNoiseGenerator(Random, Settings);
			_Toughness = new LatticeNoiseGenerator(Random, Settings);
			_UncertaintyAvoidance = new LatticeNoiseGenerator(Random, Settings);
		}

		public Culture Generate(double X, double Y)
		{
			return new Culture()
			{
				Individualism = (float)_Individualism.Generate(X, Y),
				Indulgence = (float)_Indulgence.Generate(X, Y),
				LongTermOrientation = (float)_LongTermOrientation.Generate(X, Y),
				PowerDistance = (float)_PowerDistance.Generate(X, Y),
				Toughness = (float)_Toughness.Generate(X, Y),
				UncertaintyAvoidance = (float)_UncertaintyAvoidance.Generate(X, Y)
			};
		}
	}
}

using System;

using SFML.Window;

namespace Cence
{
	public class LatticeNoiseSettings
	{
		public Func<double, double, double> Frequency = (x, y) => 1;
		public Func<double, double, double> Lacunarity = (x, y) => 2;
		public int Octaves = 8;
		public Func<double, double, double> Persistence = (x, y) => .6;
		public double Bias = .5;
		public double Factor = 1;

		public Interpolator.InterpolatorFunction Interpolator = Cence.Interpolator.Hermite;
		public Evaluator.EvaluatorFunction Evaluator = Cence.Evaluator.Gradient;

		public GeneratorMode GeneratorMode = GeneratorMode.Standard;
		public Treatment.TreatmentFunction PreTreatment = Treatment.None;
		public Treatment.TreatmentFunction PostTreatment = Treatment.None;
		public Func<double, double, Vector2f> Turbulence = (x, y) => new Vector2f(0, 0);
		public Func<double, double, double, double> PostModification = (x, y, v) => v;

		public LatticeNoiseSettings()
		{
		}
	}
}

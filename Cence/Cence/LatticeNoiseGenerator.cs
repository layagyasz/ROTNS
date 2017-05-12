using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;

namespace Cence
{
    public class LatticeNoiseGenerator
    {
        private Vector3d[] _Vectors;

        int[] _Lookup;
        private int _LookupSize = 2048;

        Func<double, double, double> _Frequency = (x, y) => 1;
        Func<double, double, double> _Lacunarity = (x, y) => 2;
        int _Octaves = 8;
        Func<double, double, double> _Persistence = (x, y) => .6;
        double _Bias = .5;
        double _Factor = 1;

        Interpolator.InterpolatorFunction _Interpolator = Cence.Interpolator.Hermite;
        Evaluator.EvaluatorFunction _Evaluator = Cence.Evaluator.Gradient;

        public GeneratorMode GeneratorMode = GeneratorMode.Standard;
        public Treatment.TreatmentFunction PreTreatment = Treatment.None;
        public Treatment.TreatmentFunction PostTreatment = Treatment.None;
		public Func<double, double, Vector2f> Turbulence = (x, y) => new Vector2f(0, 0);
        public Func<double, double, double, double> PostModification = (x, y, v) => v;

        public Interpolator.InterpolatorFunction Interpolator { get { return _Interpolator; } set { _Interpolator = value; } }
        public Evaluator.EvaluatorFunction Evaluator { get { return _Evaluator; } set { _Evaluator = value; } }
        public Func<double, double, double> Frequency { get { return _Frequency; } set { _Frequency = value; } }
        public Func<double, double, double> Lacunarity { get { return _Lacunarity; } set { _Lacunarity = value; } }
        public int Octaves { get { return _Octaves; } set { _Octaves = value; } }
        public Func<double, double, double> Persistence { get { return _Persistence; } set { _Persistence = value; } }
        public double Bias { get { return _Bias; } set { _Bias = value; } }
        public double Factor { get { return _Factor; } set { _Factor = value; } }

        public LatticeNoiseGenerator(Random Random)
        {
            _Lookup = new int[_LookupSize];
            _Vectors = new Vector3d[_LookupSize];
            Init(Random);
        }

		public LatticeNoiseGenerator(Random Random, LatticeNoiseSettings Settings)
			: this(Random)
		{
			_Frequency = Settings.Frequency;
			_Lacunarity = Settings.Lacunarity;
			_Octaves = Settings.Octaves;
			_Persistence = Settings.Persistence;
			_Bias = Settings.Bias;
			_Factor = Settings.Factor;

			_Interpolator = Settings.Interpolator;
			_Evaluator = Settings.Evaluator;

			GeneratorMode = Settings.GeneratorMode;
			PreTreatment = Settings.PreTreatment;
			PostTreatment = Settings.PostTreatment;
			Turbulence = Settings.Turbulence;
			PostModification = Settings.PostModification;
		}

        private void Init(Random Random)
        {
            for (int i = 0; i < _LookupSize; ++i) _Lookup[i] = i;
            for (int i = 0; i < _LookupSize; ++i)
            {
                int temp = _Lookup[i];
                int swap = Random.Next(0, 256);
                _Lookup[i] = _Lookup[swap];
                _Lookup[swap] = temp;
                double yaw = Random.NextDouble() * 2 * Math.PI;
                double pitch = (Random.NextDouble() - .5) * Math.PI;
                double X = Math.Cos(yaw) * Math.Cos(pitch);
                double Z = Math.Sin(pitch);
                double Y = Math.Sin(yaw) * Math.Cos(pitch);
                _Vectors[i] = new Vector3d(X, Y, Z);
            }
        }

        private Vector3d Hash(int X, int Y)
        {
            return _Vectors[_Lookup[(_Lookup[X & (_LookupSize - 1)] + (Y & (_LookupSize - 1))) % _LookupSize] % _LookupSize];
        }

        private double GenerateSingle(double X, double Y)
        {
            int gX = (int)(X + (X < 0 ? -1 : 0));
            int gY = (int)(Y + (Y < 0 ? -1 : 0));
            double dX = X - gX;
            double dY = Y - gY;

            double s = _Interpolator(dX, dY) * PreTreatment(_Evaluator(dX, dY, Hash(gX, gY)));
            double t = _Interpolator(1 - dX, dY) * PreTreatment(_Evaluator(dX - 1, dY, Hash(gX + 1, gY)));
            double u = _Interpolator(dX, 1 - dY) * PreTreatment(_Evaluator(dX, dY - 1, Hash(gX, gY + 1)));
            double v = _Interpolator(1 - dX, 1 - dY) * PreTreatment(_Evaluator(dX - 1, dY - 1, Hash(gX + 1, gY + 1)));

            double n = s + t + u + v;
            return PostTreatment(n);
        }

        private double GenerateStandard(double X, double Y)
        {
            double Total = 0;
            double Frequency = _Frequency(X, Y);
            double Amplitude = 1;
            double Max = 0;
            double P = _Persistence(X, Y);
            double L = _Lacunarity(X, Y);
            for (int i = 0; i < _Octaves; i++)
            {
                Total += GenerateSingle(X * Frequency, Y * Frequency) * Amplitude;
                Max += Amplitude;
                Amplitude *= P;
                Frequency *= L;
            }

            return Total / Max * _Factor + _Bias;
        }

        private double GenerateDerivative(double X, double Y)
        {
            double Total = 0;
            double Frequency = _Frequency(X, Y);
            double Amplitude = 1;
            double Max = 0;
            double dX = 0;
            double dY = 0;
            double P = _Persistence(X, Y);
            double L = _Lacunarity(X, Y);
            for (int i = 0; i < _Octaves; i++)
            {
                double v = GenerateSingle(X * Frequency, Y * Frequency);
                double vl = GenerateSingle((X + .01) * Frequency, Y * Frequency);
                double vu = GenerateSingle(X * Frequency, (Y + .01) * Frequency);
                dX += (vl - v) * 100 / Frequency;
                dY += (vu - v) * 100 / Frequency;
                Total += Amplitude * v / (1 + dX * dX + dY * dY);
                Max += Amplitude;
                Amplitude *= P;
                Frequency *= L;
            }

            return Total / Max * _Factor + _Bias;
        }

        public double Generate(double X, double Y)
        {
			Vector2f T = Turbulence(X, Y);
            if (GeneratorMode == GeneratorMode.Standard)
				return PostModification(X, Y, GenerateStandard(X + T.X, Y + T.Y));
            else return PostModification(X, Y, GenerateDerivative(X + T.X, Y + T.Y));
        }

        public FloatingImage GenerateImage(int Width, int Height)
        {
            FloatingImage F = new FloatingImage(Width, Height);
            float Min = float.MaxValue;
            float Max = float.MinValue;
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    float G = (float)Generate(i, j);
                    F[i, j] = new FloatingColor(G);
                    if (G > Max) Max = G;
                    if (G < Min) Min = G;
                }
            }
            return F;
        }

        public void SetRecommendedFactorAndBias(Random Random)
        {
            _Factor = 1;
            _Bias = 0;
            double min = double.MaxValue;
            double max = double.MinValue;
            for(int i=0; i<10000; ++i)
            {
                double v = Generate(Random.NextDouble(), Random.NextDouble());
                if (v < min) min = v;
                if (v > max) max = v;
            }
            _Factor = 1 / (max - min);
            _Bias = -min * _Factor;
        }
    }
}

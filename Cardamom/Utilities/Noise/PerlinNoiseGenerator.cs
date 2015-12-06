using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Utilities.Noise
{
    public class PerlinNoiseGenerator
    {
        int[] _Lookup;
        int _Scale;

        public PerlinNoiseGenerator(Random Random, int Scale = 1)
        {
            _Scale = Scale;
            _Lookup = new int[256];
            for (int i = 0; i < 256; ++i) _Lookup[i] = i;
            for (int i = 0; i < 256; ++i)
            {
                int temp = _Lookup[i];
                int swap = Random.Next(0, 256);
                _Lookup[i] = _Lookup[swap];
                _Lookup[swap] = temp;
            }
        }

        private int Hash(int X, int Y)
        {
            return _Lookup[(_Lookup[X & 255] + (Y & 255)) % 256];
        }

        private double Gradient(int gX, int gY, double X, double Y)
        {
            int h = Hash(gX, gY);
            switch (h & 7)
            {
                case 0: return X + Y;
                case 1: return X - Y;
                case 2: return -X + Y;
                case 3: return -X - Y;
                case 4: return -1.4142135623 * X;
                case 5: return 1.4142135623 * X;
                case 6: return -1.4142135623 * Y;
                case 7: return 1.4142135623 * Y;
                default: return 0;
            }
        }

        private double BlendCurve(double X)
        {
            return 3 * X * X - 2 * X * X * X;
        }

        private double GenerateSingle(double X, double Y)
        {
            int gX = (int)X;
            int gY = (int)Y;
            double dX = X - (int)X;
            double dY = Y - (int)Y;

            double s = Gradient(gX, gY, dX, dY);
            double t = Gradient(gX + 1, gY, dX - 1, dY);
            double u = Gradient(gX, gY + 1, dX, dY - 1);
            double v = Gradient(gX + 1, gY + 1, dX - 1, dY - 1);

            
            double sX = BlendCurve(dX);
            double a = s + sX * (t - s);
            double b = u + sX * (v - u);

            return (a + BlendCurve(dY) * (b - a)) / 2 + .5;
        }

        public double Generate(double X, double Y, int Octaves, double Persistence)
        {
            X /= _Scale;
            Y /= _Scale;
            double Total = 0;
            double Frequency = 1;
            double Amplitude = 1;
            double Max = 0;
            for (int i = 0; i < Octaves; i++)
            {
                Total += GenerateSingle(X * Frequency, Y * Frequency) * Amplitude;
                Max += Amplitude;
                Amplitude *= Persistence;
                Frequency *= 2;
            }

            return Total / Max;
        }
    }
}

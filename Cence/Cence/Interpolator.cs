using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public class Interpolator
    {
        public delegate double InterpolatorFunction(params double[] A);

        public static double Hermite(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= (1 - (X * X * (3 - 2 * X)));
            return R;
        }

        public static double HermiteQuintic(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= (1 - (X * X * X * (10 - 15 * X + 6 * X * X)));
            return R;
        }

        public static double Sigmoid(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= (1 / (1 + Math.Exp((X - .5) * 10)));
            return R;
        }

        public static double HermiteSigmoid(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= (Hermite(X) + Sigmoid(X)) * .5;
            return R;
        }

        public static double Linear(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= 1 - X;
            return R;
        }

        public static double HermiteDisplaced(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= Hermite(Math.Abs(X - .5) * 2);
            return R;
        }

        public static double Pyramid(params double[] A)
        {
            double R = double.MaxValue;
            foreach (double X in A) R = Math.Min(R, 1 - X);
            return Math.Max(0, R);
        }

        public static double Cosine(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= (1 + Math.Cos(X * Math.PI)) * .5;
            return R;
        }

        public static double Epanechnikov(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= (1 - X * X);
            return R;
        }

        public static double Quartic(params double[] A)
        {
            double R = 1;
            foreach(double X in A) R *= (1 - X * X) * (1 - X * X);
            return R;
        }

        public static double Triweight(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= (1 - X * X);
            return R * R * R;
        }

        public static double Tricube(params double[] A)
        {
            double R = 1;
            foreach (double X in A) R *= (1 - Math.Abs(X * X * X));
            return R * R * R;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cence
{
    public class Blenders
    {
        public static readonly Dictionary<string, Func<double, double>> Functions = new Dictionary<string, Func<double, double>>()
        {
            { "hermite", Hermite},
            { "hermite-quintic", HermiteQuintic},
            { "sigmoid", Sigmoid },
            { "hermite-sigmoid", HermiteSigmoid },
            { "linear", Linear },
            { "cosine", Cosine },
        };

        public static double Hermite(double X)
        {
            return (1 - (X * X * (3 - 2 * X)));
        }

        public static double HermiteQuintic(double X)
        {
            return (1 - (X * X * X * (10 - 15 * X + 6 * X * X)));
        }

        public static double Sigmoid(double X)
        {
            return (1 / (1 + Math.Exp((X - .5) * 10)));
        }

        public static double HermiteSigmoid(double X)
        {
            return (Hermite(X) + Sigmoid(X)) * .5;
        }

        public static double Linear(double X)
        {
            return (1 - X);
        }

        public static double Cosine(double X)
        {
            return (1 + Math.Cos(X * Math.PI)) * .5;
        }
    }
}

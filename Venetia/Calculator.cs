using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;

namespace Venetia
{
    class Calculator
    {
        public static Pair<double, double> OptimumSupply(Pair<EconomicAttributes, double>[] Input, Pair<EconomicAttributes, double>[] Output, double Population)
        {
            double Min = 0;
            double Max = Double.MaxValue;
            for (int i = 0; i < Input.Length; ++i)
            {
                double m = (Input[i].First.Supply - Input[i].First.Demand) / Input[i].Second;
                if (m < Max) Max = m;
            }
            for (int i = 0; i < Output.Length; ++i)
            {
                double min = (Output[i].First.Demand - Output[i].First.Supply) / Output[i].Second;
                double max = (Output[i].First.MaxNewProduction(Population)) / Output[i].Second;
                if (min > Min) Min = min;
                if (max < Max) Max = max;
            }
            if (Max <= Min) return new Pair<double, double>(0, Double.NaN);
            double X = Min + 1;
            for (int i = 0; i < 20; i++)
            {
                X = X - DOptimumSupplyAux(X, Input, Output, Population) / DDOptimumSupplyAux(X, Input, Output, Population);
            }
            double Profit = 0;
            for (int i = 0; i < Output.Length; ++i) { double a = (1 - Output[i].First.IncomeReduction) * Output[i].Second * X * Output[i].First.SupplyPrice(Output[i].Second * X, Population); if (a > 0) Profit += a;}
            for (int i = 0; i < Input.Length; ++i) {Profit -= Input[i].Second * X * Input[i].First.SupplyPrice(-Input[i].Second * X, Population); }
            return new Pair<double, double>(X, Profit);
        }

        static double DOptimumSupplyAux(double X, Pair<EconomicAttributes,double>[] Input, Pair<EconomicAttributes, double>[] Output, double Population)
        {
            double R = 0;
            for (int i = 0; i < Output.Length; ++i) R += DProduction(X, Output[i].First.LivingStandard * (1 - Output[i].First.IncomeReduction), Output[i].First.Supply, Output[i].First.Demand, Output[i].Second, Output[i].First.Exponent, Output[i].First.Decay * Output[i].First.Coefficient * Population);
            if (Double.IsNaN(R)) return R;
            for (int i = 0; i < Input.Length; ++i) R += DProduction(X, Input[i].First.LivingStandard, Input[i].First.Supply, Input[i].First.Demand, -Input[i].Second, Input[i].First.Exponent, Input[i].First.Decay * Input[i].First.Coefficient * Population);
            return R;
        }

        static double DDOptimumSupplyAux(double X, Pair<EconomicAttributes, double>[] Input, Pair<EconomicAttributes, double>[] Output, double Population)
        {
            double R = 0;
            for (int i = 0; i < Output.Length; ++i) R += DDProduction(X, Output[i].First.LivingStandard * (1 - Output[i].First.IncomeReduction), Output[i].First.Supply, Output[i].First.Demand, Output[i].Second, Output[i].First.Exponent, Output[i].First.Decay * Output[i].First.Coefficient * Population);
            if (Double.IsNaN(R)) return R;
            for (int i = 0; i < Input.Length; ++i) R += DDProduction(X, Input[i].First.LivingStandard, Input[i].First.Supply, Input[i].First.Demand, -Input[i].Second, Input[i].First.Exponent, Input[i].First.Decay * Input[i].First.Coefficient * Population);
            return R;
        }

        static double DProduction(double X, double Q, double S, double E, double P, double K, double M)
        {
			double Logistic = Math.Exp(1 - 2 * ((S - E + P * X) / M));
			double Log_K = Math.Log(K);
			return 2 * P * P * Q * X * Logistic / (M * Log_K * (Logistic + 1)) - P * Q * Math.Log(Logistic + 1) / Log_K;
        }

		static double DDProduction(double X, double Q, double S, double E, double P, double K, double M)
		{
			double Logistic = Math.Exp(1 - 2 * ((S - E + P * X) / M));
			return -P * X * ((4 * P * P * X / (M * M * (Logistic + 1))) * (Logistic - Math.Exp(2 - 4 *
           		((S - E + P * X) / M)) / (Logistic + 1)) - 4 * P * Logistic / (M * (Logistic + 1))) / Math.Log(K);
		}

        public static Triplet<double, double, double> OptimumTrade(EconomicAttributes G0L0, EconomicAttributes G0L1, EconomicAttributes G1L0, EconomicAttributes G1L1, EconomicAttributes Labor, double P0, double P1)
        {
            double MaxG0L0 = G0L0.Supply - G0L0.Demand;
            double MaxG0L1 = G0L1.MaxNewProduction(P1);
            double MinG0L1 = G0L1.Demand - G0L1.Supply;
            double MaxG1L0 = G1L0.MaxNewProduction(P0);
            double MaxG1L1 = G1L1.Supply - G1L1.Demand;
            double MinG1L0 = G1L0.Demand - G1L0.Supply;

            double Max = Math.Min(MaxG0L0, MaxG1L1);
            double Min = Math.Max(MinG0L1, 0);
            if (Max <= Min) return new Triplet<double, double, double>(Double.NaN, Double.NaN, 0);
            double X = Min + 1;
            double Xi = 0;
            for (int i = 0; i < 20; ++i)
            {
                double C = (1 - G0L1.IncomeReduction) * X * G0L1.SupplyPrice(X, P1);
                Xi = OptimumTradeAux(C, G1L1, P1);
                X = X - (DProduction(Xi, G1L0.LivingStandard, G1L0.Supply, G1L0.Demand, 1, G1L0.Exponent, G1L0.Decay * G1L0.Coefficient * P0) + DProduction(X, (1 - G0L0.IncomeReduction) * G0L0.LivingStandard, G0L0.Supply, G0L0.Demand, -1, G0L0.Exponent, G0L0.Decay * G0L0.Coefficient * P1) + DProduction(Math.Max(Xi, X), Labor.LivingStandard, Labor.Supply, Labor.Demand, -.001, Labor.Exponent, Labor.Decay * Labor.Coefficient * P0))
                    / (DDProduction(Xi, G1L0.LivingStandard, G1L0.Supply, G1L0.Demand, 1, G1L0.Exponent, G1L0.Decay * G1L0.Coefficient * P0) + DDProduction(X, (1 - G0L0.IncomeReduction) * G0L0.LivingStandard, G0L0.Supply, G0L0.Demand, -1, G0L0.Exponent, G0L0.Decay * G0L0.Coefficient * P1) + DDProduction(Math.Max(Xi, X), Labor.LivingStandard, Labor.Supply, Labor.Demand, -.001, Labor.Exponent, Labor.Decay * Labor.Coefficient * P0));
            }
            double Co = X * G0L1.SupplyPrice(X, P1);
            Xi = OptimumTradeAux(Co, G1L1, P1);
            double P = Xi * G1L0.SupplyPrice(Xi, P0) - X * G0L0.SupplyPrice(-X, P0) - Math.Max(X, Xi) * Labor.SupplyPrice(-Math.Max(X, Xi), P0);
            return new Triplet<double, double, double>(X, Xi, P);
        }

        static double OptimumTradeAux(double Goal, EconomicAttributes Good, double Population)
        {
            if (Double.IsInfinity(Goal) || Double.IsNaN(Goal)) return 0;
            double Max = Good.Supply - Good.Demand;
            if (Max < 0) return 0;
            double X = Max - .000001;
            for (int i = 0; i < 20; ++i)
            {
                if (X < 0) X = .000001;
                /*
                Console.WriteLine(X);
                Console.WriteLine(X * Good.SupplyPrice(-X, Population) - Goal);
                Console.WriteLine(Good.SupplyPrice(-X, Population));
                Console.WriteLine(Good);
                Console.WriteLine(DProduction(X, Good.LivingStandard, Good.Supply, Good.Demand, -1, Good.Exponent, Population * Good.Coefficient));
                 */
                X = X + (X * Good.SupplyPrice(-X, Population) - Goal) / DProduction(X, Good.LivingStandard, Good.Supply, Good.Demand, -1, Good.Exponent, Population * Good.Coefficient * Good.Decay);
            }
            return X;
        }
    }
}

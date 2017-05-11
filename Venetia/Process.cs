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
        Pair<Tangible, double>[] _Input;
        Pair<Tangible, double>[] _Output;

        public Pair<Tangible, double>[] Input { get { return _Input; } }
        public Pair<Tangible, double>[] Output { get { return _Output; } }

        public Process(ParseBlock Block, EconomySet<Tangible> Tangibles)
        {
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.ToLower())
                {
                    case "in": _Input = ReadTangibleSet(B, Tangibles).ToArray(); break;
                    case "out": _Output = ReadTangibleSet(B, Tangibles).ToArray(); break;
                }
            }
        }

        private List<Pair<Tangible, double>> ReadTangibleSet(ParseBlock Block, EconomySet<Tangible> Tangibles)
        {
            List<Pair<Tangible, double>> R = new List<Pair<Tangible, double>>();
            foreach (ParseBlock B in Block.Break())
            {
                Tangible T = Tangibles[B.Name];
                double Q = Convert.ToDouble(B.String, System.Globalization.CultureInfo.InvariantCulture);
                R.Add(new Pair<Tangible, double>(T, Q));
            }
            return R;
        }

        public Pair<double, double> OptimumSupply(Zone Zone)
        {
            Pair<EconomicAttributes, double>[] I = new Pair<EconomicAttributes, double>[_Input.Length];
            Pair<EconomicAttributes, double>[] O = new Pair<EconomicAttributes, double>[_Output.Length];
            for (int i = 0; i < I.Length; ++i) I[i] = new Pair<EconomicAttributes, double>(Zone[_Input[i].First], _Input[i].Second);
            for (int i = 0; i < O.Length; ++i) O[i] = new Pair<EconomicAttributes, double>(Zone[_Output[i].First], _Output[i].Second);
            return Calculator.OptimumSupply(I, O, Zone.Population);
        }

        public void Debug(Zone Zone)
        {
            foreach (Pair<Tangible, double> T in _Input) Console.WriteLine("{0} {1} {2}", T.First.Name, Zone[T.First], T.Second);
            Console.WriteLine("=>");
            foreach (Pair<Tangible, double> T in _Output) Console.WriteLine("{0} {1} {2}", T.First.Name, Zone[T.First], T.Second);
        }

        public override string ToString()
        {
            string S = "";
            foreach (Pair<Tangible, double> T in _Input) S += T.First.Name + ", ";
            S += " => ";
            foreach (Pair<Tangible, double> T in _Output) S += T.First.Name + ", ";
            return S;
        }
    }
}

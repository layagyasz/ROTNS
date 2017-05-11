using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Serialization;
using Cardamom.Utilities;
using Cardamom.Utilities.StableMatching;

namespace AndrassyII.LanguageGenerator
{
    class Matched
    {
        string _Name;
        bool _Vowel;
        bool _Consonant;
        bool _Alternate;
        double[] _ConsonantX;
        double[] _ConsonantY;
        double[] _VowelX;
        double[] _VowelY;
        bool _SetAlternate;
        bool _Sibilant;
        bool _Liquid;
        double _Frequency;
        double _Drop;

        public string Name { get { return _Name; } set { _Name = value; } }
        public bool Vowel { get { return _Vowel; } set { _Vowel = value; } }
        public bool Consonant { get { return _Consonant; } set { _Consonant = value; } }
        public bool Alternate { get { return _Alternate; } set { _Alternate = value; _SetAlternate = true; } }
        public double[] ConsonantX { get { return _ConsonantX; } set { _ConsonantX = value; } }
        public double[] ConsonantY { get { return _ConsonantY; } set { _ConsonantY = value; } }
        public double[] VowelX { get { return _VowelX; } set { _VowelX = value; } }
        public double[] VowelY { get { return _VowelY; } set { _VowelY = value; } }
        public bool Sibilant { get { return _Sibilant; } set { _Sibilant = value; } }
        public bool Liquid { get { return _Liquid; } set { _Liquid = value; } }
        public double Frequency { get { return _Frequency; } set { _Frequency = value; } }
        public double Drop { get { return _Drop; } set { _Drop = value; } }

        public Matched() { }

        public Matched(Sound Sound)
        {
            _Name = Sound.ToString();
            List<double> ProtoConsonantX = new List<double>();
            List<double> ProtoConsonantY = new List<double>();
            List<double> ProtoVowelX = new List<double>();
            List<double> ProtoVowelY = new List<double>();

            foreach (string Identifier in Sound.Identifiers)
            {
                switch (Identifier)
                {
                    case "consonant": _Consonant = true; break;
                    case "vowel": _Vowel = true; break;

                    case "bilabial": ProtoConsonantX.Add(0); break;
                    case "labiodental": ProtoConsonantX.Add(.1); break;
                    case "dental": ProtoConsonantX.Add(.3); break;
                    case "alveolar": ProtoConsonantX.Add(.4); break;
                    case "palatoalveolar": ProtoConsonantX.Add(.5); break;
                    case "retroflex": ProtoConsonantX.Add(.6); break;
                    case "palatal": ProtoConsonantX.Add(.8); break;
                    case "velar": ProtoConsonantX.Add(.9); break;
                    case "uvular": ProtoConsonantX.Add(1); break;
                    case "pharyngeal": ProtoConsonantX.Add(1.2); break;
                    case "glottal": ProtoConsonantX.Add(1.3); break;

                    case "labial": 
                        ProtoConsonantX.Add(0); 
                        ProtoConsonantX.Add(.1); 
                        break;
                    case "coronal":
                        ProtoConsonantX.Add(.3);
                        ProtoConsonantX.Add(.4);
                        ProtoConsonantX.Add(.5);
                        ProtoConsonantX.Add(.6);
                        break;
                    case "dorsal":
                        ProtoConsonantX.Add(.8);
                        ProtoConsonantX.Add(.9);
                        ProtoConsonantX.Add(1.0);
                        break;
                    case "laryngeal":
                        ProtoConsonantX.Add(1.2);
                        ProtoConsonantX.Add(1.3);
                        break;

                    case "nasal": ProtoConsonantY.Add(0); break;
                    case "plosive": ProtoConsonantY.Add(.1); break;
                    case "fricative": ProtoConsonantY.Add(.2); break;
                    case "approximant": ProtoConsonantY.Add(.3); break;
                    case "trill": ProtoConsonantY.Add(.4); break;
                    case "flap": ProtoConsonantY.Add(.5); break;
                    case "tap": ProtoConsonantY.Add(.5); break;
                    case "lateralfricative": ProtoConsonantY.Add(.6); break;
                    case "lateralapproximant": ProtoConsonantY.Add(.7); break;
                    case "lateralflap": ProtoConsonantY.Add(.8); break;

                    case "front": ProtoVowelX.Add(0); break;
                    case "central": ProtoVowelX.Add(.5); break;
                    case "back": ProtoVowelX.Add(1); break;

                    case "closed": ProtoVowelY.Add(0); break;
                    case "midclosed": ProtoVowelY.Add(.3333333); break;
                    case "midopen": ProtoVowelY.Add(.66666667); break;
                    case "open": ProtoVowelY.Add(1); break;

                    case "voiced": _Alternate = true; _SetAlternate = true; break;
                    case "unvoiced": _Alternate = false; _SetAlternate = true; break;

                    case "rounded": _Alternate = true; _SetAlternate = true; break;
                    case "unrounded": _Alternate = false; _SetAlternate = true; break;

                    case "sibilant": _Sibilant = true; break;
                    case "liquid": _Liquid = true; break;
                }
            }

            _ConsonantX = ProtoConsonantX.Count > 0 ? ProtoConsonantX.ToArray() : null;
            _ConsonantY = ProtoConsonantX.Count > 0 ? ProtoConsonantY.ToArray() : null;
            _VowelX = ProtoConsonantX.Count > 0 ? ProtoVowelX.ToArray() : null;
            _VowelY = ProtoConsonantX.Count > 0 ? ProtoVowelY.ToArray() : null;

            _Frequency = Sound.Frequency;
            _Drop = Sound.Drop;
        }

        public virtual double GetPreference(Matched Matched)
        {
            if (Matched is ModifiedMatched) return Matched.GetPreference(this);

            double P = 0;
            if (_Vowel == Matched.Vowel && _Vowel)
            {
                Pair<double, double> CX = ClosestPair(_VowelX, Matched.VowelX);
                Pair<double, double> CY = ClosestPair(_VowelY, Matched.VowelY);
                P += (1 - (Math.Abs(CX.First - CX.Second) + Math.Abs(CY.First - CY.Second)));
            }
            if(_Consonant == Matched.Consonant && _Consonant)
            {
                Pair<double, double> CX = ClosestPair(_ConsonantX, Matched.ConsonantX);
                Pair<double, double> CY = ClosestPair(_ConsonantY, Matched.ConsonantY);
                P += (1 - (Math.Abs(CX.First - CX.Second) + Math.Abs(CY.First - CY.Second)));
            }
            if (_Alternate == Matched.Alternate) P += .1;
            if (_Sibilant == Matched._Sibilant) P += .4;
            if (_Liquid == Matched.Liquid) P += .4;
            return P - Matched.Drop;
        }

        public ModifiedMatched Combine(Matched Matched)
        {
            if (_Vowel == Matched.Vowel || _Consonant == Matched.Consonant)
            {
                ModifiedMatched New = new ModifiedMatched(this, Matched);
                New.Name = _Name + Matched.Name;
                New.Vowel = _Vowel && Matched.Vowel;
                New.Consonant = _Consonant && Matched.Consonant;
                if (New.Consonant)
                {
                    New.ConsonantX = Matched.ConsonantX == null ? _ConsonantX : Matched.ConsonantX;
                    New.ConsonantY = Matched.ConsonantY == null ? _ConsonantY : Matched.ConsonantY;
                }
                if (New.Vowel)
                {
                    New.VowelX = Matched.VowelX == null ? _VowelX : Matched.VowelX;
                    New.VowelY = Matched.VowelY == null ? _VowelY : Matched.VowelY;
                }
                if (Matched._SetAlternate)
                {
                    New.Alternate = Matched.Alternate;
                }
                return New;
            }
            else return null;
        }

        private Pair<double, double> ClosestPair(double[] Arr1, double[] Arr2)
        {
            double Dist = double.MaxValue;
            Pair<double, double> P = new Pair<double, double>(0, 0);
            for (int i = 0; i < Arr1.Length; ++i)
            {
                for (int j = 0; j < Arr2.Length; ++j)
                {
                    double d = Math.Abs(Arr1[i] - Arr2[j]);
                    if (d < Dist)
                    {
                        Dist = d;
                        P.First = Arr1[i];
                        P.Second = Arr2[j];
                    }
                }
            }
            return P;
        }

        public override string ToString()
        {
            return "[Matched]" + _Name;
        }
    }
}

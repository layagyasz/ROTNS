using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ROTNS.Model.Flags;

using Venetia;

namespace ROTNS.Model
{
    public class Culture
    {
        float _Individualism;
        float _Indulgence;
        float _LongTermOrientation;
        float _PowerDistance;
        float _Toughness;
        float _UncertaintyAvoidance;
        FlagColor[] _Colors;

        public float Individualism { get { return _Individualism; } set { _Individualism = value; } }
        public float Indulgence { get { return _Indulgence; } set { _Indulgence = value; } }
        public float LongTermOrientation { get { return _LongTermOrientation; } set { _LongTermOrientation = value; } }
        public float PowerDistance { get { return _PowerDistance; } set { _PowerDistance = value; } }
        public float Toughness { get { return _Toughness; } set { _Toughness = value; } }
        public float UncertaintyAvoidance { get { return _UncertaintyAvoidance; } set { _UncertaintyAvoidance = value; } }
        public FlagColor[] Colors { get { return _Colors; } set { _Colors = value; } }

        public Culture() { }

        public Culture(float Individualism, float Indulgence, float LongTermOrientation, float PowerDistance, float Toughness, float UncertaintyAvoidance)
        {
            _Individualism = Individualism;
            _Indulgence = Indulgence;
            _LongTermOrientation = LongTermOrientation;
            _PowerDistance = PowerDistance;
            _Toughness = Toughness;
            _UncertaintyAvoidance = UncertaintyAvoidance;
        }

        public double DistanceTo(Culture Culture)
        {
            float IndividualismDifference = _Individualism - Culture._Individualism;
            float IndulgenceDifference = _Indulgence - Culture._Indulgence;
            float LongTermOrientationDifference = _LongTermOrientation - Culture._LongTermOrientation;
            float PowerDistanceDifference = _PowerDistance - Culture._PowerDistance;
            float ToughnessDifference = _Toughness - Culture._Toughness;
            float UncertaintyAvoidanceDifference = _UncertaintyAvoidance - Culture._UncertaintyAvoidance;

            return Math.Sqrt((IndividualismDifference * IndividualismDifference) 
                + (IndulgenceDifference * IndulgenceDifference)
                + (LongTermOrientationDifference * LongTermOrientationDifference)
                + (PowerDistanceDifference * PowerDistanceDifference)
                + (ToughnessDifference * ToughnessDifference)
                + (UncertaintyAvoidanceDifference * UncertaintyAvoidanceDifference));
        }

        public Service CalculateLabor()
        {
            float B = ((1 - _Individualism) + _Toughness + _LongTermOrientation) / 3;
            return new Service("labor", 1, .3 - (1 - B) / 10);
        }

        public double TaxTolerance()
        {
            return .25 * Math.Max(_PowerDistance, 1 - _Individualism) + .125 * Math.Min(_PowerDistance, 1 - _Individualism);
        }

        public double MilitaryTolerance()
        {
            double High = Math.Max(_UncertaintyAvoidance * .75, Math.Max(1.5 * _Toughness, _PowerDistance));
            double Mid = Math.Min(Math.Max(_UncertaintyAvoidance * .75, 1.5 * _Toughness), _PowerDistance);
            double Low = Math.Min(_UncertaintyAvoidance * .75, Math.Min(1.5 * _Toughness, _PowerDistance));

            return .78 * (High * .12 + Mid * .06 + Low * .02);
        }

        public double RestrictionTolerance()
        {
            double High = Math.Max(_Indulgence * .75, Math.Max(1 - _Individualism, 1.5 * _PowerDistance));
            double Mid = Math.Min(Math.Max(_Indulgence * .75, 1 - _Individualism), 1.5 * _PowerDistance);
            double Low = Math.Min(_Indulgence * .75, Math.Min(1 - _Individualism, 1.5 * _PowerDistance));

            return .78 * (High * .6 + Mid * .3 + Low * .1);
        }

        public double ScientificProductionProportion()
        {
            return (_Individualism + Math.Min(_LongTermOrientation, 1 - _UncertaintyAvoidance)) * .025;
        }

		public double Favorability(GovernmentForm Government)
		{
			double Total = 0;

			if (Government.Devolved) Total += 1 - _PowerDistance;
			else Total += _PowerDistance;

			if (Government.Integrated) Total += _UncertaintyAvoidance;
			else Total += (1 - _UncertaintyAvoidance);

			if (Government.RegionControl) Total += (1 - _Individualism);
			else Total += _Individualism;

			if (Government.SubRegionControl) Total += Math.Sqrt(1 - _Individualism);
			else Total += Math.Sqrt(_Individualism);

			if (!Government.Tributary) Total += 1;

			if (Government.UnanimousConsent) Total += 1 - _PowerDistance;
			else Total += _PowerDistance;

			if (Government.Voluntary) Total += 1 - _Toughness;
			else Total += _Toughness;
				
			return Total;
		}

		public double Explorativity()
		{
			return Math.Pow(_Individualism * (1 - _UncertaintyAvoidance) * _Toughness, .33);
		}

		public Culture GenerateIndividual(Random Random)
		{
			return new Culture(
				Clamp(Gaussian(Random, _Individualism, _Individualism)),
				Clamp(Gaussian(Random, _Indulgence, _Individualism)),
				Clamp(Gaussian(Random, _LongTermOrientation, _Individualism)),
				Clamp(Gaussian(Random, _PowerDistance, _Individualism)),
				Clamp(Gaussian(Random, _Toughness, _Individualism)),
				Clamp(Gaussian(Random, _UncertaintyAvoidance, _Individualism))
			);
		}

		static float Gaussian(Random Random, float Mean, float StandardDeviation)
		{
			double u1 = 1.0 - Random.NextDouble();
			double u2 = 1.0 - Random.NextDouble();
			double StdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
			return (float)(Mean + StandardDeviation * StdNormal);
		}

		static float Clamp(double V)
		{
			return (float)Math.Max(0, Math.Min(1, V));
		}

    }
}

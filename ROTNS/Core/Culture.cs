using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Venetia;

namespace ROTNS.Core
{
    public class Culture
    {
        float _Individualism;
        float _Indulgence;
        float _LongTermOrientation;
        float _PowerDistance;
        float _Toughness;
        float _UncertaintyAvoidance;

        public float Individualism { get { return _Individualism; } set { _Individualism = value; } }
        public float Indulgence { get { return _Indulgence; } set { _Indulgence = value; } }
        public float LongTermOrientation { get { return _LongTermOrientation; } set { _LongTermOrientation = value; } }
        public float PowerDistance { get { return _PowerDistance; } set { _PowerDistance = value; } }
        public float Toughness { get { return _Toughness; } set { _Toughness = value; } }
        public float UncertaintyAvoidance { get { return _UncertaintyAvoidance; } set { _UncertaintyAvoidance = value; } }

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
            return new Service("labor", 1, .5 - (1 - B) / 5);
        }
    }
}

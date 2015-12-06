using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Venetia
{
    public class EconomicAttributes
    {
        protected double _Coefficient;
        protected double _Exponent;
        protected double _Decay = 1;
        protected double _Supply;
        protected double _Demand;
        protected double _LivingStandard;

        public double Coefficient { get { return _Coefficient; } set { _Coefficient = value; } }
        public double Exponent { get { return _Exponent; } set { _Exponent = value; } }
        public double Decay { get { return _Decay; } set { _Decay = value; } }
        public double Supply { get { return _Supply; } set { _Supply = value; } }
        public double Demand { get { return _Demand; } set { _Demand = value; } }
        public double LivingStandard { get { return _LivingStandard; } set { _LivingStandard = value; } }

        public EconomicAttributes(double Coefficient, double Exponent, double Decay, double Supply, double Demand, double LivingStandard)
        {
            _Coefficient = Coefficient;
            _Exponent = Exponent;
            _Decay = Decay;
            _Supply = Supply;
            _Demand = Demand;
            _LivingStandard = LivingStandard;
        }

        public EconomicAttributes(double Coefficient, double Exponent, double Decay, double LivingStandard)
        {
            _Coefficient = Coefficient;
            _Exponent = Exponent;
            _LivingStandard = LivingStandard;
            _Decay = Decay;
        }

        public EconomicAttributes(Tangible Tangible, double LivingStandard)
        {
            _Coefficient = Tangible.Coefficient;
            _Exponent = Tangible.Exponent;
            _LivingStandard = LivingStandard;
            _Decay = Decay;
        }

        public virtual double MaxNewProduction(double Population)
        {
            return (_Coefficient * Population) - (_Supply - _Demand);
        }

        public virtual double Price(double Population)
        {
            return _LivingStandard * Math.Log((_Supply - _Demand) / (_Coefficient * Population)) / Math.Log(_Exponent);
        }

        public virtual double SupplyPrice(double IncreaseSupply, double Population)
        {
            return _LivingStandard * Math.Log((_Supply + IncreaseSupply - _Demand) / (_Coefficient * Population)) / Math.Log(_Exponent);
        }

        public void Smooth(double Constant, EconomicAttributes Upper)
        {
            _Supply = Constant * Upper._Supply + (1 - Constant) * _Supply;
            _Demand = Constant * Upper._Demand + (1 - Constant) * _Demand;
        }

        public override string ToString()
        {
            return String.Format("M = {0}, K = {1}, S = {2}, D = {3}, L = {4}", _Coefficient, _Exponent, _Supply, _Demand, _LivingStandard);
        }
    }
}

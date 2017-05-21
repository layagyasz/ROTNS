using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Venetia
{
	public class DepositAttributes : EconomicAttributes
	{

		public DepositAttributes(double Coefficient, double Exponent, double Decay, double Supply, double Demand, double LivingStandard)
			: base(Coefficient, Exponent, Decay, Supply, Demand, LivingStandard) { }

		public DepositAttributes(Resource Resource, double LivingStandard)
			: base(Resource, LivingStandard) { }

		public override double MaxNewProduction(double Population)
		{
			return (_Coefficient * _Supply) - (_Supply - _Demand);
		}

		public override double Price(double Population)
		{
			return _LivingStandard * Math.Log((_Supply - _Demand) / (_Coefficient * _Supply)) / Math.Log(_Exponent);
		}

		public override double SupplyPrice(double IncreaseSupply, double Population)
		{
			return _LivingStandard * Math.Log((_Supply + IncreaseSupply - _Demand) / (_Coefficient * _Supply)) / Math.Log(_Exponent);
		}

	}
}

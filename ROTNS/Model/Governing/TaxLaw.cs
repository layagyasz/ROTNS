using System;
using System.Collections.Generic;
using System.Linq;

using ROTNS.Model;

using Venetia;

namespace ROTNS
{
	public class TaxLaw : Law
	{
		readonly Tangible _Tangible;
		readonly double _Amount;

		public TaxLaw(Tangible Tangible, double Amount)
		{
			_Tangible = Tangible;
			_Amount = Amount;
		}

		public double Collect(Region Region)
		{
			return Region.Market.Where(i => i.First == _Tangible).Sum(i => Region.Price(i.First) * _Amount);
		}

		public override void Apply(Region Region)
		{
			Region.ChangeIncomeReduction(_Tangible, _Amount);
		}

		public override void Update(Region Region)
		{
			return;
		}
	}
}

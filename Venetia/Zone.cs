using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities;

namespace Venetia
{
	public class Zone
	{
		Dictionary<Tangible, EconomicAttributes> _Market = new Dictionary<Tangible, EconomicAttributes>();
		Economy _Economy;

		HashSet<Producer> _Producers = new HashSet<Producer>();
		List<Trade> _Trades = new List<Trade>();

		double _SmoothingConstant;
		double _Population;
		double _LivingStandard = 1;

		public EconomicAttributes this[Tangible Tangible] { get { AddTangibleIfNotExists(Tangible); return _Market[Tangible]; } }
		public Economy Economy
		{
			get
			{
				return _Economy;
			}
		}
		public double Population
		{
			get
			{
				return _Population;
			}
			set
			{
				_Population = value;
				_Market[_Economy.Labor].Supply = value;
			}
		}
		public IEnumerable<KeyValuePair<Tangible, EconomicAttributes>> Market
		{
			get
			{
				return _Market.AsEnumerable();
			}
		}

		public Zone(Service Labor, Economy Economy)
		{
			_Market.Add(Economy.Labor, new EconomicAttributes(Labor, _LivingStandard));
			_Economy = Economy;
		}

		public double Price(string Tangible)
		{
			return Price(_Economy[Tangible]);
		}

		public double Price(Tangible Tangible)
		{
			return this[Tangible].Price(_Population);
		}

		public double FlowFrom(string Tangible)
		{
			return FlowFrom(_Economy[Tangible]);
		}

		public double FlowFrom(Tangible Tangible)
		{
			return this[Tangible].Price(_Population) * this[Tangible].Supply;
		}

		public double SupplyPrice(Tangible Tangible, double Amount)
		{
			if (_Market.ContainsKey(Tangible))
				return _Market[Tangible].SupplyPrice(Amount, _Population);
			else
				return Double.PositiveInfinity;
		}

		public double Flow()
		{
			double f = 0;
			foreach (Producer P in _Producers)
			{
				foreach (Tuple<Tangible, double> O in P.Process.Output)
				{
					double p = _Market[O.Item1].Price(_Population);
					if (!double.IsInfinity(p) && !double.IsNaN(p) && p > 0)
						f += O.Item2 * P.Scale * p;
				}
			}
			foreach (Trade T in _Trades)
			{
				if (this == T.Zone1)
					f += T.Flow1;
				else
					f += T.Flow2;
			}
			return f;
		}

		private void AddTangibleIfNotExists(Tangible Tangible)
		{
			if (!_Market.ContainsKey(Tangible))
			{
				if (Tangible is Resource)
					_Market.Add(Tangible, new DepositAttributes(Tangible.Coefficient, Tangible.Exponent, Tangible.Decay, 0, 0, _LivingStandard));
				else
					_Market.Add(Tangible, new EconomicAttributes(Tangible.Coefficient, Tangible.Exponent, Tangible.Decay, 0, 0, _LivingStandard));
			}
		}

		public void ChangeSupply(Tangible Tangible, double Delta)
		{
			AddTangibleIfNotExists(Tangible);
			_Market[Tangible].Supply += Delta;
		}

		public void ChangeDemand(Tangible Tangible, double Delta)
		{
			AddTangibleIfNotExists(Tangible);
			_Market[Tangible].Demand += Delta;
		}

		public void ChangeIncomeReduction(Tangible Tangible, double Delta)
		{
			AddTangibleIfNotExists(Tangible);
			_Market[Tangible].IncomeReduction += Delta;
		}

		public void AddProducer(Producer Producer)
		{
			if (!_Producers.Contains(Producer))
				_Producers.Add(Producer);
			foreach (Tuple<Tangible, double> I in Producer.Process.Input)
				ChangeDemand(I.Item1, I.Item2 * Producer.Scale);
			foreach (Tuple<Tangible, double> O in Producer.Process.Output)
				ChangeSupply(O.Item1, O.Item2 * Producer.Scale);
		}

		public void AddTrade(Trade Trade)
		{
			_Trades.Add(Trade);
			if (this == Trade.Zone1)
			{
				ChangeDemand(Trade.Good1, Trade.Amount1);
				ChangeSupply(Trade.Good2, Trade.Amount2);
			}
			else
			{
				ChangeSupply(Trade.Good1, Trade.Amount1);
				ChangeDemand(Trade.Good2, Trade.Amount2);
			}
		}

		public void RemoveProducer(Producer Producer)
		{
			_Producers.Remove(Producer);
			RemoveProducerEffect(Producer);
		}

		private void RemoveProducerEffect(Producer Producer)
		{
			foreach (Tuple<Tangible, double> I in Producer.Process.Input)
				ChangeDemand(I.Item1, -I.Item2 * Producer.Scale);
			foreach (Tuple<Tangible, double> O in Producer.Process.Output)
				ChangeSupply(O.Item1, -O.Item2 * Producer.Scale);
		}

		public Triplet<Process, double, double> BestAddition()
		{
			Triplet<Process, double, double> R = new Triplet<Process, double, double>(null, 0, 0);
			foreach (Process Process in _Economy.Processes)
			{
				Pair<double, double> D = Process.OptimumSupply(this);
				if ((R.First == null || D.Second > R.Third) && !double.IsNaN(R.Second))
					R = new Triplet<Process, double, double>(Process, D.First, D.Second);
			}
			return R;
		}

		public Trade BestTrade(Zone Neighbor, IEnumerable<Good> PermittedGoods)
		{
			Triplet<double, double, double> O = new Triplet<double, double, double>(0, 0, 0);
			Good Good1 = null;
			Good Good2 = null;

			foreach (Good G1 in PermittedGoods)
			{
				foreach (Good G2 in PermittedGoods)
				{
					Triplet<double, double, double> T = Calculator.OptimumTrade(
						this[G1],
						Neighbor[G1],
						this[G2],
						Neighbor[G2],
						this[Economy.Labor],
						_Population,
						Neighbor.Population);
					if (Good1 == null || T.Third > O.Third && T.First > 0 && T.Second > 0)
					{
						O = T;
						Good1 = G1;
						Good2 = G2;
					}
				}
			}

			return new Trade(Good1, Good2, O.First, O.Second, this, Neighbor);
		}

		public Producer RescaleProducer(Producer Producer, double Scale)
		{
			RemoveProducer(Producer);
			if (Scale > 0)
			{
				Producer = new Producer(Producer.Process, Scale);
				AddProducer(Producer);
				return Producer;
			}
			else
				return null;
		}

		public Triplet<Producer, double, double> BestRemoval()
		{
			Triplet<Producer, double, double> R = new Triplet<Producer, double, double>(null, 0, 0);
			foreach (Producer P in _Producers)
			{
				Pair<double, double> Profit = P.Profit(this);
				double CurrentRatio = Profit.First / Profit.Second;
				RemoveProducerEffect(P);
				Pair<double, double> D = P.Process.OptimumSupply(this);
				double NewRatio = (D.Second / D.First) / CurrentRatio;
				if (R.First == null || NewRatio < R.Third)
					R = new Triplet<Producer, double, double>(P, D.Second, NewRatio);
				AddProducer(P);
			}
			return R;
		}

		public void Update()
		{
		}
	}
}

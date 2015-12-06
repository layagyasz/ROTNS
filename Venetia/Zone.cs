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
        List<Zone> _NeighborZones = new List<Zone>();
        Economy _Economy;

        HashSet<Producer> _Producers = new HashSet<Producer>();
        List<Trade> _Trades = new List<Trade>();

        double _SmoothingConstant;
        double _Population;
        double _Area;
        double _LivingStandard = 1;

        public EconomicAttributes this[Tangible Tangible] { get { return _Market[Tangible]; } }
        public Economy Economy { get { return _Economy; } }
        public List<Zone> NeighborZones { get { return _NeighborZones; } set { _NeighborZones = value; } }
        public double Area { get { return _Area; } set { _Area = value; _Market[_Economy.Property].Supply = value; } }
        public double Population { get { return _Population; } set { _Population = value; _Market[_Economy.Labor].Supply = value; } }
        public IEnumerator<Pair<Tangible, EconomicAttributes>> Market { get { foreach (KeyValuePair<Tangible, EconomicAttributes> P in _Market) yield return new Pair<Tangible, EconomicAttributes>(P.Key, P.Value); } }

        public Zone(Service Labor, Resource Property, Economy Economy)
        {
            _Market.Add(Economy.Labor, new EconomicAttributes(Labor, _LivingStandard));
            _Market.Add(Economy.Property, new DepositAttributes(Property, _LivingStandard));
            _Economy = Economy;
        }

        public double SupplyPrice(Tangible Tangible, double Amount)
        {
            if (_Market.ContainsKey(Tangible)) return _Market[Tangible].SupplyPrice(Amount, _Population);
            else return Double.PositiveInfinity;
        }

        public double Flow()
        {
            double f = 0;
            foreach (Producer P in _Producers)
            {
                foreach (Pair<Tangible, double> O in P.Process.Output)
                {
                    double p = _Market[O.First].Price(_Population);
                    if (!double.IsInfinity(p) && !double.IsNaN(p) && p > 0) f += O.Second * P.Scale * p;
                }
            }
            foreach (Trade T in _Trades)
            {
                if (this == T.Zone1) f += T.Flow1;
                else f += T.Flow2;
            }
            return f;
        }

        protected void AddSupply(Tangible Tangible, double Amount)
        {
            if(_Market.ContainsKey(Tangible)) _Market[Tangible].Supply += Amount;
            else if (Tangible is Resource) _Market.Add(Tangible, new DepositAttributes(Tangible.Coefficient, Tangible.Exponent, Tangible.Decay, Amount, 0, _LivingStandard));
            else _Market.Add(Tangible, new EconomicAttributes(Tangible.Coefficient, Tangible.Exponent, Tangible.Decay, Amount, 0, _LivingStandard));
        }

        protected void AddDemand(Tangible Tangible, double Amount)
        {
            if (_Market.ContainsKey(Tangible)) _Market[Tangible].Demand += Amount;
            else if (Tangible is Resource) _Market.Add(Tangible, new DepositAttributes(Tangible.Coefficient, Tangible.Exponent, Tangible.Decay, 0, Amount, _LivingStandard));
            else _Market.Add(Tangible, new EconomicAttributes(Tangible.Coefficient, Tangible.Exponent, Tangible.Decay, 0, Amount, _LivingStandard));
        }

        public void AddProducer(Producer Producer)
        {
            if(!_Producers.Contains(Producer)) _Producers.Add(Producer);
            foreach (Pair<Tangible, double> I in Producer.Process.Input) AddDemand(I.First, I.Second * Producer.Scale);
            foreach (Pair<Tangible, double> O in Producer.Process.Output) AddSupply(O.First, O.Second * Producer.Scale);
        }

        public void AddTrade(Trade Trade)
        {
            _Trades.Add(Trade);
            if (this == Trade.Zone1)
            {
                AddDemand(Trade.Good1, Trade.Amount1);
                AddSupply(Trade.Good2, Trade.Amount2);
            }
            else
            {
                AddSupply(Trade.Good1, Trade.Amount1);
                AddDemand(Trade.Good2, Trade.Amount2);
            }
        }

        public void RemoveProducer(Producer Producer)
        {
            _Producers.Remove(Producer);
            RemoveProducerEffect(Producer);
        }

        private void RemoveProducerEffect(Producer Producer)
        {
            foreach (Pair<Tangible, double> I in Producer.Process.Input) AddDemand(I.First, -I.Second * Producer.Scale);
            foreach (Pair<Tangible, double> O in Producer.Process.Output) AddSupply(O.First, -O.Second * Producer.Scale);
        }

        public Triplet<Process, double, double> BestAddition()
        {
            Triplet<Process, double, double> R = new Triplet<Process,double,double>(null, 0,0);
            IEnumerator<Process> Processes = _Economy.Processes;
            while(Processes.MoveNext())
            {
                Pair<double, double> D = Processes.Current.OptimumSupply(this);
                if ((R.First == null || Double.IsNaN(R.Second) || D.Second > R.Third)) R = new Triplet<Process, double, double>(Processes.Current, D.First, D.Second);
            }
            return R;
        }

        public Trade BestTrade()
        {
            Triplet<double, double, double> O = new Triplet<double, double, double>(0, 0, 0);
            Good Good1 = null;
            Good Good2 = null;
            Zone N = null;

            foreach (Zone Z in _NeighborZones)
            {
                IEnumerator<Good> G = _Economy.Goods;
                while (G.MoveNext())
                {
                    IEnumerator<Good> G2 = _Economy.Goods;
                    while (G2.MoveNext())
                    {
                        Triplet<double, double, double> T = Calculator.OptimumTrade(this[G.Current], Z[G.Current], this[G2.Current], Z[G2.Current], this[Economy.Labor], _Population, Z.Population);
                        if (Good1 == null || T.Third > O.Third && T.First > 0 && T.Second > 0)
                        {
                            O = T;
                            Good1 = G.Current;
                            Good2 = G2.Current;
                            N = Z;
                            //Console.WriteLine("{0} -> {1} : {2} {3} {4}", G.Current.Name, G2.Current.Name, T.First, T.Second, T.Third);
                        }
                    }
                }
            }

            return new Trade(Good1, Good2, O.First, O.Second, this, N);
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
            else return null;
        }

        public Triplet<Producer, double, double> BestRemoval()
        {
            Triplet<Producer, double, double> R = new Triplet<Producer, double, double>(null, 0,0);
            foreach (Producer P in _Producers)
            {
                Pair<double, double> Profit = P.Profit(this);
                double CurrentRatio = Profit.First / Profit.Second;
                RemoveProducerEffect(P);
                Pair<double, double> D = P.Process.OptimumSupply(this);
                double NewRatio = (D.Second / D.First) / CurrentRatio;
                if (R.First == null || NewRatio < R.Third) R = new Triplet<Producer, double, double>(P, D.Second, NewRatio);
                AddProducer(P);
            }
            return R;
        }

        public void Update()
        {
            EconomicAttributes Labor = _Market[_Economy.Labor];
            Labor.Demand = 0;
            EconomicAttributes Property = _Market[_Economy.Property];
            Property.Demand = 0;
            Dictionary<Tangible, EconomicAttributes> NewMarket = new Dictionary<Tangible, EconomicAttributes>();
            NewMarket.Add(_Economy.Labor, Labor);
            NewMarket.Add(_Economy.Property, Property);
            foreach (KeyValuePair<Tangible, EconomicAttributes> P in _Market)
            {
                if (P.Key is Resource && P.Key != _Economy.Property)
                {
                    P.Value.Demand = 0;
                    NewMarket.Add(P.Key, P.Value);
                }
            }
            _Market = NewMarket;
            IEnumerator<Tangible> I = _Economy.All;
            while (I.MoveNext()) AddDemand(I.Current, I.Current.Minimum * _Population);
            foreach (Producer P in _Producers) AddProducer(P);
            List<Trade> Trades = _Trades;
            _Trades = new List<Trade>();
            foreach (Trade T in Trades) AddTrade(T);
        }

        private void PrintProducer(Producer Producer)
        {
            Console.WriteLine(Producer.Process);
            foreach (Pair<Tangible, double> I in Producer.Process.Input) Console.WriteLine(_Market[I.First]);
            foreach (Pair<Tangible, double> O in Producer.Process.Output) Console.WriteLine(_Market[O.First]);
        }
    }
}

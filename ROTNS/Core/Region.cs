using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Venetia;
using Cardamom.Utilities;
using ROTNS.OverMapping;
using Cardamom.Graphing;

namespace ROTNS.Core
{
    public class Region : Zone, Pathable, DijkstraRegion<MicroRegion>
    {
        string _Name;
        MapRegion _Marker;
        Nation _Nation;
        Culture _Culture;
        MicroRegion _Center;

        RegionAdministration _Administration;

        Dictionary<Region, bool> _Neighbors = new Dictionary<Region, bool>();
        List<MicroRegion> _Regions = new List<MicroRegion>();
        bool _Coastal;
        double _ShortestEdge = float.MaxValue;

        public string Name { get { return _Name; } }
        public MicroRegion Center { get { return _Center; } }
        public List<MicroRegion> Regions { get { return _Regions; } }
        public RegionAdministration Administration { get { return _Administration; } }
        public MapRegion Marker { get { return _Marker; } set { _Marker = value; } }
        public Culture Culture { get { return _Culture; } }
        public Nation Nation { get { return _Nation; } set { _Nation = value; } }
        public float PopulationDensity { get { return (float)(Population / Area); } }
        public bool Coastal { get { return _Coastal; } }
        public double FPC { get { return Flow() / Population; } }

        //DijkstraPool and Pathable

        public double StartDistance { get { return 0; } }
        public bool Passable { get { return true; } }

        public double DistanceTo(Pathable Neighbor)
        {
            Region R = (Region)Neighbor;
            double dX = R._Center.X - _Center.X;
            double dY = R._Center.Y - _Center.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }
        public IEnumerator<Pathable> Neighbors() { foreach (Region P in _Neighbors.Keys) yield return P; }

        public void Add(MicroRegion Region)
        {
            _Regions.Add(Region);
            Region.Region = this;
            if (!Region.Oceanic) IncreaseSize();
        }

        //End

        public Region(string Name, MicroRegion Center, Culture Culture, Economy Economy)
            : base(Culture.CalculateLabor(), new Resource("property", 1, .3), Economy)
        {
            _Name = Name;
            _Center = Center;
            _Culture = Culture;
            _Administration = new RegionAdministration(this);
        }

        public void AddPopulation(float Population) { this.Population += 100 * Population; }

        public void AddResource(NaturalResource Resource, float Amount)
        {
            AddSupply(Resource, _Regions.Count * Amount);
        }

        public float GetResourceDensity(NaturalResource Resource) { return (float)(this[Resource].Supply / Area); }

        public void MakeCoastal() { _Coastal = true; }
        public void IncreaseSize() { Area+=10000; }

        public bool IsNeighbor(Region Region) { return _Neighbors.ContainsKey(Region); }
        public void AddNeighbor(Region Region, bool Oceanic) { _Neighbors.Add(Region, Oceanic); NeighborZones.Add(Region); if(DistanceTo(Region) < _ShortestEdge) _ShortestEdge = DistanceTo(Region); }
        public void UpdateNeighbor(Region Region, bool Oceanic) { _Neighbors[Region] = Oceanic; }

        private string PriceDescriptor(double Price)
        {
            if (Price < 0) return "Worthless";
            else if (Double.IsNaN(Price)) return "Not Available";
            else if (Double.IsInfinity(Price)) return "Not Available";
            else
            {
                switch ((int)Math.Pow(Price, 1d / 3))
                {
                    case 0: return "Worthless";
                    case 1: return "Cheap";
                    case 2: return "Average";
                    case 3: return "Above Average";
                    case 4: return "Expensive";
                    default: return "Very Expensive";
                }
            }
        }

        public void InitializeEconomy()
        {
           
            Update();
            for (int i = 0; i < 100; ++i)
            {
                Triplet<Process, double, double> BA = BestAddition();
                Triplet<Producer, double, double> BR = BestRemoval();
                bool Continue = false;
                if (BA.First != null && BA.Third > 0 && BA.Second > 0) { Continue = true; AddProducer(new Producer(BA.First, BA.Second)); }
                if (BR.First != null && BR.First.Scale > BR.Second) { Continue = true; RescaleProducer(BR.First, BR.Second + (BR.First.Scale - BR.Second) / 2); }
                if (!Continue) break;
            }
             
           
            Console.WriteLine("#############################");
            Console.WriteLine("Name: {0}\n Population: {1:N0}\n GDP: {2:N2}", Char.ToUpper(Name[0]) + Name.Substring(1), Population, Flow());
            Console.WriteLine("-----------------------------");
            IEnumerator<Pair<Tangible,EconomicAttributes>> I = Market;
            if (this[Economy.Labor].Price(Population) > 0) Console.ForegroundColor = ConsoleColor.Green;
            else Console.ForegroundColor = ConsoleColor.Red;
            while (I.MoveNext())
            {
                if (!(I.Current.First is Resource))
                {
                    double P = I.Current.Second.Price(Population);
                    if (!Double.IsNaN(P) && !Double.IsInfinity(P))
                    {
                        Console.WriteLine("{0} = {1}", I.Current.First.Name, String.Format("{0:N2}", P));
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            
        }

        public void DiscoverBorder()
        {
            foreach (MicroRegion Current in _Regions)
            {
                IEnumerator<Pathable> it = Current.Neighbors();
                while (it.MoveNext())
                {
                    MicroRegion N = (MicroRegion)it.Current;
                    if (N.Region != this)
                    {
                        if (!IsNeighbor(N.Region)) AddNeighbor(N.Region, true);
                        if (!N.Oceanic && !Current.Oceanic) UpdateNeighbor(N.Region, false);
                    }
                    if (N.Oceanic && !Current.Oceanic) { MakeCoastal(); N.Coastal = true; }
                }
            }
        }

        public double Desirability(Region Region)
        {
            if (Region == this) return 0;

            double Economic = Math.Max(_Culture.Indulgence, _Culture.UncertaintyAvoidance) * (Region.FPC / FPC);
            double Territorial = Math.Max(_Culture.Toughness, _Culture.Individualism) * (Region.Area / Area);
            double Cultural = Math.Max(1 - _Culture.Individualism, _Culture.LongTermOrientation) / _Culture.DistanceTo(Region._Culture);
            return (Economic + Territorial + Cultural) * Math.Sqrt(_ShortestEdge / DistanceTo(Region));
        }

        public void Tick()
        {
            _Administration.Agent.Tick();
        }
    }
}

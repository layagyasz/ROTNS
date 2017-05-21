using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Venetia;
using Cardamom.Utilities;
using ROTNS.Model.GameEvents;
using ROTNS.Model.Governing;
using ROTNS.View;
using Cardamom.Graphing;

namespace ROTNS.Model
{
	public class Region : Zone, Pathable<Region>, DijkstraRegion<MicroRegion>, Ticked
	{
		public EventHandler<GameEvent> OnEvent;

		string _Name;
		Culture _Culture;
		MicroRegion _Center;
		double _Area;

		RegionAdministration _Administration;

		Dictionary<Region, bool> _Neighbors = new Dictionary<Region, bool>();
		List<MicroRegion> _Regions = new List<MicroRegion>();
		int _Coast;
		double _ShortestEdge = float.MaxValue;

		public string Name { get { return _Name; } }
		public MicroRegion Center { get { return _Center; } }
		public List<MicroRegion> Regions { get { return _Regions; } }
		public double Area { get { return _Area; } set { _Area = value; } }
		public RegionAdministration Administration { get { return _Administration; } }
		public Culture Culture { get { return _Culture; } }
		public float PopulationDensity { get { return (float)(Population / Area); } }
		public bool Coastal { get { return _Coast > 0; } }
		public int Coast { get { return _Coast; } }

		//DijkstraPool and Pathable
		public double StartDistance { get { return 0; } }
		public bool Passable { get { return true; } }

		// Ticked
		public bool Remove { get { return false; } }

		public double DistanceTo(Region Neighbor)
		{
			Region R = (Region)Neighbor;
			double dX = R._Center.X - _Center.X;
			double dY = R._Center.Y - _Center.Y;
			return Math.Sqrt(dX * dX + dY * dY);
		}
		public double HeuristicDistanceTo(Region Pathable)
		{
			return DistanceTo(Pathable);
		}
		public IEnumerable<Region> Neighbors() { foreach (Region P in _Neighbors.Keys) yield return P; }

		public void Add(MicroRegion Region)
		{
			_Regions.Add(Region);
			Region.Region = this;
			if (!Region.Oceanic) _Area += 10000;
			if (Region.Coastal) _Coast++;
		}

		//End

		public Region(string Name, MicroRegion Center, Culture Culture, Economy Economy)
			: base(Culture.CalculateLabor(), Economy)
		{
			_Name = Name;
			_Center = Center;
			_Culture = Culture;
			_Administration = new RegionAdministration(this);
		}

		public double CurrencyPrice(string Tangible, string Base)
		{
			return Price(Tangible) / Price(Base);
		}

		public void AddPopulation(World World, float Population)
		{
			float Avg = (float)(World.Height * World.Width) / World.Regions.Length;
			this.Population += Population * (Math.Sqrt(Avg * 5) * Math.Sqrt(Area / 500) + Math.Sqrt(_Coast) * 1000);
		}

		public void AddResource(NaturalResource Resource, float Amount)
		{
			ChangeSupply(Resource, _Regions.Count * Amount);
		}

		public float GetResourceDensity(NaturalResource Resource) { return (float)(this[Resource].Supply / Area); }

		public double FlowPerCapita() { return Flow() / Population; }
		public bool IsNeighbor(Region Region) { return _Neighbors.ContainsKey(Region); }
		public void AddNeighbor(Region Region, bool Oceanic) { _Neighbors.Add(Region, Oceanic); if (DistanceTo(Region) < _ShortestEdge) _ShortestEdge = DistanceTo(Region); }
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
			for (int i = 0; i < Economy.Processes.Count(); ++i)
			{
				Triplet<Process, double, double> BA = BestAddition();
				Triplet<Producer, double, double> BR = BestRemoval();
				bool Continue = false;
				if (BA.First != null && BA.Third > 0 && BA.Second > 0) { Continue = true; AddProducer(new Producer(BA.First, BA.Second)); }
				if (BR.First != null && BR.First.Scale > BR.Second) { Continue = true; RescaleProducer(BR.First, BR.Second + (BR.First.Scale - BR.Second) / 2); }
				if (!Continue) break;
			}
		}

		public void DiscoverBorder()
		{
			foreach (MicroRegion Current in _Regions)
			{
				foreach (MicroRegion N in Current.Neighbors())
				{
					if (N.Region != this)
					{
						if (!IsNeighbor(N.Region)) AddNeighbor(N.Region, true);
						if (!N.Oceanic && !Current.Oceanic) UpdateNeighbor(N.Region, false);
						if (N.Region != this && !Current.Oceanic && Area >= N.Region.Area) N.Border = true;
					}
					if (N.Oceanic && !Current.Oceanic) { N.Coastal = true; _Coast++; }
				}
			}
		}

		public double Desirability(Region Region)
		{
			if (Region == this) return 0;

			double Economic = Math.Max(_Culture.Indulgence, _Culture.UncertaintyAvoidance) * (Region.FlowPerCapita() / FlowPerCapita());
			double Territorial = Math.Max(_Culture.Toughness, _Culture.Individualism) * (Region.Area / Area);
			double Cultural = Math.Max(1 - _Culture.Individualism, _Culture.LongTermOrientation) / _Culture.DistanceTo(Region._Culture);
			return (Economic + Territorial + Cultural) * Math.Sqrt(_ShortestEdge / DistanceTo(Region));
		}

		public void Tick(Random Random)
		{
			double expeditionChance = _Culture.Explorativity() / (1 - _Culture.Explorativity());
		}

		public void TriggerEvent(GameEvent Event)
		{
			Event.Apply(this);
			if (OnEvent != null) OnEvent(this, Event);
		}
	}
}

using System;
using System.Collections.Generic;

using Cardamom.Graphing;
using Cardamom.Utilities;

using Cence;

using AndrassyII;

using ROTNS.Model.WorldGeneration;

namespace ROTNS.Model
{
	public class World
	{
		MapGeneratorSettings _Settings;
		LatticeNoiseGenerator _Noise;
		LatticeNoiseGenerator _MoistureNoise;
		MicroRegion[,] _MicroRegions;
		Region[] _Regions;
		float _WaterLevel;

		float[,] _HeightMap;
		float[,] _MoistureMap;
		float[,] _Shade;

		public MapGeneratorSettings Settings { get { return _Settings; } }
		public Region[] Regions { get { return _Regions; } }
		public float Water { get { return _Settings.WaterLevel; } }
		public int Height { get { return _MicroRegions.GetLength(1); } }
		public int Width { get { return _MicroRegions.GetLength(0); } }
		public MicroRegion this[int X, int Y] { get { return _MicroRegions[X, Y]; } }

		public World(Random Random, MapGeneratorSettings Settings)
		{
			_Settings = Settings;
			_Noise = new LatticeNoiseGenerator(Random, Settings.Terrain);
			_MoistureNoise = new LatticeNoiseGenerator(Random, Settings.Moisture);
			_WaterLevel = Settings.WaterLevel;

			_MicroRegions = new MicroRegion[Settings.Width, Settings.Height];
			_HeightMap = new float[Settings.Width, Settings.Height];
			_MoistureMap = new float[Settings.Width, Settings.Height];
			for (int i = 0; i < Settings.Width; ++i)
			{
				for (int j = 0; j < Settings.Height; ++j)
				{
					float n = (float)_Noise.Generate(i, j);
					float h = n > _WaterLevel ? (n - _WaterLevel) / (1 - _WaterLevel) : n / _WaterLevel - 1;
					float m = (float)_MoistureNoise.Generate(i, j);
					_HeightMap[i, j] = h;
					_MoistureMap[i, j] = m;
					Biome B = Settings.BiomeMap.Closest(HeightAt(i, j), TemperatureAt(i, j), Moisture(i, j));
					_MicroRegions[i, j] = new MicroRegion(i, j, B, this, h <= 0);
				}
			}
			_Shade = new FloatingImage(_HeightMap, Channel.RED)
				.Filter(new Cence.Filters.Emboss()).GetChannel(Channel.RED);

			CreateRegions(Random, Settings.Regions, Settings.Language);
			CreateResources(Random, Settings.Population, Settings.Resource, Settings.Resources);
			InitializeEconomy();
		}

		public MicroRegion Get(int X, int Y) { return _MicroRegions[X, Y]; }

		public float Moisture(int X, int Y)
		{
			float period = _Settings.MaxLatitude - _Settings.MinLatitude;
			double pos = _Settings.MinLatitude + ((float)Y / _Settings.Height) * 4 * period;
			return (float)((1 - Math.Cos(pos + _MoistureMap[X, Y] * Math.PI * 2.5)) / 2) * .25f + _MoistureMap[X, Y] * .75f;
		}
		public float Shade(int X, int Y) { return _Shade[X, Y]; }

		public float TemperatureAt(int X, int Y) { return .8f * GetLatitudeTemp(Y) + .2f * (1 - HeightAt(X, Y)); }

		public float HeightAt(int X, int Y)
		{
			return _HeightMap[X, Y];
		}

		float GetLatitudeTemp(float y)
		{
			float period = _Settings.MaxLatitude - _Settings.MinLatitude;
			double pos = _Settings.MinLatitude + (y / _Settings.Height) * 2 * period;
			return (float)(1 - Math.Cos(pos)) / 2;
		}

		float GetLatitudeMoisture(float y)
		{
			float period = _Settings.MaxLatitude - _Settings.MinLatitude;
			double pos = _Settings.MinLatitude + (y / _Settings.Height) * 6 * period;
			return (float)(1 - Math.Cos(pos)) / 2;
		}

		void CreateRegions(Random Random, int Number, Language Language)
		{
			MicroRegion[] Arr = new MicroRegion[_Settings.Height * _Settings.Width];
			for (int i = 0; i < _Settings.Width; ++i)
			{
				for (int j = 0; j < _Settings.Height; ++j)
				{
					Arr[i * _Settings.Height + j] = _MicroRegions[i, j];
				}
			}

			for (int i = 0; i < Arr.Length; ++i)
			{
				MicroRegion T = Arr[i];
				int index = Random.Next(0, Arr.Length);
				Arr[i] = Arr[index];
				Arr[index] = T;
			}

			DijkstraPool<MicroRegion> pool = new DijkstraPool<MicroRegion>();
			CultureMap cultureMap = new CultureMap(Random, _Settings.Culture);

			_Regions = new Region[Number];
			int c = 0;
			for (int i = 0; i < Arr.Length && c < Number; ++i)
			{
				if (!Arr[i].Oceanic && Random.NextDouble() < Math.Sqrt(Arr[i].Biome.RegionSlow))
				{
					string Name = Language.Generate(Random).Orthography;
					Region R = new Region(
						char.ToUpper(Name[0]) + Name.Substring(1),
						Arr[i],
						cultureMap.Generate(Arr[i].X, Arr[i].Y),
						_Settings.Economy);
					pool.Drop(R);
					_Regions[c] = R;
					++c;
					R.Culture.Colors = _Settings.FlagColorMap.Closest(R.Culture, 3);
					R.Administration.Flag = Settings.FlagData.CreateFlag(R.Culture, Settings.FlagColorMap, Random);
					R.Administration.GovernmentForm = GovernmentForm.AllValidGovernmentForms(
						g => !g.Integrated && g.Devolved && g.Tributary).ArgMax(R.Culture.Favorability);
				}
			}
			pool.Resolve();
			foreach (Region R in _Regions) R.DiscoverBorder();
		}

		void InitializeEconomy()
		{
			foreach (Region R in _Regions) R.InitializeEconomy();
		}

		void CreateResources(Random Random, LatticeNoiseSettings Population, LatticeNoiseSettings Resource, NaturalResource[] Resources)
		{
			Console.WriteLine("POPULATING");
			LatticeNoiseGenerator PopulationNoise = new LatticeNoiseGenerator(Random, Population);
			for (int i = 0; i < _Regions.Length; ++i)
			{
				float f = _Regions[i].Center.Biome.RegionSlow;
				_Regions[i].AddPopulation(this, (float)((PopulationNoise.Generate(_Regions[i].Center.X, _Regions[i].Center.Y)) * Math.Sqrt(f)) + .5f);
			}

			foreach (NaturalResource R in Resources)
			{
				Console.WriteLine("{0} {1}", R.Name, R.Noisy);
				LatticeNoiseGenerator Noise = R.Noisy ? new LatticeNoiseGenerator(Random, Resource) : null;
				foreach (Region Region in _Regions)
				{
					float amount = (float)R.Distribute(
						R.Noisy ? (float)Noise.Generate(Region.Center.X, Region.Center.Y) : 0, Region.Center);
					Region.AddResource(R, amount);
				}
			}
		}

		public IEnumerable<MicroRegion> Neighbors(int X, int Y)
		{
			if (X > 0)
			{
				yield return _MicroRegions[X - 1, Y];
				if (Y > 0) yield return _MicroRegions[X - 1, Y - 1];
				if (Y < _Settings.Height - 1) yield return _MicroRegions[X - 1, Y + 1];
			}
			if (X < _Settings.Width - 1)
			{
				yield return _MicroRegions[X + 1, Y];
				if (Y > 0) yield return _MicroRegions[X + 1, Y - 1];
				if (Y < _Settings.Height - 1) yield return _MicroRegions[X + 1, Y + 1];
			}
			if (Y > 0) yield return _MicroRegions[X, Y - 1];
			if (Y < _Settings.Height - 1) yield return _MicroRegions[X, Y + 1];
		}

		public float WealthPercentile(Region Region)
		{
			Region[] R = new Region[_Regions.Length];
			Array.Copy(_Regions, R, _Regions.Length);
			double[] E = new double[_Regions.Length];
			for (int i = 0; i < _Regions.Length; ++i) E[i] = R[i].FlowPerCapita();
			Array.Sort(E, R);
			return (float)(Array.IndexOf(R, Region) + 1) / R.Length;
		}
	}
}

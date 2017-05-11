﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Utilities.Noise;
using Cardamom.Graphing;
using Cardamom.Utilities;

using SFML.Graphics;

using AndrassyII;

using ROTNS.View;

namespace ROTNS.Model
{
    public class World
    {
        MapGeneratorSettings _Settings;
        PerlinNoiseGenerator _Noise;
        PerlinNoiseGenerator _MoistureNoise;
        MicroRegion[,] _MicroRegions;
        Region[] _Regions;
        Nation[] _Nations;
        float _WaterLevel;

        float[,] _HeightMap;
        float[,] _MoistureMap;
        float[,] _Shade;

        public MapGeneratorSettings Settings { get { return _Settings; } }
        public Region[] Regions { get { return _Regions; } }
        public float Water { get { return _Settings.WaterLevel; } }
        public int Height { get { return _MicroRegions.GetLength(1); } }
        public int Width { get { return _MicroRegions.GetLength(0); } }
        public MicroRegion this[int X, int Y] { get { return _MicroRegions[X,Y]; } }

        public World(Random Random, MapGeneratorSettings Settings)
        {
            _Settings = Settings;
            _Noise = new PerlinNoiseGenerator(Random, Settings.Terrain.Grain);
            _MoistureNoise = new PerlinNoiseGenerator(Random, Settings.Moisture.Grain);
            _WaterLevel = Settings.WaterLevel;

            _MicroRegions = new MicroRegion[Settings.Width, Settings.Height];
            _HeightMap = new float[Settings.Width, Settings.Height];
            _MoistureMap = new float[Settings.Width, Settings.Height];
            for (int i = 0; i < Settings.Width; ++i)
            {
                for (int j = 0; j < Settings.Height; ++j)
                {
                    float n = (float)Denormalize(_Noise.Generate(i, j, _Settings.Terrain.Octaves, _Settings.Terrain.Persistence));
                    float h = n > _WaterLevel ? (n - _WaterLevel) / (1 - _WaterLevel) : n / _WaterLevel - 1;
                    float m = (float)Denormalize(_MoistureNoise.Generate(i, j, Settings.Moisture.Octaves, Settings.Moisture.Persistence));
                    _HeightMap[i, j] = h;
                    _MoistureMap[i, j] = m;
                    Biome B = Settings.BiomeMap.Closest(HeightAt(i, j), TemperatureAt(i,j), Moisture(i, j));
                    _MicroRegions[i, j] = new MicroRegion(i, j, B,this, h <= 0);
                }
            }
            Filter F = new Filter(new double[,] { { -1, -1, 0 }, { -1, 0, 1 }, { 0, 1, 1 } }, 1, .7);
            _Shade = F.Apply(_HeightMap);

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

            DijkstraPool<MicroRegion> Pool = new DijkstraPool<MicroRegion>();
            PerlinNoiseGenerator Individualism = new PerlinNoiseGenerator(Random, _Settings.Culture.Grain);
            PerlinNoiseGenerator Indulgence = new PerlinNoiseGenerator(Random, _Settings.Culture.Grain);
            PerlinNoiseGenerator LongTermOrientation = new PerlinNoiseGenerator(Random, _Settings.Culture.Grain);
            PerlinNoiseGenerator PowerDistance = new PerlinNoiseGenerator(Random, _Settings.Culture.Grain);
            PerlinNoiseGenerator Toughness = new PerlinNoiseGenerator(Random, _Settings.Culture.Grain);
            PerlinNoiseGenerator UncertaintyAvoidance = new PerlinNoiseGenerator(Random, _Settings.Culture.Grain);

            _Regions = new Region[Number];
            int c = 0;
            for (int i = 0; i < Arr.Length && c < Number; ++i)
            {
                if (!Arr[i].Oceanic && Random.NextDouble() < Math.Sqrt(Arr[i].Biome.RegionSlow))
                {
                    Culture C = new Culture();
                    C.Individualism = (float)Denormalize(Individualism.Generate(Arr[i].X, Arr[i].Y, _Settings.Culture.Octaves, _Settings.Culture.Persistence));
                    C.Indulgence = (float)Denormalize(Indulgence.Generate(Arr[i].X, Arr[i].Y, _Settings.Culture.Octaves, _Settings.Culture.Persistence));
                    C.LongTermOrientation = (float)Denormalize(LongTermOrientation.Generate(Arr[i].X, Arr[i].Y, _Settings.Culture.Octaves, _Settings.Culture.Persistence));
                    C.PowerDistance = (float)Denormalize(PowerDistance.Generate(Arr[i].X, Arr[i].Y, _Settings.Culture.Octaves, _Settings.Culture.Persistence));
                    C.Toughness = (float)Denormalize(Toughness.Generate(Arr[i].X, Arr[i].Y, _Settings.Culture.Octaves, _Settings.Culture.Persistence));
                    C.UncertaintyAvoidance = (float)Denormalize(UncertaintyAvoidance.Generate(Arr[i].X, Arr[i].Y, _Settings.Culture.Octaves, _Settings.Culture.Persistence));
                    string Name = Language.Generate(Random).Orthography;
                    Region R = new Region(Char.ToUpper(Name[0]) + Name.Substring(1), Arr[i], C, _Settings.Economy);
                    Pool.Drop(R);
                    _Regions[c] = R;
                    ++c;
                    C.Colors = _Settings.FlagColorMap.Closest(C, 3);
                    R.Administration.Flag = Settings.FlagData.CreateFlag(C, Settings.FlagColorMap, Random);
					R.Administration.GovernmentForm = GovernmentForm.AllValidGovernmentForms(
						g => !g.Integrated && g.Devolved && g.Tributary).ArgMax(C.Favorability);
                }
            }
            Pool.Resolve();
            foreach (Region R in _Regions) R.DiscoverBorder();
        }

        void InitializeEconomy()
        {
            foreach (Region R in _Regions) R.InitializeEconomy();
        }

        void CreateResources(Random Random, NoiseSettings Population, NoiseSettings Resource, NaturalResource[] Resources)
        {
            Console.WriteLine("POPULATING");
            PerlinNoiseGenerator PopulationNoise = new PerlinNoiseGenerator(Random, Population.Grain);
            for (int i = 0; i < _Regions.Length; ++i)
            {
                    float f = _Regions[i].Center.Biome.RegionSlow;
                    _Regions[i].AddPopulation(this,(float)((Denormalize(PopulationNoise.Generate(_Regions[i].Center.X,_Regions[i].Center.Y, _Settings.Population.Octaves, _Settings.Population.Persistence))) * Math.Sqrt(f)) + .5f);
            }

            foreach (NaturalResource R in Resources)
            {
                Console.WriteLine("{0} {1}", R.Name, R.Noisy);
                PerlinNoiseGenerator Noise = R.Noisy ? new PerlinNoiseGenerator(Random, Resource.Grain) : null;
                foreach (Region Region in _Regions)
                {
                        float amount = (float)R.Distribute(R.Noisy ? (float)Denormalize(Noise.Generate(Region.Center.X, Region.Center.Y, _Settings.Resource.Octaves, _Settings.Resource.Persistence)) : 0, Region.Center);
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

        private double Denormalize(double X)
        {
            double x = 3.53553 - 7.07107 * X;
            double p = .3275911;
            double t = 1 / (1 + p * x);
            double a1 = .254829592;
            double a2 = -.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double r = t * (a1 + t * (a2 + t * (a3 + t * (a4 + t * a5)))) * Math.Exp(-x * x) / 2;
            return r > 1 ? 1 : r;
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
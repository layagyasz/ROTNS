﻿using System;

using Cardamom.Interface;
using Cardamom.Serialization;
using SFML.Window;
using SFML.Graphics;

using ROTNS.Model;
using ROTNS.Model.Flags;

using AndrassyII;
using Cence;
using Venetia;

namespace ROTNS
{
	class Program
	{
		public static readonly GameRunner RUNNER =
			new GameRunner(new Interface(VideoMode.DesktopMode, "Rise of the Nation States", Styles.Default));

		static double Denormalize(double X)
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
			return r;
		}

		static void Main(string[] args)
		{
			Random Random = new Random();
			ClassLibrary.Instance.ReadFile("Theme.blk");

			MapGeneratorSettings Settings = new MapGeneratorSettings();
			Settings.Height = 700;
			Settings.Width = 1000;
			Settings.Regions = 400;
			Settings.MaxLatitude = (float)Math.PI / 2;
			Settings.MinLatitude = (float)Math.PI / 8;
			//Settings.MaxLatitude = (float)Math.PI;
			//Settings.MinLatitude = 0;
			Settings.Terrain.Frequency = Constant.Create(1d / 200);
			Settings.Terrain.Octaves = 8;
			Settings.Terrain.Persistence = Constant.Create(.6);
			Settings.Terrain.PostModification = (x, y, v) => Denormalize(v);
			Settings.Moisture.Frequency = Constant.Create(1d / 600);
			Settings.Moisture.Octaves = 6;
			Settings.Moisture.Persistence = Constant.Create(.8);
			Settings.Moisture.PostModification = (x, y, v) => Denormalize(v);
			Settings.Population.Octaves = 6;
			Settings.Population.Frequency = Constant.Create(1d / 40);
			Settings.Population.Persistence = Constant.Create(.8);
			Settings.Population.PostModification = (x, y, v) => Denormalize(v);
			Settings.Resource.Octaves = 8;
			Settings.Resource.Frequency = Constant.Create(1d / 400);
			Settings.Resource.Persistence = Constant.Create(.7);
			Settings.Culture.Octaves = 6;
			Settings.Culture.Persistence = Constant.Create(.8);
			Settings.Culture.Frequency = Constant.Create(1d / 200);
			Settings.Culture.PostModification = (x, y, v) => Denormalize(v);
			Settings.WaterLevel = .3f;
			Image Basic = new Image("Graphics/Basic.bmp");
			Image Grainy = new Image("Graphics/Grainy.bmp");
			Image Grassy = new Image("Graphics/Grassy.bmp");
			Image Mountains = new Image("Graphics/Mountain.bmp");
			Image Trees = new Image("Graphics/Trees.bmp");
			Biome[] Biomes =
			{
				new Biome(0, .5f, 1, .5f, new Color(102, 51, 0), Trees), //Bog
                new Biome(0, .8f, 1, .5f, new Color(0, 102, 0), Trees), //Mangrove
                new Biome(.5f, .2f, .2f, .25f, new Color(220, 220, 220), Grainy), //Tundra
                new Biome(.5f, .5f, .4f, 4f, new Color(51, 102, 0), Grassy), //Grasslands
                new Biome(.5f, .4f, .7f, 3.5f, new Color(25, 50, 0), Trees), //Forest
                new Biome(.5f, .8f, .15f, .5f, new Color(255, 255, 153), Grainy), //Desert
                new Biome(.5f, .8f, .8f, .5f, new Color(51, 51, 0), Trees), //Rainforest
                new Biome(.5f, .25f, .5f, .25f, new Color(204, 255, 153), Grassy), //Steppe
                new Biome(.5f, .7f, .25f, 1f, new Color(255, 255, 102), Grassy), //Savanna
                new Biome(.5f, .8f, .7f, .5f, new Color(102, 102, 0), Trees), //Tropical Forest
                new Biome(.7f, .3f, .6f, 3f, new Color(0, 50, 0), Trees), //Coniferous Forest
                new Biome(1f, .5f, .5f, 5f, new Color(100, 100, 100), Mountains) //Rocky
            };
			Biome[] WaterBiomes =
			{
				new Biome(-.1f, .5f, 1, 20f, new Color(0,150,150), Basic), //Coast
                new Biome(-.15f, .5f, 1, 20f, new Color(0, 70, 70), Basic), //Sea
                new Biome(-.65f, .5f, 1, 20f, new Color(0,0,50), Basic) //Ocean
            };
			Settings.BiomeMap = new BiomeMap(Biomes, WaterBiomes);
			FlagColor[] FlagColors =
			{
				new FlagColor("Red", new Culture(.65f, .5f, .25f, .5f, 1f, .25f), new Color(255,0,0), 1.25f),
				new FlagColor("Orange", new Culture(.5f, .5f, .75f, .5f, .75f, .65f), new Color(255, 102, 0), .75f),
				new FlagColor("Yellow", new Culture(.5f, .75f, .25f, .5f, 0f, .5f), new Color(255,255,0), .75f),
				new FlagColor("Green", new Culture(.75f, .35f, .75f, .5f, .65f, .5f), new Color(0,102,0), 1),
				new FlagColor("Blue", new Culture(1f, .5f, 1f, .5f, .25f, .85f), new Color(0,0,255), 1),
				new FlagColor("Purple", new Culture(.5f, 1f, .5f, .35f, .65f, .5f), new Color(153,0,153), 1),
				new FlagColor("White", new Culture(1f, 0f, 1f, .5f, .5f, .5f), new Color(255,255,255), 1.25f),
				new FlagColor("Black", new Culture(.5f, .35f, .5f, 0f, .65f, .35f), new Color(5,5,5), 1),
				new FlagColor("Light Blue", new Culture(.5f, .5f, .5f, .5f, 0f, 1f), new Color(51, 102, 255), 1),
				new FlagColor("Brown", new Culture(.75f,.5f, .75f, .5f, .5f, .75f), new Color(102, 51, 0), .75f),
				new FlagColor("Gray", new Culture(.5f, .5f, .5f, 0f, .7f, 1f), new Color(127,127,127), .75f),
				new FlagColor("Pink", new Culture(.25f, 1f, .4f, .75f, 0, .75f), new Color(255,127,127), .75f)
			};
			Settings.FlagColorMap = new FlagColorMap(FlagColors);
			NaturalResource[] Resources =
			{
				new NaturalResource("property", 1, .3, false, (Noise, Region) => (float)Region.Region.Area),
				new NaturalResource("res_iron", 1, .4, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 1000 * (3 * Noise * Region.Height + Noise) / 4;}),
				new NaturalResource("res_gold", 1, .7, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 100 * (3 * Noise * Region.Height + Noise) / 4;}),
				new NaturalResource("res_silver", 1, .6, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 100 * (3 * Noise * Region.Height + Noise) / 4;}),
				new NaturalResource("res_copper", 1, .35, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 400 * (3 * Noise * Region.Height + Noise) / 4;}),
				new NaturalResource("res_tin", 1, .4, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 400 * (3 * Noise * Region.Height + Noise) / 4;}),
				new NaturalResource("res_zinc", 1, .45, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 300 * (3 * Noise * Region.Height + Noise) / 4;}),
				new NaturalResource("res_arsenic", 1, .4, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 200 * (3 * Noise * Region.Height + Noise) / 4;}),
				new NaturalResource("res_lime", 1, .3, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 10000 * (3 * Noise * Region.Height + Noise) / 4;}),
				new NaturalResource("res_clay", 1, .3, true, (Noise, Region) =>  { if(Region.Oceanic) return 0; else return 10000 * Noise;}),
				new NaturalResource("res_wheat", 1, .2, false, (Noise, Region) =>
				{
					if(Region.Oceanic) return 0;
					else
					{
						float m = 1 - 3 * Math.Abs(Region.Moisture - .5f);
						if (m < 0) m = 0;
						float t = 1 - 3 * Math.Abs(Region.Temperature - .55f);
						if (t < 0) t = 0;
						return 10000 * (float)Math.Sqrt(m * t);
					}
				}),
				new NaturalResource("res_rice", 1, .15, false, (Noise, Region) =>
				{
					if(Region.Oceanic) return 0;
					else
					{
						float m = 1 - 3 * Math.Abs(Region.Moisture - .75f);
						if (m < 0) m = 0;
						float t = 1 - 3 * Math.Abs(Region.Temperature - .7f);
						if (t < 0) t = 0;
						return 10000 * (float)Math.Sqrt(m * t);
					}
				}),
				new NaturalResource("res_barley", 1, .2, false, (Noise, Region) =>
				{
					if(Region.Oceanic) return 0;
					else
					{
						float m = 1 - 2 * Math.Abs(Region.Moisture - .65f);
						if (m < 0) m = 0;
						float t = 1 - 3 * Math.Abs(Region.Temperature - .65f);
						if (t < 0) t = 0;
						return 10000 * (float)Math.Sqrt(m * t);
					}
				}),
				new NaturalResource("res_corn", 1, .2, false, (Noise, Region) =>
				{
					if(Region.Oceanic) return 0;
					else
					{
						float m = 1 - Math.Abs(Region.Moisture - .4f);
						if (m < 0) m = 0;
						float t = 1 - 4 * Math.Abs(Region.Temperature - .6f);
						if (t < 0) t = 0;
						return 10000 * (float)Math.Sqrt(m * t);
					}
				}),
				new NaturalResource("res_game", 1, .3, false, (Noise, Region) =>
				{
					return 1000 * (float)Math.Sqrt(Region.Moisture);
				}),
				new NaturalResource("res_cotton", 1, .25, false, (Noise, Region) =>
				{
					if(Region.Oceanic) return 0;
					else
					{
						float m = 1 - 2 * Math.Abs(Region.Moisture - .75f);
						if (m < 0) m = 0;
						float t = 1 - 2 * Math.Abs(Region.Temperature - .7f);
						if (t < 0) t = 0;
						return 10000 * (float)Math.Sqrt(m * t);
					}
				}),
				new NaturalResource("res_fish", 1, .2, true, (Noise, Region) =>
				{
					return 1000 * Noise * Region.Region.Coast;
				}),
				new NaturalResource("res_salt", 1, .35, true, (Noise, Region) =>
				{
					if (Region.Oceanic) return 0;
					else return 750 * (3 * Noise * Region.Height + Noise) / 4 + 500 * Region.Region.Coast;
				}),
				new NaturalResource("res_spice", 1, .7, false, (Noise, Region) =>
				{
					if(Region.Oceanic) return 0;
					else
					{
						float m = 1 - 3 * Math.Abs(Region.Moisture - .25f);
						if (m < 0) m = 0;
						float t = 1 - 3 * Math.Abs(Region.Temperature - .75f);
						if (t < 0) t = 0;
						return 1000 * (float)Math.Sqrt(m * t);
					}
				}),
				new NaturalResource("res_silk", 1, .7, false, (Noise, Region) =>
				{
					if(Region.Oceanic) return 0;
					else
					{
						float m = 1 - 3 * Math.Abs(Region.Moisture - .75f);
						if (m < 0) m = 0;
						float t = 1 - 4 * Math.Abs(Region.Temperature - .7f);
						if (t < 0) t = 0;
						return 1000 * (float)Math.Sqrt(m * t);
					}
				}),
				new NaturalResource("res_sand", 1, .7, false, (Noise, Region) =>
				{
					if (Region.Oceanic) return 0;
					else
					{
						float m = 1 - 3 * Math.Abs(Region.Moisture);
						if (m < 0) m = 0;
						float t = 1 - 3 * Math.Abs(Region.Temperature);
						if (t < 0) t = 0;
						return 100000 * (float)Math.Sqrt(m * t) + 5000 * Region.Region.Coast;
					}
				}),
				new NaturalResource("res_water", 1, .4, 1, false, (Noise, Region) =>
				{
					if (Region.Oceanic) return 0;
					else return (float)(Region.Region.Area * Region.Moisture);
				})
			};

			Settings.Resources = Resources;
			Settings.Economy = new Economy(
				"Economy.blk",
				null /* GoodParser */,
				null /*ServiceParser */,
				i => Array.Find(Resources, j => j.Name == i.String),
				null /* ProcessParser */);
			foreach (Resource R in Settings.Resources) Settings.Economy.AddTangible(R.Name, R);
			Settings.Language = new Language("Language.txt");
			Settings.FlagData = new FlagData("Flags.blk");

			RUNNER.SetWorld(new World(Random, Settings));
			RUNNER.Start();

			/*
            AndrassyII.LanguageGenerator.LanguageGenerator LG = new AndrassyII.LanguageGenerator.LanguageGenerator(ParseBlock.FromFile("LangGen.txt"));
            for (int i = 0; i < 100; ++i)
            {
                File.AppendAllText("out.txt", LG.TempGen(new Random()));
            }
			Console.ReadLine();
			*/
		}
	}
}

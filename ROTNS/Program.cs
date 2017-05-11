using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Cardamom.Interface;
using Cardamom.Serialization;
using SFML.Window;
using SFML.Graphics;

using ROTNS.View;
using ROTNS.Model;
using ROTNS.Model.Flags;

using AndrassyII;
using Venetia;

namespace ROTNS
{
    class Program
    {
		public static readonly GameRunner RUNNER = 
			new GameRunner(new Interface(VideoMode.DesktopMode, "Rise of the Nation States", Styles.Default));
		
        static void Main(string[] args)
        {
            Random Random = new Random();
            Cardamom.Interface.ClassLibrary.Instance.ReadFile("Theme.blk");

            MapGeneratorSettings Settings = new MapGeneratorSettings();
            Settings.Height = 700;
            Settings.Width = 1000;
            Settings.Regions = 400;
            Settings.MaxLatitude = (float)Math.PI / 2;
            Settings.MinLatitude = (float)Math.PI / 8;
            //Settings.MaxLatitude = (float)Math.PI;
            //Settings.MinLatitude = 0;
            Settings.Terrain.Grain = 200;
            Settings.Terrain.Octaves = 8;
            Settings.Terrain.Persistence = .6f;
            Settings.Moisture.Grain = 600;
            Settings.Moisture.Octaves = 6;
            Settings.Moisture.Persistence = .8f;
            Settings.Population.Octaves = 6;
            Settings.Population.Grain = 40;
            Settings.Population.Persistence = .8f;
            Settings.Resource.Octaves = 8;
            Settings.Resource.Grain = 400;
            Settings.Resource.Persistence = .7f;
            Settings.Culture.Octaves = 6;
            Settings.Culture.Persistence = .8f;
            Settings.Culture.Grain = 200;
            Settings.WaterLevel = .3f;
            Image Basic = new Image("Graphics/Basic.bmp");
            Image Grainy = new Image("Graphics/Grainy.bmp");
            Image Grassy = new Image("Graphics/Grassy.bmp");
            Image Mountains = new Image("Graphics/Mountain.bmp");
            Image Trees = new Image("Graphics/Trees.bmp");
            Biome[] Biomes = new Biome[]
            {
                new Biome(0, .5f, 1, .5f, new Color(102, 51, 0), Trees), //Bog
                new Biome(0, .8f, 1, .5f, new Color(0, 102, 0), Trees), //Mangrove
                new Biome(.5f, .2f, .2f, .25f, new Color(220, 220, 220), Grainy), //Tundra
                new Biome(.5f, .5f, .4f, 5f, new Color(51, 102, 0), Grassy), //Grasslands
                new Biome(.5f, .4f, .7f, 3.5f, new Color(25, 50, 0), Trees), //Forest
                new Biome(.5f, .8f, .15f, .5f, new Color(255, 255, 153), Grainy), //Desert
                new Biome(.5f, .8f, .8f, .5f, new Color(51, 51, 0), Trees), //Rainforest
                new Biome(.5f, .25f, .5f, .25f, new Color(204, 255, 153), Grassy), //Steppe
                new Biome(.5f, .7f, .25f, 1f, new Color(255, 255, 102), Grassy), //Savanna
                new Biome(.5f, .8f, .7f, .5f, new Color(102, 102, 0), Trees), //Tropical Forest
                new Biome(.7f, .3f, .6f, 3f, new Color(0, 50, 0), Trees), //Coniferous Forest
                new Biome(1f, .5f, .5f, 1f, new Color(100, 100, 100), Mountains) //Rocky
            };
            Biome[] WaterBiomes = new Biome[]
            {
                new Biome(-.1f, .5f, 1, 20f, new Color(0,150,150), Basic), //Coast
                new Biome(-.15f, .5f, 1, 20f, new Color(0, 70, 70), Basic), //Sea
                new Biome(-.65f, .5f, 1, 20f, new Color(0,0,50), Basic) //Ocean
            };
            Settings.BiomeMap = new BiomeMap(Biomes, WaterBiomes);
            FlagColor[] FlagColors = new FlagColor[]
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
            NaturalResource[] Resources = new NaturalResource[]
            {
                new NaturalResource("res_iron", 1, .4, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 1000 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_gold", 1, .7, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 100 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_silver", 1, .6, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 100 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_copper", 1, .35, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 400 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_tin", 1, .4, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 400 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_zinc", 1, .45, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 300 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_arsenic", 1, .4, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 200 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_lime", 1, .3, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 10000 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_clay", 1, .3, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 10000 * Noise;}),
                new NaturalResource("res_wheat", 1, .2, false, delegate(float Noise, MicroRegion Region)
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
                new NaturalResource("res_rice", 1, .15, false, delegate(float Noise, MicroRegion Region)
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
                new NaturalResource("res_barley", 1, .2, false, delegate(float Noise, MicroRegion Region)
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
                new NaturalResource("res_corn", 1, .2, false, delegate(float Noise, MicroRegion Region)
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
                new NaturalResource("res_game", 1, .3, false, delegate(float Noise, MicroRegion Region)
                {
                    return 1000 * (float)Math.Sqrt(Region.Moisture);
                }),
                new NaturalResource("res_cotton", 1, .25, false, delegate(float Noise, MicroRegion Region)
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
                new NaturalResource("res_fish", 1, .2, true, delegate(float Noise, MicroRegion Region)
                {
                    return 1000 * Noise * Region.Region.Coast;
                }),
                new NaturalResource("res_salt", 1, .35, true, delegate(float Noise, MicroRegion Region)
                {
                    if (Region.Oceanic) return 0;
                    else return 750 * (3 * Noise * Region.Height + Noise) / 4 + 500 * Region.Region.Coast;
                }),
                new NaturalResource("res_spice", 1, .7, false, delegate(float Noise, MicroRegion Region)
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
                new NaturalResource("res_silk", 1, .7, false, delegate(float Noise, MicroRegion Region)
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
                new NaturalResource("res_sand", 1, .7, false, delegate(float Noise, MicroRegion Region)
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
                new NaturalResource("res_water", 1, .4, 1, false, delegate(float Noise, MicroRegion Region)
                {
                    if (Region.Oceanic) return 0;
                    else return (float)(Region.Region.Area * Region.Moisture);
                })
            };

            Settings.Resources = Resources;
            Settings.Economy = new Economy();
            Settings.Economy.LoadGoods(ParseBlock.FromFile("Economy/Goods.blk"), (ParseBlock B) => new Good(B));
            Settings.Economy.LoadServices(ParseBlock.FromFile("Economy/Services.blk"), (ParseBlock B) => new Service(B));
            foreach (Resource R in Settings.Resources) Settings.Economy.AddResource(R.Name, R);
            Settings.Economy.LoadProcesses(ParseBlock.FromFile("Economy/Processes.blk"), (ParseBlock B, EconomySet<Tangible> T) => new Process(B, T));
            Settings.Language = new Language("Language.txt");
            Settings.FlagData = new Model.Flags.FlagData("Flags.blk");

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

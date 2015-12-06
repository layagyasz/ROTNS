using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface;
using Cardamom.Serialization;
using SFML.Window;
using SFML.Graphics;

using ROTNS.OverMapping;
using ROTNS.Core;

using AndrassyII;
using Venetia;

namespace ROTNS
{
    class Program
    {
        private static Color HSLtoRGB(double H, double S, double L)
        {
            double C = (1 - Math.Abs(2 * L - 1)) * S * 255;
            double X = (C * (1 - Math.Abs(((H / 60) % 2) - 1))) * 255;
            double M = (L - C / 2) * 255;

            if (H < 60) return new Color((byte)(C + M), (byte)(X + M), (byte)M);
            else if (H >= 60 && H < 120) return new Color((byte)(X + M), (byte)(C + M), (byte)M);
            else if (H >= 120 && H < 180) return new Color((byte)M, (byte)(C + M), (byte)(X + M));
            else if (H >= 180 && H < 240) return new Color((byte)M, (byte)(X + M), (byte)(C + M));
            else if (H >= 240 && H < 300) return new Color((byte)(X + M), (byte)M, (byte)(C + M));
            else return new Color((byte)(C + M), (byte)M, (byte)(X + M));
        }

        private static Color[] GenerateColors(Random Random, int Number)
        {
            Color[] R = new Color[Number];
            int c = 0;
            for (double i = 0; i < 360 && c < Number; i += 360f / Number)
            {
                double Hue = i;

                R[c] = HSLtoRGB(Hue, .8 + ((c % 2) * .1), .8 + ((c % 3) * .05));
                ++c;
            }
            return R;
        }
        static void Main(string[] args)
        {
            Random Random = new Random();
            Interface Interface = new Interface(VideoMode.DesktopMode, "Column", Styles.Default);
            Interface.Screen = new Screen();

            MapGeneratorSettings Settings = new MapGeneratorSettings();
            Settings.Height = 700;
            Settings.Width = 1000;
            Settings.Regions = 400;
            Settings.MaxLatitude = (float)Math.PI / 2;
            Settings.MinLatitude = (float)Math.PI / 8;
            Settings.MaxLatitude = (float)Math.PI;
            Settings.MinLatitude = 0;
            Settings.Terrain.Grain = 200;
            Settings.Terrain.Octaves = 8;
            Settings.Terrain.Persistence = .6f;
            Settings.Moisture.Grain = 600;
            Settings.Moisture.Octaves = 6;
            Settings.Moisture.Persistence = .8f;
            Settings.Population.Octaves = 6;
            Settings.Population.Grain = 100;
            Settings.Population.Persistence = .8f;
            Settings.Resource.Octaves = 8;
            Settings.Resource.Grain = 400;
            Settings.Resource.Persistence = .7f;
            Settings.Culture.Octaves = 6;
            Settings.Culture.Persistence = .8f;
            Settings.Culture.Grain = 500;
            Settings.WaterLevel = .5f;
            Biome[] Biomes = new Biome[]
            {
                new Biome(0, .5f, 1, .5f, new Color(102, 51, 0)), //Bog
                new Biome(0, .8f, 1, .5f, new Color(0, 102, 0)), //Mangrove
                new Biome(.5f, .2f, .2f, .25f, new Color(220, 220, 220)), //Tundra
                new Biome(.5f, .5f, .4f, 5f, new Color(51, 102, 0)), //Grasslands
                new Biome(.5f, .4f, .7f, 3.5f, new Color(25, 50, 0)), //Forest
                new Biome(.5f, .8f, .15f, .5f, new Color(255, 255, 153)), //Desert
                new Biome(.5f, .8f, .8f, .5f, new Color(51, 51, 0)), //Rainforest
                new Biome(.5f, .25f, .5f, .25f, new Color(204, 255, 153)), //Steppe
                new Biome(.5f, .7f, .25f, 1f, new Color(255, 255, 102)), //Savanna
                new Biome(.5f, .8f, .7f, .5f, new Color(102, 102, 0)), //Tropical Forest
                new Biome(.7f, .3f, .6f, 3f, new Color(0, 50, 0)), //Coniferous Forest
                new Biome(1f, .5f, .5f, 1f, new Color(100, 100, 100)) //Rocky
            };
            Biome[] WaterBiomes = new Biome[]
            {
                new Biome(-.1f, .5f, 1, 20f, new Color(0,150,150)), //Coast
                new Biome(-.15f, .5f, 1, 20f, new Color(0, 70, 70)), //Sea
                new Biome(-.65f, .5f, 1, 20f, new Color(0,0,50)) //Ocean
            };
            Settings.BiomeMap = new BiomeMap(Biomes, WaterBiomes);
            NaturalResource[] Resources = new NaturalResource[]
            {
                new NaturalResource("res_iron", 1, .4, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 500 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_gold", 1, .7, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 50 * (3 * Noise * Region.Height + Noise) / 4;}),
                new NaturalResource("res_silver", 1, .6, true, delegate(float Noise, MicroRegion Region) { if(Region.Oceanic) return 0; else return 150 * (3 * Noise * Region.Height + Noise) / 4;}),
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
                        return 10000 * (float)Math.Sqrt(Region.Moisture);
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
                        if (Region.Coastal) return 100000 * Noise;
                        else return 0;
                    }),
                    new NaturalResource("res_salt", 1, .35, true, delegate(float Noise, MicroRegion Region)
                    {
                        if (Region.Coastal) return 500;
                        else if (Region.Oceanic) return 0;
                        else return 750 * (3 * Noise * Region.Height + Noise) / 4;
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
                        if (Region.Coastal) return 5000;
                        else if (Region.Oceanic) return 0;
                        else
                        {
                            float m = 1 - 3 * Math.Abs(Region.Moisture);
                            if (m < 0) m = 0;
                            float t = 1 - 3 * Math.Abs(Region.Temperature);
                            if (t < 0) t = 0;
                            return 100000 * (float)Math.Sqrt(m * t);
                        }
                    }),
                    new NaturalResource("res_water", 1, .4, 1, false, delegate(float Noise, MicroRegion Region)
                    {
                            if (Region.Oceanic) return 0;
                            else return 10000 * Region.Moisture;
                    })
            };
            Settings.Resources = Resources;
            Settings.Economy = new Economy();
            Settings.Economy.LoadGoods(ParseBlock.FromFile("Economy/Goods.blk"), (ParseBlock B) => new Good(B));
            Settings.Economy.LoadServices(ParseBlock.FromFile("Economy/Services.blk"), (ParseBlock B) => new Service(B));
            foreach (Resource R in Settings.Resources) Settings.Economy.AddResource(R.Name, R);
            Settings.Economy.LoadProcesses(ParseBlock.FromFile("Economy/Processes.blk"), (ParseBlock B, EconomySet<Tangible> T) => new Process(B, T));
            Settings.Language = new Language("Language.txt");
            OverMap M = new OverMap(new World(Random, Settings), new Vector2f(Interface.Window.Size.X, Interface.Window.Size.Y));
            Interface.Screen.Add(M);
            InteractionController I = new InteractionController(M, Settings.Economy);
            Interface.Start();

            /*
            Interface.Screen = new Screen();
            Cardamom.Interface.ClassLibrary.Instance.ReadFile("Theme.blk");
            Interface.Screen.Add(new Cardamom.Interface.Items.Button("test") { Position = new Vector2f(100, 100) });
            Interface.Start(false);
            */
        }
    }
}
